using System;
using UnityEngine;

[Serializable]
public class ParallaxBackgroundData {
	public Color backgroundColor;
	//PLACE FRONT TO BAck
	public LayerData[] layerDatas;
}

[Serializable]
public class LayerData {
	public Sprite sprite;
	public Vector2 spriteOffset = Vector3.zero;
	public float distanceScaleX = 1;
	public float distanceScaleY = 1;
	public float opacity;
}
