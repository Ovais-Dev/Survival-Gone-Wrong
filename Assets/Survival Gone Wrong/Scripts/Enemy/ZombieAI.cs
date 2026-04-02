using UnityEngine;
using Pathfinding;

public class ZombieAI : MonoBehaviour
{
    public ZombieState currentState;
    public Transform player;
    //public VisionCone vision;

    [Header("Hearing")]
    public float hearingRadius = 6f;
    public float detectThreshold = 0.7f;
    public float suspicionThreshold = 0.5f;
    public float amplifyHearingMultiplier = 3f;
    public float obstacleHearingMultiplier = 0.5f;
    bool detected = false;
    bool suspicious = false;

    [Header("Combat")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.2f;
    public int damage = 10;

    [Header("Hurt")]
    public float hurtCooldown = 0.5f;
    public GameObject highlightLight;

    [Header("Detect")]
    public LayerMask obstacleMask;

    [Header("Suspicious")]
    private bool isSuspiciousCooldown;
    public float suspiciousCooldownTime = 2f;

    [Header("Path Obstruction")]
    public float obstructionCheckDistance = 1.5f;
    public LayerMask obstructionMask; // assign gate layer here

    [Header("Materials Setup")]
    public Material litMat;
    public Material outlineMat;

    [Header("Hit Back Feedback")]
    [SerializeField] private float hitBackDist;

    [Header("Zombie Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip suspiciousClip;
    [SerializeField] private AudioClip hurtClip;
   // [SerializeField] private AudioClip deathClip;

    private SpriteRenderer spriteRenderer;
    private float suspiciousTimer;
    private float attackTimer;
    private float hurtTimer;

    private AIPath aiPath;
    private EnemyAnimation enemyAnimation;

    private Vector3 lastHeardPosition;
    //private Vector3 lastSeenPosition;

    private ZombieState previousStateBeforeHurt;
    private ZombieState previousStateBeforeAttack;


    Vector2 previousDir;
    bool hasBeenTriggered = false;
    public enum ZombieState
    {
        Idle,
        Suspicious,
        Chase,
        Attack,
        Hurt
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
        spriteRenderer = GetComponent<SpriteRenderer>();

        aiPath = GetComponent<AIPath>();
        enemyAnimation = GetComponent<EnemyAnimation>();

        currentState = ZombieState.Idle;
        enemyAnimation.SetState(EnemyAnimation.AnimState.Idle);

        hasBeenTriggered = false;
    }

    void Update()
    {
       // CheckVision();

        switch (currentState)
        {
            case ZombieState.Idle:
                aiPath.destination = transform.position;
                break;

            case ZombieState.Suspicious:
                if (IsPathObstructed())
                {
                    currentState = ZombieState.Idle;
                    aiPath.destination = transform.position;
                    break;
                }
                aiPath.destination = lastHeardPosition;

                // Still moving to the heard location
                float dist = Vector2.Distance(transform.position, lastHeardPosition);
                if (dist>0.5f)
                {
                    isSuspiciousCooldown = false;
                    suspiciousTimer = 0f;
                    break;
                }

                // Reached location -> start suspicious cooldown
                //aiPath.destination = transform.position;

                if (!isSuspiciousCooldown)
                {
                    isSuspiciousCooldown = true;
                    suspiciousTimer = 0f;
                }

                suspiciousTimer += Time.deltaTime;
                //Debug.Log(suspiciousTimer);
                if (suspiciousTimer >= suspiciousCooldownTime)
                {
                    isSuspiciousCooldown = false;
                    suspiciousTimer = 0f;
                    currentState = ZombieState.Idle;
                    suspicious = false;
                    spriteRenderer.material = litMat;
                    highlightLight.SetActive(false);
                }
                break;
            case ZombieState.Chase:
                if (IsPathObstructed())
                {
                    currentState = ZombieState.Idle;
                    aiPath.destination = transform.position;
                    break;
                }
                aiPath.destination = player.position;
                lastHeardPosition = player.position;
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    previousStateBeforeAttack = ZombieState.Chase;
                    currentState = ZombieState.Attack;
                }
                break;
            case ZombieState.Attack:
                HandleAttack();
                break;
            case ZombieState.Hurt:
                HandleHurt();
                break;
        }

        AnimationHandler();
    }


    void HandleAttack()
    {
        aiPath.destination = transform.position;
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = previousStateBeforeAttack;
            return;
        }
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            enemyAnimation.SetState(EnemyAnimation.AnimState.Attack);

            // 🔊 Play attack sound
            PlaySound(attackClip);

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
    void HandleHurt()
    {
        aiPath.destination = transform.position;
        transform.up = previousDir;
        hurtTimer += Time.deltaTime;

        if (hurtTimer >= hurtCooldown)
        {
            hurtTimer = 0f;

            if (previousStateBeforeHurt == ZombieState.Attack)
            {
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                    currentState = ZombieState.Attack;
                else
                    currentState = ZombieState.Chase;
            }
            else
            {
                currentState = previousStateBeforeHurt;
            }
        }
    }
    void AnimationHandler()
    {
        if (currentState == ZombieState.Attack)
        {
            enemyAnimation.SetState(EnemyAnimation.AnimState.Attack);
            return;
        }

        if (currentState == ZombieState.Hurt)
        {
            enemyAnimation.SetState(EnemyAnimation.AnimState.Idle); // replace with Hurt if you add one
            return;
        }

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
        if (currentState == ZombieState.Hurt)
            return;

        float distance = Vector3.Distance(transform.position, sound.position);

        if (distance > hearingRadius)
        {
            if (aiPath.pathPending || !aiPath.reachedDestination)
                currentState = ZombieState.Suspicious;
            else
                currentState = ZombieState.Idle;
            //currentState = suspicious ? ZombieState.Suspicious : ZombieState.Idle;
            detected = false;
            if (!suspicious) { spriteRenderer.material = litMat; highlightLight.SetActive(false); }
            return;
        }

        float perceivedSound = sound.intensity * (1f - distance / hearingRadius) * (suspicious?amplifyHearingMultiplier:1f);

        if (Physics2D.Linecast(transform.position, sound.position, obstacleMask))
        {
            perceivedSound *= hasBeenTriggered ? obstacleHearingMultiplier : 0f;
        }

        if (perceivedSound >= detectThreshold)
        {
            Detected(sound.position);
        }
        else if (perceivedSound >= suspicionThreshold)
        {
            if (currentState == ZombieState.Chase && distance < hearingRadius) return;
            Suspicious(sound.position);
        }

    }
    bool IsPathObstructed()
    {
        Vector2 dir = (aiPath.destination - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, obstructionCheckDistance, obstructionMask);

        Debug.DrawRay(transform.position, dir * obstructionCheckDistance, hit ? Color.red : Color.green);

        return hit.collider != null;
    }
    // bullet hit reaction
    public void Detected(Vector3 sourcePos)
    {
        if (!hasBeenTriggered) hasBeenTriggered = true;
        if (currentState == ZombieState.Hurt || currentState == ZombieState.Chase)
            return;
        lastHeardPosition = sourcePos;
        currentState = ZombieState.Chase;
        detected = true;
        spriteRenderer.material = outlineMat;
        highlightLight.SetActive(true);
    }
    public void Suspicious(Vector3 sourcePos)
    {
        if (currentState == ZombieState.Hurt)
            return;

        lastHeardPosition = sourcePos;

        if (currentState != ZombieState.Chase && currentState != ZombieState.Attack)
        {
            currentState = ZombieState.Suspicious;
            suspicious = true;

            // 🔊 Play growl once when becoming suspicious
            PlaySound(suspiciousClip);
        }
    }

    public void Hurt()
    {
        lastHeardPosition = player.position;
        previousDir = transform.up;

        detected = true;
        suspicious = true;
        spriteRenderer.material = outlineMat;
        highlightLight.SetActive(true);

        previousStateBeforeHurt = currentState!=ZombieState.Hurt?currentState:ZombieState.Idle;
        currentState = ZombieState.Hurt;
        hurtTimer = 0f;
        PlaySound(hurtClip);
    }
    public void HitBack(Vector3 dir)
    {
        transform.position += dir * hitBackDist;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastHeardPosition, 1f);
    }
}