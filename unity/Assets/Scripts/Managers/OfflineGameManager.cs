using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using FiveElements.Shared;
using FiveElements.Shared.Models;

namespace FiveElements.Unity.Managers
{
    public class OfflineGameManager : MonoBehaviour
    {
        public static OfflineGameManager Instance { get; private set; }
        
        [Header("Game State")]
        public PlayerStats PlayerStats;
        public Position WorldPosition;
        public int WorldDistance = 0;
        
        [Header("Player Settings")]
        public float PlayerSpeed = 2f;
        public GameObject PlayerPrefab;
        
        [Header("Game Objects")]
        public GameObject ResourcePrefab;
        public GameObject MonsterPrefab;
        public GameObject NPCPrefab;
        
        [Header("UI References")]
        public UnityEngine.UI.Text CoordinatesText;
        public UnityEngine.UI.Text TaskDisplayText;
        public UnityEngine.UI.Text InventoryText;
        public UnityEngine.UI.Button SkillButton;
        
        private PlayerController _playerController;
        private TaskManager _taskManager;
        private SkillManager _skillManager;
        private GameObject _playerInstance;
        
        private List<ResourceObject> _resources = new List<ResourceObject>();
        private List<MonsterObject> _monsters = new List<MonsterObject>();
        private List<NPCObject> _npcs = new List<NPCObject>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            // 初始化玩家状态
            PlayerStats = new PlayerStats
            {
                Elements = new PlayerElementStats
                {
                    MetalValue = 10,
                    WoodValue = 10,
                    WaterValue = 10,
                    FireValue = 10,
                    EarthValue = 10
                }
            };
            
            WorldPosition = new Position(0, 0);
            
            // 初始化管理器
            _taskManager = GetComponent<TaskManager>() ?? gameObject.AddComponent<TaskManager>();
            _skillManager = GetComponent<SkillManager>() ?? gameObject.AddComponent<SkillManager>();
            
            // 创建玩家
            CreatePlayer();
            
            // 生成第一个任务
            _taskManager.GenerateTask();
            
            // 设置UI事件
            SetupUIEvents();
        }
        
        private void CreatePlayer()
        {
            if (PlayerPrefab != null)
            {
                _playerInstance = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
                _playerController = _playerInstance.GetComponent<PlayerController>();
                if (_playerController != null)
                {
                    _playerController.Initialize(this);
                }
            }
        }
        
        private void SetupUIEvents()
        {
            if (SkillButton != null)
            {
                SkillButton.onClick.AddListener(() => _skillManager.ShowSkillPanel());
            }
        }
        
        private void Update()
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            // 更新坐标显示
            if (CoordinatesText != null)
            {
                CoordinatesText.text = $"坐标: ({WorldPosition.X}, {WorldPosition.Y})";
            }
            
            // 更新任务显示
            if (TaskDisplayText != null && _taskManager != null)
            {
                var currentTask = _taskManager.CurrentTask;
                if (currentTask != null)
                {
                    var status = currentTask.Completed ? "✓" : "○";
                    TaskDisplayText.text = $"{status} {currentTask.Description}";
                    
                    if (!currentTask.Completed)
                    {
                        TaskDisplayText.text += "\n完成场景任务后才能离开";
                    }
                }
            }
            
