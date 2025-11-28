using UnityEngine;
using UnityEngine.UI;

namespace FiveElements.Unity
{
    public class NPCObject : MonoBehaviour
    {
        [Header("NPC Settings")]
        public string NPCName = "NPC";
        [TextArea(3, 5)]
        public string DefaultMessage = "你好，勇敢的冒险者！";
        
        [Header("Visual")]
        public SpriteRenderer SpriteRenderer;
        public GameObject DialogueIndicator;
        public GameObject DialoguePanel;
        public Text DialogueText;
        public Button CloseDialogueButton;
        
        private bool _talked = false;
        private string _currentMessage;
        
        public bool Talked => _talked;
        
        public void Initialize(string message)
        {
            _currentMessage = message;
            
            // 设置NPC外观
            if (SpriteRenderer != null)
            {
                SpriteRenderer.color = Color.cyan; // NPC用青色区分
            }
            
            // 隐藏对话指示器
            if (DialogueIndicator != null)
            {
                DialogueIndicator.SetActive(false);
            }
            
            // 隐藏对话面板
            if (DialoguePanel != null)
            {
                DialoguePanel.SetActive(false);
            }
        }
        
        public void Talk()
        {
            if (_talked) return;
            
            _talked = true;
            ShowDialogue(_currentMessage);
            
            // 隐藏对话指示器
            if (DialogueIndicator != null)
            {
                DialogueIndicator.SetActive(false);
            }
            
            // 播放对话动画
            PlayTalkAnimation();
        }
        
        private void ShowDialogue(string message)
        {
            if (DialoguePanel != null && DialogueText != null)
            {
                DialogueText.text = message;
                DialoguePanel.SetActive(true);
                
                // 设置关闭按钮事件
                if (CloseDialogueButton != null)
                {
                    CloseDialogueButton.onClick.RemoveAllListeners();
                    CloseDialogueButton.onClick.AddListener(CloseDialogue);
                }
            }
        }
        
        public void CloseDialogue()
        {
            if (DialoguePanel != null)
            {
                DialoguePanel.SetActive(false);
            }
        }
        
        private void PlayTalkAnimation()
        {
            if (SpriteRenderer != null)
            {
                // 简单的缩放动画
                StartCoroutine(TalkAnimationCoroutine());
            }
        }
        
        private System.Collections.IEnumerator TalkAnimationCoroutine()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;
            
            // 放大
            float duration = 0.2f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }
            
            // 缩小
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_talked)
            {
                // 显示对话指示器
                if (DialogueIndicator != null)
                {
                    DialogueIndicator.SetActive(true);
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // 隐藏对话指示器
                if (DialogueIndicator != null)
                {
                    DialogueIndicator.SetActive(false);
                }
            }
        }
        
        private void Update()
        {
            // 检查玩家输入进行对话
            var player = FindObjectOfType<PlayerController>();
            if (player != null && Vector2.Distance(transform.position, player.transform.position) < 1.5f)
            {
                if (Input.GetKeyDown(KeyCode.F) && !_talked)
                {
                    Talk();
                }
            }
        }
    }
}