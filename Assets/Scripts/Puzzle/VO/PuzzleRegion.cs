using System;
using UnityEngine;
using TMPro;

[Serializable]
public class PuzzleRegion {
    public bool isActive;
    public bool alwaysVisible;
    public PuzzleKey puzzleKey;
    public PuzzleEntity occupantEntity;
    public Vector2Int origin;
    public Vector2Int dimensions;
    public CoordinateBounds bounds;


    public void HideRegion() {
        isActive = false;
        if (occupantEntity != null)
            occupantEntity.gameObject.SetActive (false);
	}
    public void UnhideRegion () {
        isActive = true;
        if (occupantEntity != null)
            occupantEntity.gameObject.SetActive (true);
    }

}
