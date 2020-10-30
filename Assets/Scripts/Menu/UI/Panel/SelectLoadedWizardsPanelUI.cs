using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLoadedWizardsPanelUI : MonoBehaviour {
	[SerializeField]
	private LoadedWizardButtonUI[] loadedWizardButtonUIs;

	private int currentPage;
	private int maxPages;

	private void Awake () {
		loadedWizardButtonUIs = GetComponentsInChildren<LoadedWizardButtonUI> ();
	}

	public void PopulateLoadedWizardButtons () {
		Debug.Log ("populating wizard buttons");
		List<WizardSaveData> wizardSaveData = SaveManager.instance.LoadWizardSavesFromDiskJSON();
		int wizardSaveDataCount = wizardSaveData.Count;
		for (int i = 0; i < loadedWizardButtonUIs.Length; i++) {
			if (i < wizardSaveDataCount) {
				loadedWizardButtonUIs[i].DisplayWizardUI (wizardSaveData [i]);
				} else {
				loadedWizardButtonUIs [i].BlankOutFields ();
			}
		}
	}
}
