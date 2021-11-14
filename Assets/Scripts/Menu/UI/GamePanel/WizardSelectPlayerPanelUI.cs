using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WizardSelectPlayerPanelUI : MonoBehaviour {

	private bool initialized;

	[SerializeField]
	private GameObject wizardSelectionButtonPrefab;

	[SerializeField]
	private RectTransform verticalLayoutGroupRectTransform;

	[SerializeField]
	private ScrollRect scrollRect;
	private float defaultRectVerticalSize;

	[SerializeReference]
	private GameObject noWizardText;

	[SerializeField]
	private List<LoadedWizardSelectionUI> loadedWizardSelectionUI;

	void Awake () {
		Debug.Log ("wizard select panel player awake...");
		scrollRect = GetComponentInChildren<ScrollRect> ();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
	}
	
	private void InitializePanel () {
		scrollRect = GetComponentInChildren<ScrollRect> ();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
		initialized = true;
	}

	public void PopulateLoadedWizardButtons (List<WizardSaveData> wizardSaveDatas) {
		if (!initialized) {
			InitializePanel ();
		}
		if (wizardSaveDatas.Count == 0) {
			noWizardText.SetActive (true);
		} else {
			noWizardText.SetActive (false);
		}
		Debug.Log ("populating loaded wizard buttons. wizard data length: " + wizardSaveDatas.Count);
		int wizardSaveDataCount = wizardSaveDatas.Count;
		UpdateLeaderboardEntryGrouping (wizardSaveDataCount);
		for (int i = 0; i < wizardSaveDataCount; i++) {
			if (i >= loadedWizardSelectionUI.Count) {
				//adds a button if necessary
				loadedWizardSelectionUI.Add (Instantiate (wizardSelectionButtonPrefab, verticalLayoutGroupRectTransform.transform).GetComponent<LoadedWizardSelectionUI> ());
			}
			loadedWizardSelectionUI [i].DisplayWizardUI (wizardSaveDatas [i]);
		}
		}

	private void UpdateLeaderboardEntryGrouping (int wizardSaveDataCount) {
		for (int i = 0; i < loadedWizardSelectionUI.Count; i++) {
			if (i >= wizardSaveDataCount) {
				LoadedWizardSelectionUI deletingButton = loadedWizardSelectionUI[i];
				loadedWizardSelectionUI.RemoveAt (i);
				Destroy (deletingButton.gameObject);
			}
		}
		if (wizardSaveDataCount >= 5) {
			Debug.Log ("resizing wizard select panel/rect Transform.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, wizardSaveDataCount * 160);
			scrollRect.vertical = true;
		} else {
			Debug.Log ("5 or less wizards. reverting to default size and disabling scroll.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, defaultRectVerticalSize);
			scrollRect.vertical = false;
		}

	}
}