            // 更新背包显示
            if (InventoryText != null)
            {
                InventoryText.text = GetInventoryText();
            }
        }
        
        private string GetInventoryText()
        {
            string text = "背包: ";
            var elements = PlayerStats.Elements;
            
            if (elements.MetalValue > 0) text += $"金:{elements.MetalValue} ";
            if (elements.WoodValue > 0) text += $"木:{elements.WoodValue} ";
            if (elements.WaterValue > 0) text += $"水:{elements.WaterValue} ";
            if (elements.FireValue > 0) text += $"火:{elements.FireValue} ";
            if (elements.EarthValue > 0) text += $"土:{elements.EarthValue} ";
            
            if (text == "背包: ")
            {
                text = "背包: 空";
            }
            
            return text;
        }
        
        public void SpawnResource(ElementType elementType, int amount)
        {
            if (ResourcePrefab == null) return;
            
            Vector3 position = GetRandomSpawnPosition();
            GameObject resourceObj = Instantiate(ResourcePrefab, position, Quaternion.identity);
            ResourceObject resource = resourceObj.GetComponent<ResourceObject>();
            
            if (resource != null)
            {
                resource.Initialize(elementType, amount);
                _resources.Add(resource);
            }
        }
        
        public void SpawnMonster(int health, int damage, float speed)
        {
            if (MonsterPrefab == null) return;
            
            Vector3 position = GetRandomSpawnPosition();
            GameObject monsterObj = Instantiate(MonsterPrefab, position, Quaternion.identity);
            MonsterObject monster = monsterObj.GetComponent<MonsterObject>();
            
            if (monster != null)
            {
                monster.Initialize(health, damage, speed);
                _monsters.Add(monster);
            }
        }
        
        public void SpawnNPC(string message)
        {
            if (NPCPrefab == null) return;
            
            Vector3 position = GetRandomSpawnPosition();
            GameObject npcObj = Instantiate(NPCPrefab, position, Quaternion.identity);
            NPCObject npc = npcObj.GetComponent<NPCObject>();
            
            if (npc != null)
            {
                npc.Initialize(message);
                _npcs.Add(npc);
            }
        }
        
        private Vector3 GetRandomSpawnPosition()
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-3f, 3f);
            return new Vector3(x, y, 0);
        }
        
        public void ChangeScene(int directionX, int directionY)
        {
            // 更新世界坐标
            WorldPosition.X += directionX;
            WorldPosition.Y += directionY;
            WorldDistance = Mathf.Max(Mathf.Abs(WorldPosition.X), Mathf.Abs(WorldPosition.Y));
            
            // 通知场景管理器场景已改变
            var sceneManager = FindObjectOfType<GameSceneManager>();
            if (sceneManager != null)
            {
                sceneManager.OnSceneChanged();
            }
            
            // 清除当前场景对象
            ClearScene();
            
            // 重置玩家位置
            if (_playerInstance != null)
            {
                _playerInstance.transform.position = Vector3.zero;
            }
            
            // 生成新任务
            _taskManager.GenerateTask();
        }
        
        private void ClearScene()
        {
            // 清除资源
            foreach (var resource in _resources)
            {
                if (resource != null && resource.gameObject != null)
                {
                    Destroy(resource.gameObject);
                }
            }
            _resources.Clear();
            
            // 清除怪物
            foreach (var monster in _monsters)
            {
                if (monster != null && monster.gameObject != null)
                {
                    Destroy(monster.gameObject);
                }
            }
            _monsters.Clear();
            
            // 清除NPC
            foreach (var npc in _npcs)
            {
                if (npc != null && npc.gameObject != null)
                {
                    Destroy(npc.gameObject);
                }
            }
            _npcs.Clear();
        }
        
        public void CollectResource(ResourceObject resource)
        {
            // 增加元素
            PlayerStats.Elements.AddValue(resource.ElementType, resource.Amount);
            
            // 完成任务
            if (_taskManager.CurrentTask != null && _taskManager.CurrentTask.Type == TaskType.Collect)
            {
                _taskManager.CompleteTask();
                ShowFloatingText($"任务完成！+{resource.Amount} {GetElementName(resource.ElementType)}", resource.transform.position);
            }
            else
            {
                ShowFloatingText($"+{resource.Amount} {GetElementName(resource.ElementType)}", resource.transform.position);
            }
            
            // 移除资源
            _resources.Remove(resource);
            Destroy(resource.gameObject);
        }
        
        public void KillMonster(MonsterObject monster)
        {
            // 随机获得元素
            ElementType[] elements = { ElementType.Metal, ElementType.Wood, ElementType.Water, ElementType.Fire, ElementType.Earth };
            ElementType randomElement = elements[Random.Range(0, elements.Length)];
            int amount = Mathf.FloorToInt(3 + WorldDistance * 1.5f);
            
            PlayerStats.Elements.AddValue(randomElement, amount);
            
            // 完成任务
            if (_taskManager.CurrentTask != null && _taskManager.CurrentTask.Type == TaskType.Kill)
            {
                _taskManager.CompleteTask();
                ShowFloatingText($"任务完成！+{amount} {GetElementName(randomElement)}", monster.transform.position);
            }
            else
            {
                ShowFloatingText($"+{amount} {GetElementName(randomElement)}", monster.transform.position);
            }
            
            // 移除怪物
            _monsters.Remove(monster);
            Destroy(monster.gameObject);
        }
        
        public void TalkToNPC(NPCObject npc)
        {
            if (npc.Talked) return;
            
            npc.Talk();
            
            // 完成任务
            if (_taskManager.CurrentTask != null && _taskManager.CurrentTask.Type == TaskType.Talk)
            {
                _taskManager.CompleteTask();
            }
        }
        
        private void ShowFloatingText(string text, Vector3 position)
        {
            // 这里可以实例化一个漂浮文字的预制体
            Debug.Log($"Floating Text: {text} at {position}");
        }
        
        private string GetElementName(ElementType element)
        {
            switch (element)
            {
                case ElementType.Metal: return "金";
                case ElementType.Wood: return "木";
                case ElementType.Water: return "水";
                case ElementType.Fire: return "火";
                case ElementType.Earth: return "土";
                default: return "未知";
            }
        }
        
        public bool CanChangeScene()
        {
            return _taskManager.CurrentTask != null && _taskManager.CurrentTask.Completed;
        }
    }
}