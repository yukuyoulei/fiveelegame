using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FiveElements.Unity.Managers;
using FiveElements.Shared.Models;

namespace FiveElements.Unity.Managers
{
    [System.Serializable]
    public class PlayerSkill
    {
        public int Level;
        public string Direction; // "mind" 或 "body"
        
        public PlayerSkill()
        {
            Level = 0;
            Direction = null;
        }
    }
    
    public class SkillManager : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject SkillPanel;
        public GameObject BreakthroughPanel;
        
        [Header("Skill UI")]
        public Text SkillInfoText;
        public Text BreakthroughLevelText;
        public Text BreakthroughCostText;
        public Text BreakthroughChanceText;
        public Slider BreakthroughProgressSlider;
        
        [Header("Buttons")]
        public Button UpgradeMindButton;
        public Button UpgradeBodyButton;
        public Button StartBreakthroughButton;
        public Button StopBreakthroughButton;
        public Button CloseSkillPanelButton;
        public Button CloseBreakthroughPanelButton;
        
        private PlayerSkill _mindSkill = new PlayerSkill();
        private PlayerSkill _bodySkill = new PlayerSkill();
        private string _currentBreakthroughType;
        private bool _breakthroughInProgress = false;
        
        private void Start()
        {
            SetupUIEvents();
        }
        
        private void SetupUIEvents()
        {
            if (UpgradeMindButton != null)
                UpgradeMindButton.onClick.AddListener(() => UpgradeSkill("mind"));
            
            if (UpgradeBodyButton != null)
                UpgradeBodyButton.onClick.AddListener(() => UpgradeSkill("body"));
            
            if (StartBreakthroughButton != null)
                StartBreakthroughButton.onClick.AddListener(StartBreakthrough);
            
            if (StopBreakthroughButton != null)
                StopBreakthroughButton.onClick.AddListener(StopBreakthrough);
            
            if (CloseSkillPanelButton != null)
                CloseSkillPanelButton.onClick.AddListener(CloseSkillPanel);
            
            if (CloseBreakthroughPanelButton != null)
                CloseBreakthroughPanelButton.onClick.AddListener(CloseBreakthroughPanel);
        }
        
        public void ShowSkillPanel()
        {
            if (SkillPanel != null)
            {
                SkillPanel.SetActive(true);
                UpdateSkillInfo();
            }
        }
        
        public void CloseSkillPanel()
        {
            if (SkillPanel != null)
            {
                SkillPanel.SetActive(false);
            }
        }
        
        private void UpdateSkillInfo()
        {
            if (SkillInfoText != null)
            {
                string info = $"心法等级: {_mindSkill.Level}\n";
                info += $"外功等级: {_bodySkill.Level}\n";
                info += $"总元素: {GetTotalElements()}";
                
                SkillInfoText.text = info;
            }
        }
        
        private int GetTotalElements()
        {
            var elements = OfflineGameManager.Instance.PlayerStats.Elements;
            return elements.MetalValue + elements.WoodValue + elements.WaterValue + 
                   elements.FireValue + elements.EarthValue;
        }
        
        public void UpgradeSkill(string type)
        {
            PlayerSkill skill = type == "mind" ? _mindSkill : _bodySkill;
            
            if (string.IsNullOrEmpty(skill.Direction))
            {
                // 选择方向
                skill.Direction = type;
                ShowFloatingText($"选择了{type == "mind" ? "心法" : "外功"}方向");
                return;
            }
            
            // 检查是否需要突破
            if (skill.Level > 0 && skill.Level % 5 == 0)
            {
                ShowBreakthroughPanel(type);
                return;
            }
            
            // 消耗元素升级
            int cost = (skill.Level + 1) * 5;
            if (GetTotalElements() >= cost)
            {
                DeductElements(cost);
                skill.Level++;
                ShowFloatingText($"{(type == "mind" ? "心法" : "外功")}升级到{skill.Level}级");
                UpdateSkillInfo();
            }
            else
            {
                ShowFloatingText("元素不足");
            }
        }
        
