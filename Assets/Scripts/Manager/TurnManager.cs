using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// ターン管理
    /// </summary>
    public class TurnManager
        : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }
        
        private readonly Queue<IEnumerator> _turnQueue = new Queue<IEnumerator>();
        private bool _isProcessing = false;
        
        public bool IsPlayerTurn => _turnQueue.Count == 0 && !_isProcessing;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Awake()
        {
            // シングルトン
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            if (!_isProcessing && _turnQueue.Count > 0)
            {
                StartCoroutine(ProcessTurn());
            }
        }
        
        /// <summary>
        /// ターン追加
        /// </summary>
        /// <param name="turn"></param>
        public void AddTurn(IEnumerator turn)
        {
            _turnQueue.Enqueue(turn);
        }

        /// <summary>
        /// ターン処理
        /// </summary>
        /// <returns></returns>
        private IEnumerator ProcessTurn()
        {
            _isProcessing = true;
            
            while (_turnQueue.Count > 0)
            {
                var currentTurn = _turnQueue.Dequeue();
                yield return StartCoroutine(currentTurn);
            }
            
            _isProcessing = false;
        }

        /// <summary>
        /// プレイヤーターン終了
        /// </summary>
        public void EndPlayerTurn()
        {
            AddTurn(ProcessEnemyTurn());
        }

        /// <summary>
        /// 敵ターン処理
        /// </summary>
        /// <returns></returns>
        private IEnumerator ProcessEnemyTurn()
        {
            // 敵のターン処理
            foreach (var enemy in FindObjectsOfType<Enemy.EnemyBase>())
            {
                enemy.TakeTurn();
            }
            
            // 敵が移動中の場合
            while (AnyEnemyMoving())
            {
                // 敵が移動中の場合は待機
                yield return null;
            }
        }
        
        /// <summary>
        /// 敵が移動中か
        /// </summary>
        /// <returns></returns>
        private bool AnyEnemyMoving()
        {
            return FindObjectsOfType<Enemy.EnemyBase>().Any(enemy => enemy.IsMoving);
        }
    }
}