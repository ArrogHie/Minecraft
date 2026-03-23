using UnityEngine;

/// <summary>
/// 敌人基类，提供通用属性、AI状态机和伤害系统
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class Enemy : Entity
{
    public EnemyType enemyType;
    public EnemyState state = EnemyState.Idle;

    public int maxHealth = 10;
    public int currentHealth;

    public float maxDistance = 30f;
    public int attackDamage = 3;
    public float sightRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    protected Transform target;
    protected float lastAttackTime;
    protected Vector3 spawnPosition;
    protected bool isAggro;

    protected bool isGrounded = false;
    public float groundCheckDistance = 0.3f;
    public float obstacleCheckDistance = 0.8f;
    public float obstacleCheckLowHeight = 0.35f;
    public float obstacleCheckHighHeight = 1.1f;
    public float jumpCooldown = 0.35f;
    public float stuckCheckInterval = 0.4f;
    public float stuckDistanceThreshold = 0.08f;
    public float stuckJumpCooldown = 1.2f;

    private float lastJumpTime;
    private float lastStuckCheckTime;
    private float lastStuckJumpTime;
    private Vector3 lastStuckCheckPosition;
    private Collider cachedCollider;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
        spawnPosition = transform.position;
        isAggro = false;
        lastJumpTime = -10f;
        lastStuckCheckTime = Time.time;
        lastStuckJumpTime = -10f;
        lastStuckCheckPosition = transform.position;

        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        cachedCollider = GetComponent<Collider>();
        if (jumpForce <= 0f) jumpForce = 5f;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    protected virtual void Start()
    {
        if (World.instance == null || World.instance.player == null)
        {
            Debug.LogWarning($"{enemyType}: World or player not found, disabling AI");
            enabled = false;
            return;
        }

        target = World.instance.player.transform;

        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.RegisterEnemy(this);
        }
        else
        {
            Debug.LogWarning($"{enemyType}: EnemyManager not found");
        }
    }

    protected virtual void FixedUpdate()
    {
        CheckGround();
    }

    protected virtual void Update()
    {
        if (state == EnemyState.Dead) return;
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= sightRange)
        {
            isAggro = true;
        }
        else if (distanceToPlayer > maxDistance)
        {
            isAggro = false;
        }

        if (isAggro)
        {
            if (distanceToPlayer <= attackRange + 0.2f)
            {
                state = EnemyState.Attack;
            }
            else
            {
                state = EnemyState.Chase;
            }
        }
        else
        {
            state = EnemyState.Idle;
        }

        UpdateBehavior();
    }

    protected virtual void CheckGround()
    {
        Vector3 rayOrigin;
        float rayDistance;

        if (cachedCollider != null)
        {
            rayOrigin = cachedCollider.bounds.center;
            rayDistance = cachedCollider.bounds.extents.y + groundCheckDistance;
        }
        else
        {
            rayOrigin = transform.position + Vector3.up * 0.5f;
            rayDistance = 1f + groundCheckDistance;
        }

        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayDistance, LayerMask.GetMask("Chunk"));
    }

    protected virtual bool CheckObstacle(Vector3 direction)
    {
        Vector3 lowOrigin = transform.position + Vector3.up * obstacleCheckLowHeight;
        Vector3 highOrigin = transform.position + Vector3.up * obstacleCheckHighHeight;

        bool lowBlocked = Physics.Raycast(lowOrigin, direction, obstacleCheckDistance, LayerMask.GetMask("Chunk"));
        bool highBlocked = Physics.Raycast(highOrigin, direction, obstacleCheckDistance, LayerMask.GetMask("Chunk"));

        return lowBlocked && !highBlocked;
    }

    protected virtual void TryJump(Vector3 direction)
    {
        if (!isGrounded) return;
        if (Time.time - lastJumpTime < jumpCooldown) return;
        if (Mathf.Abs(rigidbody.velocity.y) > 0.2f) return;

        if (CheckObstacle(direction))
        {
            lastJumpTime = Time.time;
            Jump();
        }
    }

    protected virtual void MoveToward(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
        {
            rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
            return;
        }

        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 12f);

        TryStuckJump(destination);
        TryJump(direction);
        rigidbody.velocity = new Vector3(direction.x * speed, rigidbody.velocity.y, direction.z * speed);
    }

    protected virtual void TryStuckJump(Vector3 destination)
    {
        if (state != EnemyState.Chase) return;
        if (!isGrounded) return;
        if (Time.time - lastStuckCheckTime < stuckCheckInterval) return;

        Vector3 currentPos = transform.position;
        Vector3 currentFlat = new Vector3(currentPos.x, 0f, currentPos.z);
        Vector3 lastFlat = new Vector3(lastStuckCheckPosition.x, 0f, lastStuckCheckPosition.z);
        float movedDistance = Vector3.Distance(currentFlat, lastFlat);

        lastStuckCheckPosition = currentPos;
        lastStuckCheckTime = Time.time;

        Vector3 destinationFlat = new Vector3(destination.x, 0f, destination.z);
        bool stillNeedMove = Vector3.Distance(currentFlat, destinationFlat) > attackRange;
        bool isStuck = movedDistance <= stuckDistanceThreshold;
        bool canStuckJump = Time.time - lastStuckJumpTime >= stuckJumpCooldown;

        if (stillNeedMove && isStuck && canStuckJump && Mathf.Abs(rigidbody.velocity.y) <= 0.2f)
        {
            lastStuckJumpTime = Time.time;
            Jump();
        }
    }

    protected abstract void UpdateBehavior();

    protected virtual void Chase()
    {
        MoveToward(target.position);
    }

    protected virtual void Idle()
    {
        float distanceToSpawn = Vector3.Distance(transform.position, spawnPosition);
        if (distanceToSpawn > 1.5f)
        {
            MoveToward(spawnPosition);
            return;
        }

        rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
    }

    protected virtual void Attack()
    {
        Vector3 lookTarget = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookTarget);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            OnAttack();
        }
    }

    protected abstract void OnAttack();

    public virtual void TakeDamage(int damage)
    {
        if (state == EnemyState.Dead) return;

        currentHealth -= damage;
        Debug.Log($"{enemyType} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        state = EnemyState.Dead;
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.UnregisterEnemy(this);
        }
        OnDeath();
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject, 2f);
    }

    public void OnDestroy()
    {
        if (EnemyManager.instance != null)
        {
            EnemyManager.instance.UnregisterEnemy(this);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        if (Application.isPlaying)
        {
            Vector3 direction = transform.forward;
            Vector3 lowOrigin = transform.position + Vector3.up * obstacleCheckLowHeight;
            Vector3 highOrigin = transform.position + Vector3.up * obstacleCheckHighHeight;
            Debug.DrawRay(lowOrigin, direction * obstacleCheckDistance, Color.red);
            Debug.DrawRay(highOrigin, direction * obstacleCheckDistance, Color.blue);

            Vector3 groundRayOrigin = cachedCollider != null ? cachedCollider.bounds.center : transform.position + Vector3.up * 0.5f;
            float groundRayDistance = cachedCollider != null ? cachedCollider.bounds.extents.y + groundCheckDistance : 1f + groundCheckDistance;
            Debug.DrawRay(groundRayOrigin, Vector3.down * groundRayDistance, isGrounded ? Color.green : Color.magenta);
        }
    }
}
