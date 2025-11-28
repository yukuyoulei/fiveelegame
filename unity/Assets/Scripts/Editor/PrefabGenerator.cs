using UnityEngine;
using UnityEditor;
using System.IO;
using FiveElements.Unity;
using FiveElements.Shared;

namespace FiveElements.Unity.Editor
{
    public class PrefabGenerator : EditorWindow
    {
        private string _prefabSavePath = "Assets/Prefabs/";
        private bool _createPlayerPrefab = true;
        private bool _createResourcePrefab = true;
        private bool _createMonsterPrefab = true;
        private bool _createNPCPrefab = true;
        private bool _createUIPrefabs = true;
        
        [MenuItem("Five Elements/Generate Prefabs")]
        public static void ShowWindow()
        {
            GetWindow<PrefabGenerator>("Generate Prefabs");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Five Elements Prefab Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            _prefabSavePath = EditorGUILayout.TextField("Save Path:", _prefabSavePath);
            
            GUILayout.Space(10);
            GUILayout.Label("Prefabs to Generate:", EditorStyles.boldLabel);
            
            _createPlayerPrefab = EditorGUILayout.Toggle("Player Prefab", _createPlayerPrefab);
            _createResourcePrefab = EditorGUILayout.Toggle("Resource Prefab", _createResourcePrefab);
            _createMonsterPrefab = EditorGUILayout.Toggle("Monster Prefab", _createMonsterPrefab);
            _createNPCPrefab = EditorGUILayout.Toggle("NPC Prefab", _createNPCPrefab);
            _createUIPrefabs = EditorGUILayout.Toggle("UI Prefabs", _createUIPrefabs);
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("Generate All Prefabs"))
            {
                GeneratePrefabs();
            }
        }
        
        private void GeneratePrefabs()
        {
            // 确保目录存在
            if (!Directory.Exists(_prefabSavePath))
            {
                Directory.CreateDirectory(_prefabSavePath);
            }
            
            if (_createPlayerPrefab)
            {
                CreatePlayerPrefab();
            }
            
            if (_createResourcePrefab)
            {
                CreateResourcePrefab();
            }
            
            if (_createMonsterPrefab)
            {
                CreateMonsterPrefab();
            }
            
            if (_createNPCPrefab)
            {
                CreateNPCPrefab();
            }
            
            if (_createUIPrefabs)
            {
                CreateUIPrefabs();
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "All prefabs generated successfully!", "OK");
        }
        
        private void CreatePlayerPrefab()
        {
            // 创建玩家游戏对象
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            
            // 添加组件
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            
            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 0.8f);
            
            SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite(Color.white);
            spriteRenderer.sortingOrder = 10;
            
            PlayerController playerController = player.AddComponent<PlayerController>();
            
