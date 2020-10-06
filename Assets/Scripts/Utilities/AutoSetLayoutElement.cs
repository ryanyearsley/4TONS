using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent (typeof (LayoutElement))]
public class AutoSetLayoutElement : MonoBehaviour {

	[SerializeField, Tooltip ("None if equal to 0.")]
	private float preferredHeightScaleFactor = 15f;
	[SerializeField, Tooltip ("None if equal to 0.")]
	private float preferredWidthScaleFactor = 0f;

	[SerializeField, HideInInspector]
	private float screenHeight;
	[SerializeField, HideInInspector]
	private float screenWidth;

	private LayoutElement layoutElement;

	private void Start () {
		UpdateLayoutElement ();
	}

	public void UpdateLayoutElement () {
		screenHeight = Screen.height;
		screenWidth = Screen.width;
		if (layoutElement == null)
			layoutElement = GetComponent<LayoutElement> ();

		if (preferredHeightScaleFactor != 0) {
			float preferredHeight = Screen.height / preferredHeightScaleFactor;
			layoutElement.preferredHeight = preferredHeight;
		}

		if (preferredWidthScaleFactor != 0) {
			float preferredWidth = Screen.width / preferredWidthScaleFactor;
			layoutElement.preferredWidth = preferredWidth;
		}
	}

	private void Update () {
		if (Screen.height != screenHeight || Screen.width != screenWidth)
			UpdateLayoutElement ();
	}
}