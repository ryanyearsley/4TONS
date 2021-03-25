using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerProgressPanelUI : AbstractPanelUI
{
	private int finalFloorNumber;

	[SerializeField]
	private Image towerProgressImage;

	protected override void InitializePanel () {
		base.InitializePanel ();
		if (GameManager.instance != null) {
			finalFloorNumber = GameManager.instance.gameContext.worldData.mapDatas.Length;
			GameManager.instance.loadLevelEvent += OnLoadLevel;
			UpdateTowerProgressUI (1);
		}
		
	}

	public void OnLoadLevel(int levelIndex) {
		UpdateTowerProgressUI (levelIndex + 1);
	}

	public void OnPortalEntered (int nextFloorIndex) {
		Debug.Log ("floor complete. updating obj UI");
		UpdateTowerProgressUI (nextFloorIndex + 1);
	}
	private void UpdateTowerProgressUI (int floorNumber) {
		Debug.Log ("updating tower UI. current floor number: " + floorNumber + ", top: " + finalFloorNumber);
		towerProgressImage.fillAmount = ((float)floorNumber / finalFloorNumber);
	}

}
