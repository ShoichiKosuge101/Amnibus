using UnityEngine;

namespace Debug
{
    /// <summary>
    /// スプライトサイズチェッカー
    /// </summary>
    public class DebugSpriteSize
        : MonoBehaviour
    {
        private void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                return;
            }
            
            Sprite sprite = spriteRenderer.sprite;
            if (sprite == null)
            {
                return;
            }
            
            UnityEngine.Debug.Log("Sprite Size: " + sprite.rect.size);
            UnityEngine.Debug.Log("Sprite size in units: " + sprite.bounds.size);
        }
    }
}