using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;

public class ScreenshotSaver : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        // Putanja do desktopa i foldera "skshoot"
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string folderPath = Path.Combine(desktopPath, "skshoot");

        // Ako folder ne postoji, napravi ga
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Ime fajla sa vremenom za jedinstvenost
        string fileName = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(folderPath, fileName);

        // Napravi screenshot
        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log("Screenshot saved to: " + fullPath);
    }
}
