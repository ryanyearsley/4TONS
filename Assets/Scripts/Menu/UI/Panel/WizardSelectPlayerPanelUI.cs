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
	private GameObject noWizardsTextObject;

	[SerializeField]
	private List<LoadedWizardSelectionUI> loadedWizardButtonUIs;

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
			noWizardsTextObject.SetActive (true);
		} else {
			noWizardsTextObject.SetActive (false);
		}
		Debug.Log ("populating loaded wizard buttons. wizard data length: " + wizardSaveDatas.Count);
		int wizardSaveDataCount = wizardSaveDatas.Count;
		UpdateWizardSelectionGrouping (wizardSaveDataCount);
		for (int i = 0; i < wizardSaveDataCount; i++) {
			if (i >= loadedWizardButtonUIs.Count) {
				//adds a button if necessary
				loadedWizardButtonUIs.Add (Instantiate (wizardSelectionButtonPrefab, verticalLayoutGroupRectTransform.transform).GetComponent<LoadedWizardSelectionUI> ());
			}
			loadedWizardButtonUIs [i].DisplayWizardUI (wizardSaveDatas [i]);
		}
		}

	private void UpdateWizardSelectionGrouping (int wizardSaveDataCount) {
		for (int i = 0; i < loadedWizardButtonUIs.Count; i++) {
			if (i >= wizardSaveDataCount) {
				LoadedWizardSelectionUI deletingButton = loadedWizardButtonUIs[i];
				loadedWizardButtonUIs.RemoveAt (i);
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