            Animator animator = player.AddComponent<Animator>();
            animator.runtimeAnimatorController = CreatePlayerAnimatorController();
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "Player.prefab");
            PrefabUtility.SaveAsPrefabAsset(player, path);
            DestroyImmediate(player);
        }
        
        private void CreateResourcePrefab()
        {
            GameObject resource = new GameObject("Resource");
            
            // 添加组件
            SpriteRenderer spriteRenderer = resource.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite(Color.gray);
            spriteRenderer.sortingOrder = 5;
            
            BoxCollider2D collider = resource.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.6f, 0.6f);
            collider.isTrigger = true;
            
            ResourceObject resourceObject = resource.AddComponent<ResourceObject>();
            
            // 创建进度条
            GameObject progressBarObj = new GameObject("ProgressBar");
            progressBarObj.transform.SetParent(resource.transform);
            progressBarObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            
            ProgressBar progressBar = progressBarObj.AddComponent<ProgressBar>();
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "Resource.prefab");
            PrefabUtility.SaveAsPrefabAsset(resource, path);
            DestroyImmediate(resource);
        }
        
        private void CreateMonsterPrefab()
        {
            GameObject monster = new GameObject("Monster");
            
            // 添加组件
            SpriteRenderer spriteRenderer = monster.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite(Color.red);
            spriteRenderer.sortingOrder = 5;
            
            BoxCollider2D collider = monster.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 0.8f);
            
            MonsterObject monsterObject = monster.AddComponent<MonsterObject>();
            
            // 创建血条
            GameObject healthBarObj = new GameObject("HealthBar");
            healthBarObj.transform.SetParent(monster.transform);
            healthBarObj.transform.localPosition = new Vector3(0, 0.6f, 0);
            
            Slider healthBar = healthBarObj.AddComponent<Slider>();
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "Monster.prefab");
            PrefabUtility.SaveAsPrefabAsset(monster, path);
            DestroyImmediate(monster);
        }
        
        private void CreateNPCPrefab()
        {
            GameObject npc = new GameObject("NPC");
            
            // 添加组件
            SpriteRenderer spriteRenderer = npc.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite(Color.cyan);
            spriteRenderer.sortingOrder = 5;
            
            CircleCollider2D collider = npc.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;
            
            NPCObject npcObject = npc.AddComponent<NPCObject>();
            
            // 创建对话指示器
            GameObject indicatorObj = new GameObject("DialogueIndicator");
            indicatorObj.transform.SetParent(npc.transform);
            indicatorObj.transform.localPosition = new Vector3(0, 0.6f, 0);
            
            SpriteRenderer indicatorRenderer = indicatorObj.AddComponent<SpriteRenderer>();
            indicatorRenderer.sprite = CreateExclamationSprite();
            indicatorRenderer.sortingOrder = 15;
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "NPC.prefab");
            PrefabUtility.SaveAsPrefabAsset(npc, path);
            DestroyImmediate(npc);
        }
        
        private void CreateUIPrefabs()
        {
            // 创建技能面板
            CreateSkillPanelPrefab();
            
            // 创建突破面板
            CreateBreakthroughPanelPrefab();
            
            // 创建漂浮文字预制体
            CreateFloatingTextPrefab();
        }
        
        private void CreateSkillPanelPrefab()
        {
            GameObject panel = new GameObject("SkillPanel");
            Canvas canvas = panel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            panel.AddComponent<CanvasScaler>();
            panel.AddComponent<GraphicRaycaster>();
            
            // 背景面板
            GameObject bgPanel = new GameObject("Background");
            bgPanel.transform.SetParent(panel.transform);
            Image bgImage = bgPanel.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.9f);
            RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 内容面板
            GameObject contentPanel = new GameObject("Content");
            contentPanel.transform.SetParent(panel.transform);
            RectTransform contentRect = contentPanel.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.3f, 0.3f);
            contentRect.anchorMax = new Vector2(0.7f, 0.7f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            
            Image contentImage = contentPanel.AddComponent<Image>();
            contentImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
            
            // 标题
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(contentPanel.transform);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "技能系统";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 20;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "SkillPanel.prefab");
            PrefabUtility.SaveAsPrefabAsset(panel, path);
            DestroyImmediate(panel);
        }
        
        private void CreateBreakthroughPanelPrefab()
        {
            GameObject panel = new GameObject("BreakthroughPanel");
            Canvas canvas = panel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            panel.AddComponent<CanvasScaler>();
            panel.AddComponent<GraphicRaycaster>();
            
            // 背景面板
            GameObject bgPanel = new GameObject("Background");
            bgPanel.transform.SetParent(panel.transform);
            Image bgImage = bgPanel.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.9f);
            RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 内容面板
            GameObject contentPanel = new GameObject("Content");
            contentPanel.transform.SetParent(panel.transform);
            RectTransform contentRect = contentPanel.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.3f, 0.3f);
            contentRect.anchorMax = new Vector2(0.7f, 0.7f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            
            Image contentImage = contentPanel.AddComponent<Image>();
            contentImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "BreakthroughPanel.prefab");
            PrefabUtility.SaveAsPrefabAsset(panel, path);
            DestroyImmediate(panel);
        }
        
        private void CreateFloatingTextPrefab()
        {
            GameObject floatingText = new GameObject("FloatingText");
            Canvas canvas = floatingText.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            floatingText.AddComponent<CanvasScaler>();
            
            Text text = floatingText.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 16;
            text.color = Color.yellow;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            
            RectTransform rect = text.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 30);
            
            // 添加动画组件
            FloatingTextAnimation anim = floatingText.AddComponent<FloatingTextAnimation>();
            
            // 保存为预制体
            string path = Path.Combine(_prefabSavePath, "FloatingText.prefab");
            PrefabUtility.SaveAsPrefabAsset(floatingText, path);
            DestroyImmediate(floatingText);
        }
        
        private Sprite CreateSquareSprite(Color color)
        {
            Texture2D texture = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
        
        private Sprite CreateExclamationSprite()
        {
            Texture2D texture = new Texture2D(16, 16);
            Color[] pixels = new Color[16 * 16];
            
            // 创建简单的感叹号图案
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    int index = y * 16 + x;
                    if (x >= 6 && x <= 9 && y <= 10) // 竖线
                    {
                        pixels[index] = Color.yellow;
                    }
                    else if (x >= 6 && x <= 9 && y >= 12 && y <= 13) // 底部点
                    {
                        pixels[index] = Color.yellow;
                    }
                    else
                    {
                        pixels[index] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));
        }
        
        private RuntimeAnimatorController CreatePlayerAnimatorController()
        {
            // 创建简单的动画控制器
            // 这是一个简化版本，实际使用中需要创建完整的动画状态机
            return null;
        }
    }
    
    public class FloatingTextAnimation : MonoBehaviour
    {
        public float FloatSpeed = 2f;
        public float FadeTime = 2f;
        private Vector3 _startPosition;
        private float _startTime;
        
        private void Start()
        {
            _startPosition = transform.position;
            _startTime = Time.time;
        }
        
        private void Update()
        {
            float elapsed = Time.time - _startTime;
            float progress = elapsed / FadeTime;
            
            if (progress >= 1f)
            {
                Destroy(gameObject);
                return;
            }
            
            // 向上移动
            transform.position = _startPosition + Vector3.up * (FloatSpeed * elapsed);
            
            // 淡出
            Text text = GetComponent<Text>();
            if (text != null)
            {
                Color color = text.color;
                color.a = 1f - progress;
                text.color = color;
            }
        }
    }
}