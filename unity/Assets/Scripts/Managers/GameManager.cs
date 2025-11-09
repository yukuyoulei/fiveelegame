using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using FiveElements.Shared;
using FiveElements.Shared.Messages;

namespace FiveElements.Unity.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game State")]
        public PlayerStats PlayerStats;
        public MapView CurrentMapView;
        public List<PlayerInfo> NearbyPlayers = new List<PlayerInfo>();
        
        [Header("UI References")]
        public GameObject MainElementSelectionPanel;
        public GameObject GameHUD;
        public Text StaminaText;
        public Text MetalText;
        public Text WoodText;
        public Text WaterText;
        public Text FireText;
        public Text EarthText;
        public Button MoveUpButton;
        public Button MoveDownButton;
        public Button MoveLeftButton;
        public Button MoveRightButton;
        
        private NetworkManager _networkManager;
        private UIManager _uiManager;
        private MapManager _mapManager;
        
        private bool _isWaitingForMainElementSelection = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                InitializeManagers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeManagers()
        {
            _networkManager = GetComponent<NetworkManager>();
            _uiManager = GetComponent<UIManager>();
            _mapManager = GetComponent<MapManager>();
            
            if (_networkManager == null)
                _networkManager = gameObject.AddComponent<NetworkManager>();
            if (_uiManager == null)
                _uiManager = gameObject.AddComponent<UIManager>();
            if (_mapManager == null)
                _mapManager = gameObject.AddComponent<MapManager>();
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            PlayerStats = new PlayerStats();
            CurrentMapView = new MapView(new Position(0, 0));
            
            ShowMainElementSelection();
            
            // Start network connection
            StartCoroutine(_networkManager.ConnectToServer());
        }

        private void ShowMainElementSelection()
        {
            if (MainElementSelectionPanel != null)
                MainElementSelectionPanel.SetActive(true);
            
            if (GameHUD != null)
                GameHUD.SetActive(false);
        }

        private void ShowGameHUD()
        {
            if (MainElementSelectionPanel != null)
                MainElementSelectionPanel.SetActive(false);
            
            if (GameHUD != null)
                GameHUD.SetActive(true);
        }

        public void SelectMainElement(ElementType element, string playerName)
        {
            var message = new PlayerSelectMainElementMessage
            {
                MainElement = element,
                PlayerName = playerName
            };
            
            _networkManager.SendMessage(message);
        }

        public void OnWelcomeMessageReceived(WelcomeMessage message)
        {
            PlayerStats = message.PlayerStats;
            
            if (!message.NeedsMainElementSelection)
            {
                _isWaitingForMainElementSelection = false;
                ShowGameHUD();
                UpdateUI();
            }
        }

        public void OnMapUpdateReceived(MapUpdateMessage message)
        {
            CurrentMapView = message.MapView;
            NearbyPlayers = message.MapView.NearbyPlayers;
            
            _mapManager.UpdateMap(message.MapView);
            UpdateUI();
        }

        public void OnPlayerStatsUpdated(PlayerStatsUpdateMessage message)
        {
            PlayerStats = message.PlayerStats;
            UpdateUI();
        }

        public void OnTrainingResultReceived(TrainingResultMessage message)
        {
            _uiManager.ShowMessage(message.Result);
        }

        public void OnHarvestResultReceived(HarvestResultMessage message)
        {
            _uiManager.ShowMessage(message.Message);
        }

        public void OnAttackResultReceived(AttackResultMessage message)
        {
            _uiManager.ShowMessage(message.Message);
        }

        public void OnPlayerJoined(PlayerJoinedMessage message)
        {
            _uiManager.ShowMessage($"{message.Player.Name} 加入了游戏");
            NearbyPlayers.Add(message.Player);
            UpdateUI();
        }

        public void OnPlayerLeft(PlayerLeftMessage message)
        {
            var player = NearbyPlayers.Find(p => p.PlayerId == message.PlayerId);
            if (player != null)
            {
                _uiManager.ShowMessage($"{player.Name} 离开了游戏");
                NearbyPlayers.Remove(player);
                UpdateUI();
            }
        }

        public void OnError(ErrorMessage message)
        {
            _uiManager.ShowMessage($"错误: {message.Message}");
        }

        private void UpdateUI()
        {
            if (StaminaText != null)
                StaminaText.text = $"体力: {PlayerStats.Stamina}/{PlayerStats.MaxStamina}";
            
            if (MetalText != null)
                MetalText.text = $"金: {PlayerStats.Elements.MetalValue}/{PlayerStats.Elements.MetalMax}";
            
            if (WoodText != null)
                WoodText.text = $"木: {PlayerStats.Elements.WoodValue}/{PlayerStats.Elements.WoodMax}";
            
            if (WaterText != null)
                WaterText.text = $"水: {PlayerStats.Elements.WaterValue}/{PlayerStats.Elements.WaterMax}";
            
            if (FireText != null)
                FireText.text = $"火: {PlayerStats.Elements.FireValue}/{PlayerStats.Elements.FireMax}";
            
            if (EarthText != null)
                EarthText.text = $"土: {PlayerStats.Elements.EarthValue}/{PlayerStats.Elements.EarthMax}";
        }

        // Button click handlers
        public void OnMoveUpClicked() => MovePlayer(0, 1);
        public void OnMoveDownClicked() => MovePlayer(0, -1);
        public void OnMoveLeftClicked() => MovePlayer(-1, 0);
        public void OnMoveRightClicked() => MovePlayer(1, 0);

        private void MovePlayer(int directionX, int directionY)
        {
            if (_isWaitingForMainElementSelection) return;
            
            var message = new PlayerMoveMessage
            {
                DirectionX = directionX,
                DirectionY = directionY
            };
            
            _networkManager.SendMessage(message);
        }

        public void OnHarvestClicked(Position targetPosition)
        {
            if (_isWaitingForMainElementSelection) return;
            
            var message = new PlayerHarvestMessage
            {
                TargetPosition = targetPosition
            };
            
            _networkManager.SendMessage(message);
        }

        public void OnAttackClicked(Position targetPosition, ElementType attackElement)
        {
            if (_isWaitingForMainElementSelection) return;
            
            var message = new PlayerAttackMessage
            {
                TargetPosition = targetPosition,
                AttackElement = attackElement
            };
            
            _networkManager.SendMessage(message);
        }

        public void OnTrainBodyClicked(ElementType targetElement)
        {
            if (_isWaitingForMainElementSelection) return;
            
            var message = new PlayerTrainBodyMessage
            {
                TargetElement = targetElement
            };
            
            _networkManager.SendMessage(message);
        }

        public void OnTrainMindClicked(ElementType targetElement)
        {
            if (_isWaitingForMainElementSelection) return;
            
            var message = new PlayerTrainMindMessage
            {
                TargetElement = targetElement
            };
            
            _networkManager.SendMessage(message);
        }
    }
}