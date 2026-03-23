using UnityEngine;

/// <summary>
/// 骷髅：远程敌人，血量低、移动快、发射弓箭攻击
/// </summary>
public class Skeleton : Enemy
{
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public float throwForce = 15f;

    protected override void Awake()
    {
        base.Awake();
        enemyType = EnemyType.Skeleton;
        maxHealth = 10;
        currentHealth = maxHealth;
        attackDamage = 3;
        speed = 4f;
        jumpForce = 5f;
        sightRange = 15f;
        attackRange = 8f;
        attackCooldown = 2f;
        maxDistance = 40f;
    }

    protected override void UpdateBehavior()
    {
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    protected override void OnAttack()
    {
        if (target == null) return;
        if (arrowPrefab == null || shootPoint == null)
        {
            Debug.LogWarning("Skeleton: arrowPrefab or shootPoint not set!");
            return;
        }

        Vector3 direction = (target.position - shootPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, rotation);
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.AddForce(direction * throwForce, ForceMode.Impulse);
        }

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.damage = attackDamage;
            arrowScript.shooter = this;
        }

        Debug.Log("Skeleton shot an arrow!");
    }
}
