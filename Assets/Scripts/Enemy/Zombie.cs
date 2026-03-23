using UnityEngine;

/// <summary>
/// 僵尸：近战敌人，血量高、移动慢、攻击伤害高
/// </summary>
public class Zombie : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        enemyType = EnemyType.Zombie;
        maxHealth = 20;
        currentHealth = maxHealth;
        attackDamage = 5;
        speed = 3f;
        jumpForce = 6f;
        sightRange = 12f;
        attackRange = 1.5f;
        attackCooldown = 1.5f;
        maxDistance = 30f;
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

        PlayerControl player = target.GetComponent<PlayerControl>();
        if (player != null)
        {
            player.TakeDamage(attackDamage);
            Debug.Log($"Zombie attacked player for {attackDamage} damage!");
        }
    }
}
