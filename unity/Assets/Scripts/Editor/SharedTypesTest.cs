#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FiveElements.Shared;
using FiveElements.Shared.Models;

namespace FiveElements.Unity.Editor
{
    /// <summary>
    /// Test script to verify shared types are accessible
    /// </summary>
    public class SharedTypesTest : Editor
    {
        [MenuItem("Five Elements/Test Shared Types")]
        public static void TestSharedTypes()
        {
            Debug.Log("Testing shared types access...");
            
            try
            {
                // Test basic types
                var element = ElementType.Metal;
                Debug.Log($"ElementType test: {element}");
                
                // Test Position
                var position = new Position(5, 10);
                Debug.Log($"Position test: ({position.X}, {position.Y})");
                
                // Test PlayerStats
                var playerStats = new PlayerStats();
                playerStats.Name = "TestPlayer";
                playerStats.Elements.MainElement = ElementType.Fire;
                Debug.Log($"PlayerStats test: {playerStats.Name}, Main element: {playerStats.Elements.MainElement}");
                
                // Test MapView
                var mapView = new MapView(position);
                Debug.Log($"MapView test: Center at ({mapView.CenterPosition.X}, {mapView.CenterPosition.Y})");
                
                // Test PlayerInfo
                var playerInfo = new PlayerInfo();
                playerInfo.Name = "TestPlayer2";
                playerInfo.Position = position;
                playerInfo.MainElement = ElementType.Water;
                Debug.Log($"PlayerInfo test: {playerInfo.Name} at ({playerInfo.Position.X}, {playerInfo.Position.Y})");
                
                Debug.Log("✅ All shared types are accessible!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error accessing shared types: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
#endif
