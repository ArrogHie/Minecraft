using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人口管理器：单例模式，维护场景中所有敌人的列表
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 注册敌人到列表
    /// </summary>
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// 从列表注销敌人
    /// </summary>
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    /// <summary>
    /// 获取当前存活敌人数量
    /// </summary>
    public int GetEnemyCount()
    {
        return enemies.Count;
    }

    /// <summary>
    /// 获取所有敌人的副本列表
    /// </summary>
    public List<Enemy> GetAllEnemies()
    {
        return new List<Enemy>(enemies);
    }
}
