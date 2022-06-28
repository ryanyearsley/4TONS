using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class AbstractButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {

	public float requiredHoldTime = 1.5f;
	private float currentTimeHeld = 0f;
	public Image fillImage;

	private bool pointerDown = false;

	protected virtual void Awake () {
		ResetButton ();
	}

	public virtual void OnPointerDown (PointerEventData eventData) {
		ResetButton ();
		pointerDown = true;
	}

	public virtual void OnPointerExit (PointerEventData eventData) {
		ResetButton ();
	}
	public virtual void OnPointerUp (PointerEventData eventData) {
		ResetButton ();
	}

	public virtual void OnLongClick () {
		ResetButton ();
	}

	void Update() {
		if (pointerDown) {
			currentTimeHeld += Time.deltaTime;
			fillImage.fillAmount = currentTimeHeld / requiredHoldTime;
			if (currentTimeHeld > requiredHoldTime) {
				OnLongClick ();
			}
		}
	}

	private void ResetButton() {
		currentTimeHeld = 0;
		pointerDown = false;
		fillImage.fillAmount = 0;
	}
}

