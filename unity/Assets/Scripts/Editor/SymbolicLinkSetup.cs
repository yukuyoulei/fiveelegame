#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

namespace FiveElements.Unity.Editor
{
    /// <summary>
    /// Automatically sets up symbolic links to shared code when Unity compiles
    /// </summary>
    [InitializeOnLoad]
    public class SymbolicLinkSetup
    {
        static SymbolicLinkSetup()
        {
            // Check if shared code directory exists
            string sharedPath = Path.Combine(Application.dataPath, "Scripts/Shared/FiveElements.Shared");
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string sharedSourcePath = Path.Combine(projectRoot, "src/FiveElements.Shared");
            
            if (!Directory.Exists(sharedPath))
            {
                UnityEngine.Debug.Log("Shared code directory not found. Attempting to create symbolic link...");
                
                // Try to create symbolic link
                if (Directory.Exists(sharedSourcePath))
                {
                    // Ensure the parent directory exists
                    string parentDir = Path.GetDirectoryName(sharedPath);
                    if (!Directory.Exists(parentDir))
                    {
                        Directory.CreateDirectory(parentDir);
                    }
                    
                    try
                    {
                        // Create symbolic link using mklink command
                        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c mklink /D \"" + sharedPath + "\" \"" + sharedSourcePath + "\"");
                        processInfo.UseShellExecute = false;
                        processInfo.RedirectStandardOutput = true;
                        processInfo.RedirectStandardError = true;
                        processInfo.CreateNoWindow = true;
                        processInfo.Verb = "runas"; // Request administrator privileges
                        
                        Process process = Process.Start(processInfo);
                        process.WaitForExit();
                        
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        
                        if (process.ExitCode == 0)
                        {
                            UnityEngine.Debug.Log("Symbolic link created successfully!");
                            AssetDatabase.Refresh(); // Refresh Unity's asset database
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning("Failed to create symbolic link automatically.");
                            UnityEngine.Debug.LogWarning("Error: " + error);
                            UnityEngine.Debug.LogWarning("Please run setup-symlinks.bat as Administrator manually.");
                            
                            // Fallback: try to copy files if symbolic link fails
                            TryCopySharedCode(sharedPath, sharedSourcePath);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        UnityEngine.Debug.LogError("Exception creating symbolic link: " + ex.Message);
                        UnityEngine.Debug.LogWarning("Please run setup-symlinks.bat as Administrator manually.");
                        
                        // Fallback: try to copy files if symbolic link fails
                        TryCopySharedCode(sharedPath, sharedSourcePath);
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Shared source directory not found: " + sharedSourcePath);
                }
            }
        }
        
        private static void TryCopySharedCode(string targetPath, string sourcePath)
        {
            try
            {
                UnityEngine.Debug.Log("Attempting to copy shared code as fallback...");
                
                // Copy all files recursively
                CopyDirectory(sourcePath, targetPath);
                
                UnityEngine.Debug.Log("Shared code copied successfully!");
                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("Failed to copy shared code: " + ex.Message);
            }
        }
        
        private static void CopyDirectory(string sourcePath, string targetPath)
        {
            // Create all subdirectories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }
            
            // Copy all files
            foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                string targetFilePath = filePath.Replace(sourcePath, targetPath);
                File.Copy(filePath, targetFilePath, true);
            }
        }
    }
}
#endif
