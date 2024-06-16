using Manager;
using UnityEngine;

namespace Player
{
    public class PlayerController
         : MonoBehaviour
    {
        public float moveSpeed = 5.0f;
        
        private Vector2Int _gridPosition;

        private void Start()
        {
            _gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        }

        private void Update()
        {
            var moveX = Input.GetAxis("Horizontal");
            var moveY = Input.GetAxis("Vertical");
            Move(new Vector2Int((int)moveX, (int)moveY));
        }
        
        private void Move(Vector2Int direction)
        {
            Vector2Int newPostion = _gridPosition + direction;
            UnityEngine.Debug.Log("Player Move: " + newPostion.x + ", " + newPostion.y);
            
            if(!GridManager.Instance.IsOccupied(newPostion.x, newPostion.y))
            {
                _gridPosition = newPostion;
                transform.position = new Vector3(_gridPosition.x, _gridPosition.y, 0);
                UnityEngine.Debug.Log("Player Moved to: " + _gridPosition.x + ", " + _gridPosition.y);
            }
            else if(GridManager.Instance.IsEnemy(newPostion.x, newPostion.y))
            {
                UnityEngine.Debug.Log("衝突しました");
            }
        }
    }
}