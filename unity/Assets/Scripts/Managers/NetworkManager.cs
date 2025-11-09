using UnityEngine;
using System.Collections;
using System.Text;
using WebSocketSharp;
using FiveElements.Shared.Messages;
using FiveElements.Shared;

namespace FiveElements.Unity.Managers
{
    public class NetworkManager : MonoBehaviour
    {
        private WebSocket _webSocket;
        private bool _isConnected = false;
        private const string ServerUrl = "ws://localhost:5000/ws";

        public IEnumerator ConnectToServer()
        {
            _webSocket = new WebSocket(ServerUrl);
            
            _webSocket.OnOpen += (sender, e) =>
            {
                Debug.Log("Connected to server");
                _isConnected = true;
            };
            
            _webSocket.OnMessage += (sender, e) =>
            {
                HandleMessage(e.Data);
            };
            
            _webSocket.OnError += (sender, e) =>
            {
                Debug.LogError($"WebSocket error: {e.Message}");
                _isConnected = false;
            };
            
            _webSocket.OnClose += (sender, e) =>
            {
                Debug.Log("Disconnected from server");
                _isConnected = false;
                
                // Try to reconnect after 5 seconds
                StartCoroutine(Reconnect());
            };
            
            _webSocket.ConnectAsync();
            
            yield return new WaitUntil(() => _isConnected || _webSocket == null);
        }

        private IEnumerator Reconnect()
        {
            yield return new WaitForSeconds(5f);
            
            if (!_isConnected)
            {
                Debug.Log("Attempting to reconnect...");
                yield return ConnectToServer();
            }
        }

        private void HandleMessage(string messageJson)
        {
            try
            {
                var message = JsonUtility.FromJson<MessageBase>(messageJson);
                if (message == null) return;

                // Handle messages on main thread
                UnityMainThreadDispatcher.Instance.Enqueue(() =>
                {
                    switch (message.Type)
                    {
                        case nameof(WelcomeMessage):
                            var welcomeMsg = JsonUtility.FromJson<WelcomeMessage>(messageJson);
                            GameManager.Instance.OnWelcomeMessageReceived(welcomeMsg!);
                            break;
                            
                        case nameof(MapUpdateMessage):
                            var mapMsg = JsonUtility.FromJson<MapUpdateMessage>(messageJson);
                            GameManager.Instance.OnMapUpdateReceived(mapMsg!);
                            break;
                            
                        case nameof(PlayerStatsUpdateMessage):
                            var statsMsg = JsonUtility.FromJson<PlayerStatsUpdateMessage>(messageJson);
                            GameManager.Instance.OnPlayerStatsUpdated(statsMsg!);
                            break;
                            
                        case nameof(TrainingResultMessage):
                            var trainingMsg = JsonUtility.FromJson<TrainingResultMessage>(messageJson);
                            GameManager.Instance.OnTrainingResultReceived(trainingMsg!);
                            break;
                            
                        case nameof(HarvestResultMessage):
                            var harvestMsg = JsonUtility.FromJson<HarvestResultMessage>(messageJson);
                            GameManager.Instance.OnHarvestResultReceived(harvestMsg!);
                            break;
                            
                        case nameof(AttackResultMessage):
                            var attackMsg = JsonUtility.FromJson<AttackResultMessage>(messageJson);
                            GameManager.Instance.OnAttackResultReceived(attackMsg!);
                            break;
                            
                        case nameof(PlayerJoinedMessage):
                            var joinMsg = JsonUtility.FromJson<PlayerJoinedMessage>(messageJson);
                            GameManager.Instance.OnPlayerJoined(joinMsg!);
                            break;
                            
                        case nameof(PlayerLeftMessage):
                            var leaveMsg = JsonUtility.FromJson<PlayerLeftMessage>(messageJson);
                            GameManager.Instance.OnPlayerLeft(leaveMsg!);
                            break;
                            
                        case nameof(ErrorMessage):
                            var errorMsg = JsonUtility.FromJson<ErrorMessage>(messageJson);
                            GameManager.Instance.OnError(errorMsg!);
                            break;
                    }
                });
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error handling message: {ex.Message}");
            }
        }

        public void SendMessage(MessageBase message)
        {
            if (!_isConnected || _webSocket == null)
            {
                Debug.LogWarning("Not connected to server");
                return;
            }

            try
            {
                var messageJson = JsonUtility.ToJson(message);
                _webSocket.SendAsync(messageJson);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error sending message: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            if (_webSocket != null)
            {
                _webSocket.CloseAsync();
                _webSocket = null;
            }
        }
    }

    // Helper class to execute code on main thread
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly System.Collections.Concurrent.ConcurrentQueue<System.Action> _executionQueue = new System.Collections.Concurrent.ConcurrentQueue<System.Action>();
        private static UnityMainThreadDispatcher _instance = null;

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UnityMainThreadDispatcher>();
                    if (_instance == null)
                    {
                        var go = new GameObject("UnityMainThreadDispatcher");
                        _instance = go.AddComponent<UnityMainThreadDispatcher>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        void Update()
        {
            while (_executionQueue.TryDequeue(out System.Action action))
            {
                action.Invoke();
            }
        }

        public void Enqueue(System.Action action)
        {
            _executionQueue.Enqueue(action);
        }
    }
}