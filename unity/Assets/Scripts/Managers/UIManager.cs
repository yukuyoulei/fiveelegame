using UnityEngine;
using UnityEngine.UI;
using FiveElements.Shared.Models;

namespace FiveElements.Unity.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("Message UI")]
        public GameObject MessagePanel;
        public Text MessageText;
        public float MessageDisplayTime = 3f;

        private Coroutine _messageCoroutine;

        public void ShowMessage(string message)
        {
            if (MessagePanel != null && MessageText != null)
            {
                MessageText.text = message;
                MessagePanel.SetActive(true);
                
                if (_messageCoroutine != null)
                {
                    StopCoroutine(_messageCoroutine);
                }
                
                _messageCoroutine = StartCoroutine(HideMessageAfterDelay());
            }
        }

        private System.Collections.IEnumerator HideMessageAfterDelay()
        {
            yield return new WaitForSeconds(MessageDisplayTime);
            
            if (MessagePanel != null)
            {
                MessagePanel.SetActive(false);
            }
        }

        public void ShowMainElementSelection()
        {
            // Implementation for main element selection UI
        }

        public void UpdatePlayerStats(PlayerStats playerStats)
        {
            // Update UI elements with player stats
        }

        public void UpdateMap(MapView mapView)
        {
            // Update map visualization
        }
    }
}