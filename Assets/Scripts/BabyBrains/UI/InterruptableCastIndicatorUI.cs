using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterruptableCastIndicatorUI : MonoBehaviour {

	[SerializeField]
	private GameObject indicatorObject;
	[SerializeField]
	private Transform maskTransform;

	[SerializeField]
	private float castCompleteYValue = 0.45f;

	private float yValuePerPercent;

	private void Awake () {
		SetUpCastIndicator ();
		HideCastIndicator ();
	}
	public void SetUpCastIndicator () {
		ResetCastIndicator ();
		yValuePerPercent = castCompleteYValue / 100;
	}
	public void HideCastIndicator() {
		indicatorObject.SetActive (false);
	}
	public void ShowCastIndicator () {
		indicatorObject.SetActive (true);
	}
	public void ResetCastIndicator() {
		maskTransform.localPosition = Vector3.zero;
	}

	public void UpdateCastIndicator(float percentageFill) {
		maskTransform.localPosition = new Vector3 (0, yValuePerPercent * percentageFill, 0);
	}

}
