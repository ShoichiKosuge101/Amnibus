using Enemy;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(fileName = "StageData", menuName = "StageData", order = 1)]
    public class StageData
        : ScriptableObject
    {
        /// <summary>
        /// ステージの大きさ
        /// </summary>
        public int width;
        public int height;

        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject goalPrefab;
        
        public EnemyData[] enemies;
    }
}