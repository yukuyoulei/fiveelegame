using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace FiveElements.Unity.Editor
{
    public class SceneSetupWizard : EditorWindow
    {
        private string _sceneName = "FiveElementsGame";
        
        [MenuItem("Five Elements/Setup Game Scene")]
        public static void ShowWindow()
        {
            GetWindow<SceneSetupWizard>("Setup Game Scene");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Five Elements Game Scene Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            _sceneName = EditorGUILayout.TextField("Scene Name:", _sceneName);
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("Create Game Scene"))
            {
                CreateGameScene();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Generate All Required Assets"))
            {
                GenerateRequiredAssets();
            }
        }
        
        private void CreateGameScene()
        {
            // 创建新场景
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // 创建主游戏对象
            GameObject gameBootstrap = new GameObject("GameBootstrap");
            gameBootstrap.AddComponent<FiveElements.Unity.GameBootstrap>();
            
            // 创建相机
            GameObject cameraObj = new GameObject("MainCamera");
            cameraObj.tag = "MainCamera";
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.backgroundColor = new Color(0.529f, 0.808f, 0.922f);
            cameraObj.AddComponent<AudioListener>();
            
            // 创建UI Canvas
            GameObject canvasObj = new GameObject("UICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 创建事件系统
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            // 保存场景
            string scenePath = $"Assets/{_sceneName}.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", $"Game scene '{_sceneName}' created successfully!", "OK");
        }
        
        private void GenerateRequiredAssets()
        {
            // 生成预设
            PrefabGenerator generator = GetWindow<PrefabGenerator>("Generate Prefabs");
            generator.GeneratePrefabs();
            
            // 创建必要的文件夹
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
            
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", "All required assets generated successfully!", "OK");
        }
    }
    
    public class QuickStartGuide : EditorWindow
    {
        [MenuItem("Five Elements/Quick Start Guide")]
        public static void ShowWindow()
        {
            GetWindow<QuickStartGuide>("Quick Start Guide");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Five Elements Unity Game", EditorStyles.boldLabel);
            GUILayout.Label("Quick Start Guide", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("Step 1: Setup Scene", EditorStyles.boldLabel);
            GUILayout.Label("Click 'Five Elements/Setup Game Scene' to create a new game scene.");
            GUILayout.Space(5);
            
            GUILayout.Label("Step 2: Generate Prefabs", EditorStyles.boldLabel);
            GUILayout.Label("Click 'Five Elements/Generate Prefabs' to create all required game objects.");
            GUILayout.Space(5);
            
            GUILayout.Label("Step 3: Configure Game", EditorStyles.boldLabel);
            GUILayout.Label("Assign the generated prefabs to the OfflineGameManager component.");
            GUILayout.Space(5);
            
            GUILayout.Label("Step 4: Play", EditorStyles.boldLabel);
            GUILayout.Label("Press Play to start the game!");
            GUILayout.Space(10);
            
            GUILayout.Label("Controls:", EditorStyles.boldLabel);
            GUILayout.Label("• WASD/Arrow Keys: Move");
            GUILayout.Label("• Space: Attack");
            GUILayout.Label("• E: Collect Resources");
            GUILayout.Label("• F: Talk to NPCs");
            GUILayout.Label("• Boundary + Direction: Change Scene");
            GUILayout.Space(10);
            
            if (GUILayout.Button("Open Migration Guide"))
            {
                string guidePath = "Assets/Scripts/UNITY_MIGRATION_GUIDE.md";
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(guidePath));
            }
            
            if (GUILayout.Button("Generate All Assets"))
            {
                SceneSetupWizard wizard = GetWindow<SceneSetupWizard>("Setup Game Scene");
                wizard.GenerateRequiredAssets();
            }
        }
    }
}