        private void ShowBreakthroughPanel(string type)
        {
            if (BreakthroughPanel != null)
            {
                PlayerSkill skill = type == "mind" ? _mindSkill : _bodySkill;
                
                if (BreakthroughLevelText != null)
                    BreakthroughLevelText.text = skill.Level.ToString();
                
                if (BreakthroughCostText != null)
                    BreakthroughCostText.text = (skill.Level * 3).ToString();
                
                if (BreakthroughChanceText != null)
                    BreakthroughChanceText.text = Mathf.Min(50 + skill.Level * 5, 90).ToString();
                
                BreakthroughPanel.SetActive(true);
                _currentBreakthroughType = type;
            }
        }
        
        public void CloseBreakthroughPanel()
        {
            if (BreakthroughPanel != null)
            {
                BreakthroughPanel.SetActive(false);
            }
        }
        
        public void StartBreakthrough()
        {
            if (_breakthroughInProgress) return;
            
            _breakthroughInProgress = true;
            StartCoroutine(BreakthroughCoroutine());
        }
        
        public void StopBreakthrough()
        {
            _breakthroughInProgress = false;
        }
        
        private System.Collections.IEnumerator BreakthroughCoroutine()
        {
            PlayerSkill skill = _currentBreakthroughType == "mind" ? _mindSkill : _bodySkill;
            int cost = skill.Level * 3;
            float progress = 0f;
            
            while (_breakthroughInProgress && progress < 1f)
            {
                if (GetTotalElements() >= cost)
                {
                    DeductElements(cost);
                    progress += 0.01f;
                    
                    if (BreakthroughProgressSlider != null)
                    {
                        BreakthroughProgressSlider.value = progress;
                    }
                }
                else
                {
                    ShowFloatingText("元素不足，突破中断");
                    _breakthroughInProgress = false;
                    yield break;
                }
                
                yield return new WaitForSeconds(0.1f);
            }
            
            if (progress >= 1f)
            {
                // 突破成功判定
                int chance = Mathf.Min(50 + skill.Level * 5, 90);
                if (Random.Range(0, 100) < chance)
                {
                    skill.Level++;
                    ShowFloatingText($"突破成功！{(skill.Level - 1)}级 -> {skill.Level}级");
                    UpdateSkillInfo();
                    CloseBreakthroughPanel();
                }
                else
                {
                    ShowFloatingText("突破失败");
                }
            }
            
            _breakthroughInProgress = false;
            
            if (BreakthroughProgressSlider != null)
            {
                BreakthroughProgressSlider.value = 0f;
            }
        }
        
        private void DeductElements(int amount)
        {
            var elements = OfflineGameManager.Instance.PlayerStats.Elements;
            
            // 按顺序扣除元素
            while (amount > 0)
            {
                if (elements.MetalValue > 0)
                {
                    int deduct = Mathf.Min(amount, elements.MetalValue);
                    elements.MetalValue -= deduct;
                    amount -= deduct;
                }
                else if (elements.WoodValue > 0)
                {
                    int deduct = Mathf.Min(amount, elements.WoodValue);
                    elements.WoodValue -= deduct;
                    amount -= deduct;
                }
                else if (elements.WaterValue > 0)
                {
                    int deduct = Mathf.Min(amount, elements.WaterValue);
                    elements.WaterValue -= deduct;
                    amount -= deduct;
                }
                else if (elements.FireValue > 0)
                {
                    int deduct = Mathf.Min(amount, elements.FireValue);
                    elements.FireValue -= deduct;
                    amount -= deduct;
                }
                else if (elements.EarthValue > 0)
                {
                    int deduct = Mathf.Min(amount, elements.EarthValue);
                    elements.EarthValue -= deduct;
                    amount -= deduct;
                }
                else
                {
                    break; // 没有足够的元素
                }
            }
        }
        
        private void ShowFloatingText(string text)
        {
            Debug.Log($"Floating Text: {text}");
            // 这里可以实例化一个漂浮文字的预制体
        }
        
        public int GetMindLevel() => _mindSkill.Level;
        public int GetBodyLevel() => _bodySkill.Level;
    }
}