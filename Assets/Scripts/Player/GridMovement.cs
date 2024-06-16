using System;
using Constants;
using Manager;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// グリッド移動
    /// </summary>
    public class GridMovement
        : MonoBehaviour
    {
        public float moveSpeed = 5.0f;
        public float gridSize = 1.0f;
        private Vector3 _targetPosition;
        
        /// <summary>
        /// プレイヤーの状態
        /// </summary>
        private PlayerState _currentState = PlayerState.Idle;

        /// <summary>
        /// 初期化
        /// </summary>
        private void Start()
        {
            // 初期位置をグリッドに合わせる
            _targetPosition = transform.position;
        }

        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {
            // ゲームがポーズ中は何もしない
            if (GameManager.Instance == null 
                || !GameManager.Instance.IsPlaying)
            {
                return;
            }
            
            switch (_currentState)
            {
                case PlayerState.Idle:
                {
                    HandleIdleState();
                    break;
                }
                case PlayerState.Moving:
                {
                    HandleMovingState();
                    break;
                }
                case PlayerState.Dead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 移動状態の更新
        /// </summary>
        private void HandleMovingState()
        {
            // 目標地点へスムーズに移動
            transform.position = Vector3.MoveTowards(
                transform.position, 
                _targetPosition,
                moveSpeed * Time.deltaTime);
            
            // 目標地点に到達したら待機状態に遷移
            if (transform.position == _targetPosition)
            {
                transform.position = SnapToGrid(_targetPosition);
                _currentState = PlayerState.Idle;
                
                // ゴール判定
                if (IsGoalReached())
                {
                    GameManager.Instance.OnGoalReached();
                }
            }
        }

        /// <summary>
        /// ゴールに到達したか
        /// </summary>
        /// <returns></returns>
        private bool IsGoalReached()
        {
            return transform.position == GameManager.Instance.GetGoalPosition();
        }
        
        /// <summary>
        /// 敵との衝突判定
        /// </summary>
        /// <returns></returns>
        private bool IsEnemyCollision()
        {
            var prevPosition = SnapToGrid(transform.position);
            return GridManager.Instance.IsEnemy((int)prevPosition.x, (int)prevPosition.y);
        }

        /// <summary>
        /// 敵との衝突処理
        /// </summary>
        private void HandleEnemyCollision()
        {
            UnityEngine.Debug.Log("敵と衝突しました");
        }

        /// <summary>
        /// グリッドにスナップ
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private Vector3 SnapToGrid(Vector3 targetPosition)
        {
            return new Vector3(
                Mathf.RoundToInt(targetPosition.x / gridSize) * gridSize,
                Mathf.RoundToInt(targetPosition.y / gridSize) * gridSize,
                targetPosition.z
                );
        }

        /// <summary>
        /// 待機状態の更新
        /// </summary>
        private void HandleIdleState()
        {
            // ユーザー入力に基づいて目標地点を設定
            // 1行動ずつ取りたいのでGetAxisRawを使う
            var moveX = Input.GetAxisRaw("Horizontal");
            var moveY = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(moveX, moveY, 0);
            
            // 入力があれば
            if (input != Vector3.zero)
            {
                // 移動方向をグリッドに合わせる
                SetTargetPosition(input);
            }
        }

        /// <summary>
        /// 目標地点を設定
        /// </summary>
        /// <param name="input"></param>
        private void SetTargetPosition(Vector3 input)
        {
            // 移動方向をグリッドに合わせる
            Vector3 direction = input.normalized;
            
            // 移動先が壁でないかチェック
            Vector3 proposedPosition = transform.position + direction * gridSize;
            Vector2Int nextPosition = new Vector2Int(
                Mathf.RoundToInt(proposedPosition.x),
                Mathf.RoundToInt(proposedPosition.y));
            
            // 壁か敵でなければ移動
            if (!GridManager.Instance.IsOccupied(nextPosition.x, nextPosition.y))
            {
                _targetPosition = proposedPosition;
                // 移動状態に遷移
                _currentState = PlayerState.Moving;
            }
        }
    }
}