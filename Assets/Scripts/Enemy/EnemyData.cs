using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵のデータ
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData", order = 1)]
    public class EnemyData
        : ScriptableObject
    {
        public string enemyName;
        public float health;
        public float damage;
        public float speed;
        public GameObject enemyPrefab;
    }
}