using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUIObject : MonoBehaviour {

	public Image tileImage;

	private int tileIndex;

	public void Initialize (int index, Sprite sprite, Color color) {
		tileImage.sprite = sprite;
		tileImage.color = color;
		tileIndex = index;
	}

	public void OnButtonDown () {
		LevelBuilderManager.instance.OnSelectTile (tileIndex);
	}
}