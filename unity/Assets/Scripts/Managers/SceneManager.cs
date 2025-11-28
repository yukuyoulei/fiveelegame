using UnityEngine;
using UnityEngine.SceneManagement;

namespace FiveElements.Unity.Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }
        
        [Header("Scene Generation")]
        public GameObject BackgroundLayer;
        public GameObject MiddleLayer;
        public GameObject ForegroundLayer;
        
        [Header("Scene Objects")]
        public GameObject[] TreePrefabs;
        public GameObject[] MountainPrefabs;
        public GameObject[] GrassPrefabs;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            GenerateScene();
        }
        
        public void GenerateScene()
        {
            ClearScene();
            
            // 生成背景层
            GenerateBackgroundLayer();
            
            // 生成中景层
            GenerateMiddleLayer();
            
            // 生成前景层
            GenerateForegroundLayer();
        }
        
        private void ClearScene()
        {
            // 清除现有的场景对象
            if (BackgroundLayer != null)
            {
                foreach (Transform child in BackgroundLayer.transform)
                {
                    if (child != null)
                        DestroyImmediate(child.gameObject);
                }
            }
            
            if (MiddleLayer != null)
            {
                foreach (Transform child in MiddleLayer.transform)
                {
                    if (child != null)
                        DestroyImmediate(child.gameObject);
                }
            }
            
            if (ForegroundLayer != null)
            {
                foreach (Transform child in ForegroundLayer.transform)
                {
                    if (child != null)
                        DestroyImmediate(child.gameObject);
                }
            }
        }
        
        private void GenerateBackgroundLayer()
        {
            if (BackgroundLayer == null) return;
            
            // 生成远山
            for (int i = 0; i < 5; i++)
            {
                Vector3 position = new Vector3(i * 12f + OfflineGameManager.Instance.WorldPosition.X * 2f, 3f, 10f);
                CreateMountain(position, 8f, 6f);
            }
        }
        
        private void GenerateMiddleLayer()
        {
            if (MiddleLayer == null) return;
            
            // 生成树木
            for (int i = 0; i < 8; i++)
            {
                Vector3 position = new Vector3(
                    i * 6f + OfflineGameManager.Instance.WorldPosition.X * 3f,
                    -1f + Mathf.Sin(i * 0.5f) * 1.5f,
                    5f
                );
                CreateTree(position);
            }
        }
        
        private void GenerateForegroundLayer()
        {
            if (ForegroundLayer == null) return;
            
            // 生成草地纹理
            for (int i = 0; i < 20; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(-8f, 8f),
                    Random.Range(-4f, -2f),
                    1f
                );
                CreateGrass(position);
            }
        }
        
        private void CreateMountain(Vector3 position, float width, float height)
        {
            GameObject mountain = new GameObject("Mountain");
            mountain.transform.SetParent(BackgroundLayer.transform);
            mountain.transform.position = position;
            
            SpriteRenderer renderer = mountain.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateMountainSprite(width, height);
            renderer.color = new Color(0.4f, 0.4f, 0.4f, 0.3f);
            renderer.sortingOrder = -10;
            
            // 添加视差效果
            ParallaxEffect parallax = mountain.AddComponent<ParallaxEffect>();
            parallax.ParallaxMultiplier = 0.3f;
        }
        
        private void CreateTree(Vector3 position)
        {
            GameObject tree = new GameObject("Tree");
            tree.transform.SetParent(MiddleLayer.transform);
            tree.transform.position = position;
            
            // 树干
            GameObject trunk = new GameObject("Trunk");
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 0, 0);
            
            SpriteRenderer trunkRenderer = trunk.AddComponent<SpriteRenderer>();
            trunkRenderer.sprite = CreateRectangleSprite(0.3f, 1.5f, new Color(0.545f, 0.271f, 0.075f)); // 棕色
            trunkRenderer.sortingOrder = 2;
            
            // 树叶
            GameObject leaves = new GameObject("Leaves");
            leaves.transform.SetParent(tree.transform);
            leaves.transform.localPosition = new Vector3(0, 1f, 0);
            
            SpriteRenderer leavesRenderer = leaves.AddComponent<SpriteRenderer>();
            leavesRenderer.sprite = CreateCircleSprite(2f, Color.green);
            leavesRenderer.sortingOrder = 3;
            
            // 添加视差效果
            ParallaxEffect parallax = tree.AddComponent<ParallaxEffect>();
            parallax.ParallaxMultiplier = 0.6f;
        }
        
        private void CreateGrass(Vector3 position)
        {
            GameObject grass = new GameObject("Grass");
            grass.transform.SetParent(ForegroundLayer.transform);
            grass.transform.position = position;
            
            LineRenderer lineRenderer = grass.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.color = new Color(0.133f, 0.545f, 0.133f, 0.5f); // 半透明绿色
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.sortingOrder = 1;
            
            // 创建草地形状
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            lineRenderer.SetPosition(1, new Vector3(-0.1f, 0.3f, 0));
            lineRenderer.SetPosition(2, new Vector3(0.1f, 0.3f, 0));
        }
        
        private Sprite CreateMountainSprite(float width, float height)
        {
            Texture2D texture = new Texture2D((int)(width * 32), (int)(height * 32));
            Color[] pixels = new Color[texture.width * texture.height];
            
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float normalizedX = (float)x / texture.width;
                    float normalizedY = (float)y / texture.height;
                    
                    // 创建三角形山体
                    float triangleHeight = 1f - Mathf.Abs(normalizedX - 0.5f) * 2f;
                    
                    if (normalizedY <= triangleHeight)
                    {
                        pixels[y * texture.width + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * texture.width + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0f));
        }
        
        private Sprite CreateRectangleSprite(float width, float height, Color color)
        {
            Texture2D texture = new Texture2D((int)(width * 32), (int)(height * 32));
            Color[] pixels = new Color[texture.width * texture.height];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        
        private Sprite CreateCircleSprite(float radius, Color color)
        {
            int size = (int)(radius * 32);
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            
            Vector2 center = new Vector2(size / 2f, size / 2f);
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= size / 2f)
                    {
                        pixels[y * size + x] = color;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }
        
        public void OnSceneChanged()
        {
            GenerateScene();
        }
    }
    
    public class ParallaxEffect : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float ParallaxMultiplier = 0.5f;
        
        private Transform _cameraTransform;
        private Vector3 _previousCameraPosition;
        
        private void Start()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                _cameraTransform = mainCamera.transform;
                _previousCameraPosition = _cameraTransform.position;
            }
        }
        
        private void Update()
        {
            if (_cameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    _cameraTransform = mainCamera.transform;
                    _previousCameraPosition = _cameraTransform.position;
                    return;
                }
                return;
            }
            
            Vector3 deltaMovement = _cameraTransform.position - _previousCameraPosition;
            transform.position += deltaMovement * ParallaxMultiplier;
            _previousCameraPosition = _cameraTransform.position;
        }
    }
}