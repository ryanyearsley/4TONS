using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HighlightTextOnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField]
	private Color highlightedColor = Color.white;

	[SerializeField]
	private Color normalColor = Color.gray;

	private TMP_Text buttonText;


	void Awake() {
		buttonText = GetComponentInChildren<TMP_Text> ();
		buttonText.color = normalColor;
	}
	void OnDisable() {
		buttonText.color = normalColor;
	}
	public void OnPointerEnter (PointerEventData eventData) {
		buttonText.color = highlightedColor;
	}

	public void OnPointerExit (PointerEventData eventData) {
		buttonText.color = normalColor;
	}

}
