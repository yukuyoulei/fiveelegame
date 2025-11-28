using UnityEngine;
using UnityEngine.UI;
using FiveElements.Unity.Managers;

namespace FiveElements.Unity
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Managers")]
        public OfflineGameManager OfflineGameManager;
        public GameSceneManager SceneManager;
        public Camera MainCamera;
        
        [Header("UI Canvas")]
        public GameObject UICanvas;
        public Text CoordinatesText;
        public Text TaskDisplayText;
        public Text InventoryText;
        public Button SkillButton;
        
        [Header("Game HUD")]
        public Slider HealthBar;
        public Slider ResourceBar;
        
        [Header("Scene Layers")]
        public GameObject BackgroundLayer;
        public GameObject MiddleLayer;
        public GameObject ForegroundLayer;
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            // 初始化离线游戏管理器
            if (OfflineGameManager == null)
            {
                GameObject gameManagerObj = new GameObject("OfflineGameManager");
                OfflineGameManager = gameManagerObj.AddComponent<OfflineGameManager>();
            }
            
            // 设置离线游戏管理器的引用
            SetupOfflineGameManager();
            
            // 初始化场景管理器
            if (SceneManager == null)
            {
                GameObject sceneManagerObj = new GameObject("GameSceneManager");
                SceneManager = sceneManagerObj.AddComponent<GameSceneManager>();
            }
            
            // 设置场景管理器的引用
            SetupSceneManager();
            
            // 设置相机
            SetupCamera();
            
            // 设置UI
            SetupUI();
            
            // 创建游戏对象
            CreateGameObjects();
        }
        
        private void SetupOfflineGameManager()
        {
            // 设置UI引用
            OfflineGameManager.CoordinatesText = CoordinatesText;
            OfflineGameManager.TaskDisplayText = TaskDisplayText;
            OfflineGameManager.InventoryText = InventoryText;
            OfflineGameManager.SkillButton = SkillButton;
            
            // 设置预制体引用（这些将由PrefabGenerator生成）
            // OfflineGameManager.PlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");
            // OfflineGameManager.ResourcePrefab = Resources.Load<GameObject>("Prefabs/Resource");
            // OfflineGameManager.MonsterPrefab = Resources.Load<GameObject>("Prefabs/Monster");
            // OfflineGameManager.NPCPrefab = Resources.Load<GameObject>("Prefabs/NPC");
        }
        
        private void SetupSceneManager()
        {
            SceneManager.BackgroundLayer = BackgroundLayer;
            SceneManager.MiddleLayer = MiddleLayer;
            SceneManager.ForegroundLayer = ForegroundLayer;
        }
        
        private void SetupCamera()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
                if (MainCamera == null)
                {
                    GameObject cameraObj = new GameObject("MainCamera");
                    MainCamera = cameraObj.AddComponent<Camera>();
                    cameraObj.tag = "MainCamera";
                }
            }
            
            // 设置相机为正交投影
            MainCamera.orthographic = true;
            MainCamera.orthographicSize = 6f;
            MainCamera.backgroundColor = new Color(0.529f, 0.808f, 0.922f); // 天空蓝
        }
        
        private void SetupUI()
        {
            if (UICanvas == null)
            {
                UICanvas = FindObjectOfType<Canvas>()?.gameObject;
                if (UICanvas == null)
                {
                    UICanvas = CreateUICanvas();
                }
            }
            
            // 如果UI元素不存在，创建它们
            if (CoordinatesText == null)
            {
                CoordinatesText = CreateUIText("CoordinatesText", new Vector2(10, 10), "坐标: (0, 0)");
            }
            
            if (TaskDisplayText == null)
            {
                TaskDisplayText = CreateUIText("TaskDisplayText", new Vector2(10, 30), "暂无任务");
            }
            
            if (InventoryText == null)
            {
                InventoryText = CreateUIText("InventoryText", new Vector2(10, 50), "背包: 空");
            }
            
            if (SkillButton == null)
            {
                SkillButton = CreateUIButton("SkillButton", new Vector2(10, 70), "技能");
            }
        }
        
        private GameObject CreateUICanvas()
        {
            GameObject canvasObj = new GameObject("UICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            return canvasObj;
        }
        
        private Text CreateUIText(string name, Vector2 position, string text)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(UICanvas.transform);
            
            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(200, 20);
            
            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 12;
            textComponent.color = Color.white;
            
            // 添加背景
            GameObject background = new GameObject("Background");
            background.transform.SetParent(textObj.transform);
            background.transform.localPosition = Vector3.zero;
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.7f);
            
            RectTransform bgRect = bgImage.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgRect.transform.SetAsFirstSibling();
            
            return textComponent;
        }
        
        private Button CreateUIButton(string name, Vector2 position, string text)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(UICanvas.transform);
            
            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(80, 30);
            
            Image bgImage = buttonObj.AddComponent<Image>();
            bgImage.color = Color.green;
            
            Button button = buttonObj.AddComponent<Button>();
            
            // 添加文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 14;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            
            return button;
        }
        
        private void CreateGameObjects()
        {
            // 创建场景层
            if (BackgroundLayer == null)
            {
                BackgroundLayer = new GameObject("BackgroundLayer");
                BackgroundLayer.transform.position = new Vector3(0, 0, 10f);
            }
            
            if (MiddleLayer == null)
            {
                MiddleLayer = new GameObject("MiddleLayer");
                MiddleLayer.transform.position = new Vector3(0, 0, 5f);
            }
            
            if (ForegroundLayer == null)
            {
                ForegroundLayer = new GameObject("ForegroundLayer");
                ForegroundLayer.transform.position = new Vector3(0, 0, 1f);
            }
            
            // 创建边界
            CreateBoundaries();
        }
        
        private void CreateBoundaries()
        {
            // 创建场景边界
            GameObject boundaries = new GameObject("Boundaries");
            
            // 上边界
            CreateBoundary(boundaries, "TopBoundary", new Vector3(0, 5.5f, 0), new Vector2(15f, 0.2f));
            
            // 下边界
            CreateBoundary(boundaries, "BottomBoundary", new Vector3(0, -5.5f, 0), new Vector2(15f, 0.2f));
            
            // 左边界
            CreateBoundary(boundaries, "LeftBoundary", new Vector3(-7.5f, 0, 0), new Vector2(0.2f, 11f));
            
            // 右边界
            CreateBoundary(boundaries, "RightBoundary", new Vector3(7.5f, 0, 0), new Vector2(0.2f, 11f));
        }
        
        private void CreateBoundary(GameObject parent, string name, Vector3 position, Vector2 size)
        {
            GameObject boundary = new GameObject(name);
            boundary.transform.SetParent(parent.transform);
            boundary.transform.position = position;
            boundary.tag = "Boundary";
            
            BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
            collider.size = size;
            
            // 设置为不可见
            SpriteRenderer renderer = boundary.AddComponent<SpriteRenderer>();
            renderer.enabled = false;
        }
    }
}