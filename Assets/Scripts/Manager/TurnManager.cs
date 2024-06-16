using System.Collections;
using System.Collections.Generic;
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
        
        private Queue<IEnumerator> turnQueue = new Queue<IEnumerator>();
        private bool _isProcessing = false;

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
            if (!_isProcessing && turnQueue.Count > 0)
            {
                StartCoroutine(ProcessTurn());
            }
        }

        /// <summary>
        /// ターン処理
        /// </summary>
        /// <returns></returns>
        private IEnumerator ProcessTurn()
        {
            _isProcessing = true;
            while (turnQueue.Count > 0)
            {
                var currentTurn = turnQueue.Dequeue();
                yield return StartCoroutine(currentTurn);
            }
            _isProcessing = false;
        }
        
        /// <summary>
        /// ターン追加
        /// </summary>
        /// <param name="turn"></param>
        public void AddTurn(IEnumerator turn)
        {
            turnQueue.Enqueue(turn);
        }
    }
}