using UnityEngine;
using UnityEngine.UI;
using FiveElements.Shared;

namespace FiveElements.Unity
{
    public class ResourceObject : MonoBehaviour
    {
        [Header("Resource Settings")]
        public ElementType ElementType;
        public int Amount;
        public float CollectTime = 2f;
        
        [Header("Visual")]
        public SpriteRenderer SpriteRenderer;
        public ProgressBar ProgressBar;
        
        private bool _collected = false;
        private bool _collecting = false;
        private float _progress = 0f;
        
        public void Initialize(ElementType elementType, int amount)
        {
            ElementType = elementType;
            Amount = amount;
            
            // 根据元素类型设置颜色
            if (SpriteRenderer != null)
            {
                SpriteRenderer.color = GetElementColor(elementType);
            }
            
            // 根据世界距离调整采集时间和数量
            if (OfflineGameManager.Instance != null)
            {
                CollectTime = 2f + OfflineGameManager.Instance.WorldDistance * 0.5f;
                Amount = Mathf.FloorToInt(5 + OfflineGameManager.Instance.WorldDistance * 2);
            }
        }
        
        public bool IsCollected() => _collected;
        
        public void UpdateProgress(float progress)
        {
            if (_collected) return;
            
            _collecting = true;
            _progress = progress;
            
            if (ProgressBar != null)
            {
                ProgressBar.SetProgress(_progress);
            }
        }
        
        public void ResetProgress()
        {
            _collecting = false;
            _progress = 0f;
            
            if (ProgressBar != null)
            {
                ProgressBar.SetProgress(0f);
            }
        }
        
        private Color GetElementColor(ElementType element)
        {
            switch (element)
            {
                case ElementType.Metal: return Color.gray;
                case ElementType.Wood: return Color.green;
                case ElementType.Water: return Color.blue;
                case ElementType.Fire: return Color.red;
                case ElementType.Earth: return new Color(0.6f, 0.4f, 0.2f); // 棕色
                default: return Color.white;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 可以在这里添加自动采集逻辑
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            // 玩家离开采集范围时重置进度
            if (other.CompareTag("Player"))
            {
                ResetProgress();
            }
        }
    }
}