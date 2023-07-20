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

	[SerializeField]
	private Sound interruptableSpellSound;
	[SerializeField]
	private AudioSource source;

	private void Awake () {
		SetUpCastIndicator ();
		HideCastIndicator ();
	}

	public void SetUpCastIndicator()
	{
		ResetCastIndicator();
		yValuePerPercent = castCompleteYValue / 100;
		if (interruptableSpellSound.singleClip != null)
			source.clip = interruptableSpellSound.singleClip;
	}
	public void HideCastIndicator() {
		indicatorObject.SetActive (false);
			if (source.clip != null)
				source.Stop();
	}
	public void ShowCastIndicator () {
		indicatorObject.SetActive (true);
		if (source.clip != null)
			source.Play();
	}
	public void ResetCastIndicator() {
		maskTransform.localPosition = Vector3.zero;
	}

	public void UpdateCastIndicator(float percentageFill) {
		maskTransform.localPosition = new Vector3 (0, yValuePerPercent * percentageFill, 0);
	}

}
