using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTasklistElement : MonoBehaviour
{
	public TutorialTask task;
	public Image checkbox;
	public TMP_Text taskText;
	public TMP_Text inputText;

	public void ReuseTasklistElement(TutorialTaskInfo taskInfo, string binding)
	{
		task = taskInfo.task;
		taskText.text = taskInfo.taskDescription;
		inputText.text = binding;
		checkbox.sprite = TutorialManager.instance.incompleteCheckboxSprite;
	}
	public void SetTutorialLevelComplete()
	{
		taskText.text = "Enter the portal";
		inputText.text = "F";
		checkbox.sprite = TutorialManager.instance.incompleteCheckboxSprite;
	}

	public void TrySetTaskComplete(TutorialTask tutorialTask)
	{
		Debug.Log("TutorialTaskListElement: Try set task complete");
		if (tutorialTask == task)
		{
			Debug.Log("TutorialTaskListElement: Corresponding task complete!");
			checkbox.sprite = TutorialManager.instance.completeCheckboxSprite;
		}
	}
}
