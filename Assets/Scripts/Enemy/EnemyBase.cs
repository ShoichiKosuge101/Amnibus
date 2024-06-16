using System.Collections;
using Manager;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敵の基底クラス
    /// </summary>
    public class EnemyBase
        : MonoBehaviour
    {
        private float _health;
        private float _damage;
        private float _speed;
        
        private Vector2Int _gridPosition;
        private float _moveCooldown = 1.0f;
        private float _moveTimer = 0.0f;
        
        private bool _isMoving = false;
        public bool IsMoving => _isMoving;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="enemyData"></param>
        public void Initialize(EnemyData enemyData)
        {
            _health = enemyData.health;
            _damage = enemyData.damage;
            _speed = enemyData.speed;
        }

        /// <summary>
        /// 開始
        /// </summary>
        private void Start()
        {
            _gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            GridManager.Instance.PlaceEnemy(_gridPosition.x, _gridPosition.y);
        }

        /// <summary>
        /// ターン処理
        /// </summary>
        public void TakeTurn()
        {
            _isMoving = true;
            StartCoroutine(MoveRandomly());
        }

        /// <summary>
        /// 移動
        /// </summary>
        private IEnumerator MoveRandomly()
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            };
            
            Vector2Int newDirection = directions[UnityEngine.Random.Range(0, directions.Length)];
            Vector2Int newGridPosition = _gridPosition + newDirection;
            
            // 他の敵がいないか、プレイヤーの位置に移動しないかチェック
            if(!GridManager.Instance.IsOccupied(newGridPosition.x, newGridPosition.y) 
               && !GetPlayerPosition().Equals(newGridPosition))
            {
                GridManager.Instance.RemoveEnemy(_gridPosition.x, _gridPosition.y);
                _gridPosition = newGridPosition;
                
                yield return StartCoroutine(MoveToPosition(new Vector3(_gridPosition.x, _gridPosition.y, 0)));
                
                GridManager.Instance.PlaceEnemy(_gridPosition.x, _gridPosition.y);
            }
            
            _isMoving = false;
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            // 移動中
            while((transform.position - targetPosition).sqrMagnitude > float.Epsilon)
            {
                transform.position = Vector3
                    .MoveTowards(
                        transform.position, 
                        targetPosition, 
                        _speed * Time.deltaTime
                    );
                yield return null;
            }
            
            // 移動完了
            transform.position = targetPosition;
            _isMoving = false;
        }

        /// <summary>
        /// プレイヤーの位置を取得
        /// </summary>
        /// <returns></returns>
        private static Vector2Int GetPlayerPosition()
        {
            var player = GameManager.Instance.GetPlayerInstance();
            return new Vector2Int(
                Mathf.RoundToInt(player.transform.position.x),
                Mathf.RoundToInt(player.transform.position.y));
        }

        /// <summary>
        /// 破棄
        /// </summary>
        private void OnDestroy()
        {
            if (GridManager.Instance != null)
            {
                GridManager.Instance.RemoveEnemy(_gridPosition.x, _gridPosition.y);
            }
        }
    }
}