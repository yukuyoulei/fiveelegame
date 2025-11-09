using UnityEngine;
using UnityEngine.UI;
using FiveElements.Shared.Models;

namespace FiveElements.Unity.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Stats Panel")]
        public Text StaminaText;
        public Text MetalText;
        public Text WoodText;
        public Text WaterText;
        public Text FireText;
        public Text EarthText;
        public Text PositionText;
        public Text MainElementText;

        [Header("Action Buttons")]
        public Button TrainBodyMetalButton;
        public Button TrainBodyWoodButton;
        public Button TrainBodyWaterButton;
        public Button TrainBodyFireButton;
        public Button TrainBodyEarthButton;
        
        public Button TrainMindMetalButton;
        public Button TrainMindWoodButton;
        public Button TrainMindWaterButton;
        public Button TrainMindFireButton;
        public Button TrainMindEarthButton;

        [Header("Movement Controls")]
        public Button MoveUpButton;
        public Button MoveDownButton;
        public Button MoveLeftButton;
        public Button MoveRightButton;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            // Training buttons
            TrainBodyMetalButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Metal));
            TrainBodyWoodButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Wood));
            TrainBodyWaterButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Water));
            TrainBodyFireButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Fire));
            TrainBodyEarthButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Earth));
            
            TrainMindMetalButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Metal));
            TrainMindWoodButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Wood));
            TrainMindWaterButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Water));
            TrainMindFireButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Fire));
            TrainMindEarthButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Earth));

            // Movement buttons
            MoveUpButton.onClick.AddListener(() => GameManager.Instance.OnMoveUpClicked());
            MoveDownButton.onClick.AddListener(() => GameManager.Instance.OnMoveDownClicked());
            MoveLeftButton.onClick.AddListener(() => GameManager.Instance.OnMoveLeftClicked());
            MoveRightButton.onClick.AddListener(() => GameManager.Instance.OnMoveRightClicked());
        }

        public void UpdateStats(PlayerStats playerStats)
        {
            if (StaminaText != null)
                StaminaText.text = $"体力: {playerStats.Stamina}/{playerStats.MaxStamina}";
            
            if (MetalText != null)
                MetalText.text = $"金: {playerStats.Elements.MetalValue}/{playerStats.Elements.MetalMax}";
            
            if (WoodText != null)
                WoodText.text = $"木: {playerStats.Elements.WoodValue}/{playerStats.Elements.WoodMax}";
            
            if (WaterText != null)
                WaterText.text = $"水: {playerStats.Elements.WaterValue}/{playerStats.Elements.WaterMax}";
            
            if (FireText != null)
                FireText.text = $"火: {playerStats.Elements.FireValue}/{playerStats.Elements.FireMax}";
            
            if (EarthText != null)
                EarthText.text = $"土: {playerStats.Elements.EarthValue}/{playerStats.Elements.EarthMax}";
            
            if (PositionText != null)
                PositionText.text = $"位置: ({playerStats.Position.X}, {playerStats.Position.Y})";
            
            if (MainElementText != null)
                MainElementText.text = $"主五行: {playerStats.Elements.MainElement}";
        }

        public void UpdateMovementButtons(bool canMove)
        {
            if (MoveUpButton != null) MoveUpButton.interactable = canMove;
            if (MoveDownButton != null) MoveDownButton.interactable = canMove;
            if (MoveLeftButton != null) MoveLeftButton.interactable = canMove;
            if (MoveRightButton != null) MoveRightButton.interactable = canMove;
        }
    }
}