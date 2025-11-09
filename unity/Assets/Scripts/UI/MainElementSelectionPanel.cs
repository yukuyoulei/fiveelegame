using UnityEngine;
using UnityEngine.UI;
using FiveElements.Shared;

namespace FiveElements.Unity.UI
{
    public class MainElementSelectionPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public InputField PlayerNameInput;
        public Button MetalButton;
        public Button WoodButton;
        public Button WaterButton;
        public Button FireButton;
        public Button EarthButton;
        public Text DescriptionText;

        private void Start()
        {
            SetupButtons();
            ShowDescription(ElementType.Metal);
        }

        private void SetupButtons()
        {
            if (MetalButton != null)
            {
                MetalButton.onClick.AddListener(() => SelectElement(ElementType.Metal));
                MetalButton.onClick.AddListener(() => ShowDescription(ElementType.Metal));
            }
            if (WoodButton != null)
            {
                WoodButton.onClick.AddListener(() => SelectElement(ElementType.Wood));
                WoodButton.onClick.AddListener(() => ShowDescription(ElementType.Wood));
            }
            if (WaterButton != null)
            {
                WaterButton.onClick.AddListener(() => SelectElement(ElementType.Water));
                WaterButton.onClick.AddListener(() => ShowDescription(ElementType.Water));
            }
            if (FireButton != null)
            {
                FireButton.onClick.AddListener(() => SelectElement(ElementType.Fire));
                FireButton.onClick.AddListener(() => ShowDescription(ElementType.Fire));
            }
            if (EarthButton != null)
            {
                EarthButton.onClick.AddListener(() => SelectElement(ElementType.Earth));
                EarthButton.onClick.AddListener(() => ShowDescription(ElementType.Earth));
            }
        }

        private void SelectElement(ElementType element)
        {
            var playerName = (PlayerNameInput != null && !string.IsNullOrEmpty(PlayerNameInput.text)) ? PlayerNameInput.text : "Player";
            GameManager.Instance.SelectMainElement(element, playerName);
        }

        private void ShowDescription(ElementType element)
        {
            if (DescriptionText == null) return;
            
            var description = element switch
            {
                ElementType.Metal => "金：坚固、锋利、收敛。主攻防兼备，擅长炼体增强上限。",
                ElementType.Wood => "木：生长、柔韧、生机。主恢复与成长，擅长炼心积累能量。",
                ElementType.Water => "水：流动、适应、净化。主灵活变化，擅长移动与闪避。",
                ElementType.Fire => "火：炽热、爆发、毁灭。主强力攻击，擅长快速击败敌人。",
                ElementType.Earth => "土：稳定、承载、防御。主坚固防御，擅长持久作战。",
                _ => "选择你的主五行属性，这将影响你的游戏体验。"
            };
            
            DescriptionText.text = description;
        }
    }
}