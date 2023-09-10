using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObituaryPanelUI : MonoBehaviour
{

	private bool initialized;
	[SerializeField]
	private int maxObituariesCount = 50;

	[SerializeField]
	private GameObject obituaryEntryPrefab;

	[SerializeField]
	private RectTransform verticalLayoutGroupRectTransform;

	[SerializeField]
	private ScrollRect scrollRect;
	private float defaultRectVerticalSize;
	private Vector3 defaultLocalPosition;

	[SerializeReference]
	private GameObject noWizardText;

	[SerializeField]
	private TMP_Text totalDeathsText;

	[SerializeField]
	private List<ObituaryEntryUI> loadedObituaryUI;

	void Awake()
	{
		scrollRect = GetComponentInChildren<ScrollRect>();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
		defaultLocalPosition = verticalLayoutGroupRectTransform.localPosition;
	}
	private void InitializePanel()
	{
		scrollRect = GetComponentInChildren<ScrollRect>();
		defaultRectVerticalSize = verticalLayoutGroupRectTransform.rect.height;
		initialized = true;
	}

	public void PopulateObituaryEntries(List<WizardSaveData> wizardSaveDatas)
	{
		if (!initialized)
		{
			InitializePanel();
		}
		if (wizardSaveDatas.Count == 0)
		{
			noWizardText.SetActive(true);
		}
		else
		{
			noWizardText.SetActive(false);
		}
		totalDeathsText.text = "Total Deaths: " + wizardSaveDatas.Count;
		int wizardCount = wizardSaveDatas.Count;
		int displayCount = wizardCount > maxObituariesCount? maxObituariesCount : wizardCount;
		UpdateObituaryGroupingUI(displayCount);
		for (int i = 0; i < displayCount; i++)
		{
			if (i >= loadedObituaryUI.Count)
			{
				//adds a button if necessary
				loadedObituaryUI.Add(Instantiate(obituaryEntryPrefab, verticalLayoutGroupRectTransform.transform).GetComponent<ObituaryEntryUI>());
			}
			loadedObituaryUI[i].DisplayWizardUI(wizardSaveDatas[i]);
		}
	}

	private void UpdateObituaryGroupingUI(int wizardSaveDataCount)
	{
		for (int i = 0; i < loadedObituaryUI.Count; i++)
		{
			if (i >= wizardSaveDataCount)
			{
				ObituaryEntryUI deletingButton = loadedObituaryUI[i];
				loadedObituaryUI.RemoveAt(i);
				Destroy(deletingButton.gameObject);
			}
		}
		if (wizardSaveDataCount >= 4)
		{
			Debug.Log("resizing wizard select panel/rect Transform.");
			float windowSizeY = (wizardSaveDataCount * 90) + 20;
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, windowSizeY);
			verticalLayoutGroupRectTransform.localPosition = new Vector3(18, -(windowSizeY / 2), 0);
			scrollRect.vertical = true;
		}
		else
		{
			Debug.Log("5 or less wizards. reverting to default size and disabling scroll.");
			verticalLayoutGroupRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, defaultRectVerticalSize);
			scrollRect.vertical = false;
			verticalLayoutGroupRectTransform.localPosition = defaultLocalPosition;
		}
	}
}
