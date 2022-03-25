using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	bool shouldCallManageCameraRatio = false;

	Camera mainCam;
	const float longDeviceSizeMultiplier = 1.04f;
	const float defaultScreenRatio = 1.78f;
	float screenRatio;

	private void Awake()
	{
		mainCam = GetComponent<Camera>();
		ManageCameraRatio();
	}
	/// <summary>
	/// This will manage the camera fov or orthographic size according to device screen ration
	/// </summary>
	void ManageCameraRatio()
	{
		if (shouldCallManageCameraRatio)
		{
			screenRatio = Screen.height / Screen.width;
			if (screenRatio > defaultScreenRatio)
			{
				if (!mainCam.orthographic)
				{
					mainCam.fieldOfView = (mainCam.fieldOfView * screenRatio * longDeviceSizeMultiplier) / defaultScreenRatio;
				}
				else
				{
					mainCam.orthographicSize = (mainCam.orthographicSize / defaultScreenRatio) * screenRatio;
				}
			}
		}
	}
}
