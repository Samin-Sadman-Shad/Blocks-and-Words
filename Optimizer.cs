using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    private void Awake()
    {
        AdjustFPS();
    }

    [SerializeField] bool debugMode = false;
    [SerializeField] float fps = 60.0f;
    float currentFrameTime;
    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / fps;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }

    void AdjustFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 9999;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine(WaitForNextFrame());
        if (debugMode == false)
        {
#if !UNITY_EDITOR
            Debug.unityLogger.logEnabled = false;
#endif
        }
    }
}
