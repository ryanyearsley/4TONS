using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AbstractButtonToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler// required interface when using the OnPointerEnter method.
{
	[TextArea(5, 5)]
	[SerializeField]
	private string toolTipString;
	[SerializeField]
	private TextMeshProUGUI toolTipText;
	void Start () {
		toolTipText.text = string.Empty;
	}
	//Do this when the cursor enters the rect area of this selectable UI object.
	public void OnPointerEnter (PointerEventData eventData) {
		toolTipText.text = toolTipString;
	}
	public void OnPointerExit (PointerEventData eventData) {
		toolTipText.text = string.Empty;
	}
	public void OnPointerClick (PointerEventData eventData) {
		toolTipText.text = string.Empty;
	}
}
