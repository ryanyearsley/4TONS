using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffAimObject : MonoBehaviour
{
	public Transform staffTipTransform;
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	public void InitializeStaffAimObject (Sprite sprite) {
		spriteRenderer.sprite = sprite;
		staffTipTransform = new GameObject ("StaffTip").transform;
		staffTipTransform.parent = this.transform;
		staffTipTransform.localPosition = Vector3.up * 0.75f;
	}
}
