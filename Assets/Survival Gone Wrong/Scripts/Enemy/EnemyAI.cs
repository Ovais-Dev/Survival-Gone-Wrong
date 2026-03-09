using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Search;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public VisionCone vision;

    public float patrolRadius = 5f;
    public float waitTime = 1.5f;
    public float rotateDuration = 2f;
    private AIPath aiPath;
    private Vector3 homePosition;
    private Vector3 lastSeenPosition;

    private EnemyState currentState;
    private InvestigatePhase investigatePhase;
    private List<Vector3> searchPoints = new List<Vector3>();
   
    //private int currentSearchIndex;

    private List<Vector3> patrolPoints = new List<Vector3>();
   
    private float waitTimer;
    private int currentPatrolIndex;
    private bool isRotating;
    private float rotateTimer;
    private float startRotation;
    private float targetRotation;
    private int searchIndex;
    private float searchWaitTime;
    private bool hasRotatedAtPoint;


    enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Investigate,
        ReturnHome
    }
    enum InvestigatePhase
    {
        GoingToLastSeen,
        SearchingPoints
    }

    private EnemyAnimation enemyAnimation;
    void Start()
    {
        aiPath = GetComponent<AIPath>();
        enemyAnimation = GetComponent<EnemyAnimation>();
        homePosition = transform.position;

        PickPatrolPoint();
        currentState = EnemyState.Patrol;
        enemyAnimation.SetState(EnemyAnimation.AnimState.Idle);

        aiPath.destination = homePosition;
        
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (vision.IsTargetVisible())
                {
                    currentState = EnemyState.Chase;
                    enemyAnimation.SetState(EnemyAnimation.AnimState.Run);
                    break;
                }
                PatrolRoutine();
                break;

            case EnemyState.Chase:
                if (vision.IsTargetVisible())
                {
                    lastSeenPosition = player.position;
                    aiPath.destination = player.position;
                }
                else
                {
                    currentState = EnemyState.Investigate;
                    investigatePhase = InvestigatePhase.GoingToLastSeen;
                    enemyAnimation.SetState(EnemyAnimation.AnimState.Move);
                    aiPath.destination = lastSeenPosition;

                    searchPoints.Clear();
                    searchIndex = 0;

                   
                }
                break;

            case EnemyState.Investigate:
                if (vision.IsTargetVisible())
                {
                    currentState = EnemyState.Chase;
                    enemyAnimation.SetState(EnemyAnimation.AnimState.Run);
                    break;
                }
                SearchRoutine();
                break;

            case EnemyState.ReturnHome:
                if (vision.IsTargetVisible())
                {
                    currentState = EnemyState.Chase;
                    enemyAnimation.SetState(EnemyAnimation.AnimState.Run);
                }
                else
                {
                    if (!aiPath.pathPending && aiPath.reachedEndOfPath)
                    {
                        PickPatrolPoint();
                        currentState = EnemyState.Patrol;
                        enemyAnimation.SetState(EnemyAnimation.AnimState.Move);
                    }
                }
                break;
        }
        AnimationHandler();
    }
    void AnimationHandler()
    {
        if (aiPath.velocity.magnitude > 0)
        {

            enemyAnimation.SetState(EnemyAnimation.AnimState.Move);

        }
        else
        {
            enemyAnimation.SetState(EnemyAnimation.AnimState.Idle);

        }
    }
    void PickPatrolPoint()
    {
        patrolPoints.Clear();
        currentPatrolIndex = 0;
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = GetRandomReachablePoint(patrolRadius);
            patrolPoints.Add(pos);
            //patrolPointsPos[i].position = pos;
        }
        //Vector2 random = Random.insideUnitCircle * patrolRadius;
        //Vector3 patrolPoint = homePosition + new Vector3(random.x, random.y, 0);
        //if (IsPointValid(patrolPoint))
        //aiPath.destination = GetRandomReachablePoint(patrolRadius);
      // StartCoroutine(PatrolRoutine());
    }
   
    public bool IsPointValid(Vector3 position)
    {
        GridGraph graph = AstarPath.active.data.gridGraph;

        GraphNode node = graph.GetNearest(position).node;

        if (node == null)
            return false;

        return node.Walkable;
    }
    public Vector3 GetRandomReachablePoint(float radius)
    {
        GraphNode startNode = AstarPath.active.GetNearest(transform.position).node;

        var reachable = PathUtilities.GetReachableNodes(startNode);

        List<GraphNode> validNodes = new List<GraphNode>();

        foreach (var node in reachable)
        {
            if (node == startNode) continue;

            Vector3 worldPos = (Vector3)node.position;

            if (Vector3.Distance(worldPos, transform.position) <= radius)
            {
                validNodes.Add(node);
            }
        }

        if (validNodes.Count > 0)
        {
            GraphNode randomNode = validNodes[Random.Range(0, validNodes.Count)];
            return (Vector3)randomNode.position;
        }

        return transform.position;
    }
    void GenerateSearchPoints()
    {
        searchPoints.Clear();
        //currentSearchIndex = 0;
        searchIndex = 0;
        for (int i = 0; i < 2; i++)
        {
            //Vector2 random = Random.insideUnitCircle * 2f;
            //searchPoints.Add(lastSeenPosition + new Vector3(random.x, random.y, 0));
            Vector3 pos = GetRandomReachablePoint(patrolRadius);
            searchPoints.Add(pos);
            //searchPointPos[i].position = pos;
        }
    }
    void PatrolRoutine()
    {
        if (!aiPath.pathPending && aiPath.reachedEndOfPath)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                aiPath.destination = patrolPoints[currentPatrolIndex];
                waitTimer = 0f;
            }
        }
    }
    void SearchRoutine()
    {
        // Move through search points
        if (aiPath.pathPending || !aiPath.reachedEndOfPath) return;

        // -------- PHASE 1: Arrived at Last Seen Position --------
        if (investigatePhase == InvestigatePhase.GoingToLastSeen)
        {
            GenerateSearchPoints();
            investigatePhase = InvestigatePhase.SearchingPoints;

            aiPath.destination = searchPoints[0];
            return;
        }

        // -------- PHASE 2: Searching Points --------
        if (investigatePhase == InvestigatePhase.SearchingPoints)
        {
            if (!hasRotatedAtPoint)
            {
                if (!isRotating)
                {
                    StartRotation();
                }
                else
                {
                    HandleRotation();

                    // Rotation finished
                    if (!isRotating)
                    {
                        hasRotatedAtPoint = true;
                        waitTimer = 0f;
                    }
                }

                return;
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= searchWaitTime)
            {
                waitTimer = 0f;
                hasRotatedAtPoint = false;
                searchIndex++;

                if (searchIndex >= searchPoints.Count)
                {
                    currentState = EnemyState.ReturnHome;
                    aiPath.destination = homePosition;
                }
                else
                {
                    aiPath.destination = searchPoints[searchIndex];
                }
            }
        }


    }
    void HandleRotation()
    {
        rotateTimer += Time.deltaTime;

        float t = rotateTimer / rotateDuration;

        float angle = Mathf.LerpAngle(startRotation, targetRotation, t);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (t >= 1f)
        {
            isRotating = false;
            rotateTimer = 0f;
        }
    }

    void StartRotation()
    {
        isRotating = true;
        rotateTimer = 0f;

        float randomAngle = Random.Range(50f, 90f);
        bool turnRight = Random.value > 0.5f;

        startRotation = transform.eulerAngles.z;

        targetRotation = turnRight ?
            startRotation - randomAngle :
            startRotation + randomAngle;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(homePosition, patrolRadius);
    }
}