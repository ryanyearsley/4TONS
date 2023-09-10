using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObituaryScreenUI : AbstractScreenUI
{
	[SerializeField]
	private ObituaryPanelUI obituaryPanel;

	private void Awake()
	{
		Debug.Log("obituary screen awake");
	}

	protected override void Start()
	{
		base.Start();
	}


	protected override void OnScreenChange(MenuScreen mainMenuScreen)
	{
		base.OnScreenChange(mainMenuScreen);
		if (screenActiveStates.Contains(mainMenuScreen))
		{
			UpdateObituaryPanels();
		}
	}

	private void UpdateObituaryPanels()
	{
		List<WizardSaveData> wizardSaveDatas = WizardSaveDataManager.instance.deadWizardSaveDatas;
		Debug.Log("updating wizard select panels. wizard count: " + wizardSaveDatas.Count);
		obituaryPanel.PopulateObituaryEntries(wizardSaveDatas);
	}
}