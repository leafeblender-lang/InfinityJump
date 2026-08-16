using UnityEngine;
using UnityEngine.InputSystem; // Novi Input System
using System.IO;

public class ScreenshotCapture : MonoBehaviour
{
    void Update()
    {
        // Provera pritiska P tastera (Novi Input System)
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        // Putanja do Desktopa
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, "unitySkrinovi");

        // Ako folder ne postoji, napravi ga
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Generiši jedinstveno ime fajla (po datumu i vremenu)
        string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(folderPath, filename);

        // Snimi screenshot
        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log("Screenshot sa?uvan: " + fullPath);
    }
}
