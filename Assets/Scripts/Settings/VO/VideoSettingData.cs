using UnityEngine;
using System;

[System.Serializable]
public class VideoSettingsData {
	public bool isVSyncOn = false;
	public FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;
	public int resolutionIndex = 2;
	public int framerateIndex;
}

[Serializable]
public class ResolutionSetting {
	public int resX;
	public int resY;
	public ResolutionSetting(int x, int y) {
		resX = x;
		resY = y;
	}
}

