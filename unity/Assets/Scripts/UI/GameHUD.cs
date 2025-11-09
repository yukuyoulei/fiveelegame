using UnityEngine;
using UnityEngine.UI;
using FiveElements.Shared;

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
            if (TrainBodyMetalButton != null)
                TrainBodyMetalButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Metal));
            if (TrainBodyWoodButton != null)
                TrainBodyWoodButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Wood));
            if (TrainBodyWaterButton != null)
                TrainBodyWaterButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Water));
            if (TrainBodyFireButton != null)
                TrainBodyFireButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Fire));
            if (TrainBodyEarthButton != null)
                TrainBodyEarthButton.onClick.AddListener(() => GameManager.Instance.OnTrainBodyClicked(ElementType.Earth));
            
            if (TrainMindMetalButton != null)
                TrainMindMetalButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Metal));
            if (TrainMindWoodButton != null)
                TrainMindWoodButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Wood));
            if (TrainMindWaterButton != null)
                TrainMindWaterButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Water));
            if (TrainMindFireButton != null)
                TrainMindFireButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Fire));
            if (TrainMindEarthButton != null)
                TrainMindEarthButton.onClick.AddListener(() => GameManager.Instance.OnTrainMindClicked(ElementType.Earth));

            // Movement buttons
            if (MoveUpButton != null)
                MoveUpButton.onClick.AddListener(() => GameManager.Instance.OnMoveUpClicked());
            if (MoveDownButton != null)
                MoveDownButton.onClick.AddListener(() => GameManager.Instance.OnMoveDownClicked());
            if (MoveLeftButton != null)
                MoveLeftButton.onClick.AddListener(() => GameManager.Instance.OnMoveLeftClicked());
            if (MoveRightButton != null)
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