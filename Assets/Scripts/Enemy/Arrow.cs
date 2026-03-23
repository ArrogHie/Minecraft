using UnityEngine;

/// <summary>
/// 箭矢：骷髅发射的投射物，碰撞玩家时造成伤害
/// </summary>
public class Arrow : MonoBehaviour
{
    public int damage = 3;
    public Enemy shooter;
    public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// 碰撞检测：命中玩家时造成伤害，然后销毁箭矢
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            if (player != null && shooter != null)
            {
                if (shooter.state != EnemyState.Dead)
                {
                    player.TakeDamage(damage);
                    Debug.Log($"Arrow hit player for {damage} damage!");
                }
            }
        }

        Destroy(gameObject);
    }
}
