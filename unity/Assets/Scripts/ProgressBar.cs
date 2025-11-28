using UnityEngine;
using UnityEngine.UI;

namespace FiveElements.Unity
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("Progress Bar")]
        public Image FillImage;
        public Image BackgroundImage;
        
        [Header("Settings")]
        public bool ShowText = true;
        public Text ProgressText;
        public string Format = "0%";
        
        private float _currentProgress = 0f;
        private float _targetProgress = 0f;
        
        public void SetProgress(float progress, bool immediate = false)
        {
            progress = Mathf.Clamp01(progress);
            
            if (immediate)
            {
                _currentProgress = progress;
                _targetProgress = progress;
            }
            else
            {
                _targetProgress = progress;
            }
        }
        
        private void Update()
        {
            // 平滑过渡到目标进度
            if (Mathf.Abs(_currentProgress - _targetProgress) > 0.01f)
            {
                _currentProgress = Mathf.Lerp(_currentProgress, _targetProgress, Time.deltaTime * 5f);
                UpdateVisual();
            }
        }
        
        private void UpdateVisual()
        {
            if (FillImage != null)
            {
                FillImage.fillAmount = _currentProgress;
            }
            
            if (ShowText && ProgressText != null)
            {
                ProgressText.text = (_currentProgress * 100f).ToString(Format);
            }
        }
        
        public void SetColor(Color color)
        {
            if (FillImage != null)
            {
                FillImage.color = color;
            }
        }
        
        public void SetBackgroundColor(Color color)
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.color = color;
            }
        }
    }
}