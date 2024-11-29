using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            string screenshotName;

            int randomNumber = Random.Range(0, 100000);

            screenshotName = "ScreenShot" + randomNumber + ".png";

            ScreenCapture.CaptureScreenshot(screenshotName);
        }
    }
}
