using UnityEngine;
using Pathfinding;

public class ZombieAI : MonoBehaviour
{
    public Transform player;
    public VisionCone vision;

    [Header("Hearing")]
    public float hearingRadius = 6f;
    public float detectThreshold = 0.7f;
    public float suspicionThreshold = 0.5f;
    public float amplifyHearingMultiplier = 3f;
    bool detected = false;
    [Header("Combat")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.2f;
    public int damage = 10;

    [Header("Detect")]
    public LayerMask obstacleMask;

    private float attackTimer;

    private AIPath aiPath;
    private EnemyAnimation enemyAnimation;

    private Vector3 lastHeardPosition;
    private Vector3 lastSeenPosition;

    private ZombieState currentState;

    enum ZombieState
    {
        Idle,
        Suspicious,
        Chase,
        Search,
        Attack
    }

    private void OnEnable()
    {
        SoundManager.OnSoundEmitted += HearSound;
    }
    private void OnDisable()
    {
        SoundManager.OnSoundEmitted -= HearSound;
    }
    void Start()
    {
        aiPath = GetComponent<AIPath>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        currentState = ZombieState.Idle;
        enemyAnimation.SetState(EnemyAnimation.AnimState.Idle);
    }

    void Update()
    {
        CheckVision();

        switch (currentState)
        {
            case ZombieState.Idle:
                aiPath.destination = transform.position;
                break;

            case ZombieState.Suspicious:
                aiPath.destination = lastHeardPosition;

                if (!aiPath.pathPending && aiPath.reachedEndOfPath)
                {
                    currentState = ZombieState.Idle;
                }
                break;

            case ZombieState.Chase:
                aiPath.destination = player.position;

                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    currentState = ZombieState.Attack;
                }
                else if (!vision.IsTargetVisible())
                {
                    lastHeardPosition = player.position;
                    currentState = ZombieState.Search;
                    detected = false;
                }
                break;

            case ZombieState.Search:
                aiPath.destination = lastHeardPosition;

                if (!aiPath.pathPending && aiPath.reachedEndOfPath)
                {
                    currentState = ZombieState.Idle;
                }
                break;

            case ZombieState.Attack:
                HandleAttack();
                break;
        }

        AnimationHandler();
    }

    void CheckVision()
    {
        if (vision.IsTargetVisible())
        {
            lastSeenPosition = player.position;
            currentState = ZombieState.Chase;
        }
    }

    void HandleAttack()
    {
        aiPath.destination = transform.position;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            enemyAnimation.SetState(EnemyAnimation.AnimState.Attack);

            // damage player
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = ZombieState.Chase;
        }
    }

    void AnimationHandler()
    {
        if (currentState == ZombieState.Attack)
            return;

        if (aiPath.velocity.magnitude > 0.1f)
        {
            if (currentState == ZombieState.Chase)
                enemyAnimation.SetState(EnemyAnimation.AnimState.Run);
            else
                enemyAnimation.SetState(EnemyAnimation.AnimState.Move);
        }
        else
        {
            enemyAnimation.SetState(EnemyAnimation.AnimState.Idle);
        }
    }

    // ---------------- HEARING ----------------

    public void HearSound(SoundEvent sound)
    {
        float distance = Vector3.Distance(transform.position, sound.position);

        if (distance > hearingRadius)
            return;

        float perceivedSound = sound.intensity * (1f - distance / hearingRadius) * (detected?amplifyHearingMultiplier:1f);

        if (Physics2D.Linecast(transform.position, sound.position, obstacleMask))
        {
            perceivedSound *= 0.5f;
        }

        if (perceivedSound >= detectThreshold)
        {
            lastHeardPosition = sound.position;
            currentState = ZombieState.Chase;
            detected = true;
        }
        else if (perceivedSound >= suspicionThreshold)
        {
            lastHeardPosition = sound.position;

            if (currentState != ZombieState.Chase)
                currentState = ZombieState.Suspicious;
        }

    }

    // bullet hit reaction
    public void OnHit(Vector3 hitSource)
    {
        lastHeardPosition = hitSource;
        currentState = ZombieState.Chase;
        detected = true;
    }
}