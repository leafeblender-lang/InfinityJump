using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;

public class UpdateChecker : MonoBehaviour
{
    private AppUpdateManager appUpdateManager;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appUpdateManager = new AppUpdateManager();
            StartCoroutine(CheckForUpdateCoroutine());
        }
        else
        {
            Debug.Log("In-App Update presko?en (nije Android platforma).");
        }
        
    }

    IEnumerator CheckForUpdateCoroutine()
    {
        var appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfo = appUpdateInfoOperation.GetResult();

            var immediateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
                appUpdateInfo.IsUpdateTypeAllowed(immediateOptions))
            {
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo, immediateOptions);

                yield return startUpdateRequest;

              
            }
            else
            {
                Debug.Log("Update nije dostupan ili immediate update nije dozvoljen.");
            }
        }
        else
        {
            Debug.LogError("Greška pri proveri update-a: " + appUpdateInfoOperation.Error);
        }
    }
}
