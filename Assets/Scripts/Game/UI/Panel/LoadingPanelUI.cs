using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelUI : AbstractPanelUI
{
	public TipData tipData;

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private Text loadingText;
	[SerializeField]
	private Text tipText;
	protected override void OnUIChange (GameState gameState) {
		if (panelActiveStates.Contains (gameState)) {
			panelObject.SetActive (true);

			string loadingTextString = "Loading: \n";
			if (GameManager.instance != null) {
				loadingTextString += GameManager.instance.gameContext.zoneData.zone;
				backgroundImage.sprite = GameManager.instance.gameContext.zoneData.loadingBackgroundSprite;
			} 

			if (GauntletGameManager.instance != null) {
				loadingTextString += " Tower\nFloor " + (GauntletGameManager.instance.GetProgress ().currentLevelIndex + 1);
			}
			loadingText.text = loadingTextString;

			int randomTipIndex = Random.Range(0, tipData.tips.Length - 1);
			tipText.text = tipData.tips [randomTipIndex];
		} else {
			panelObject.SetActive (false);
		}
	}
}
