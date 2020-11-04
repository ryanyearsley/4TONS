﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffAimObject : MonoBehaviour
{
	public Transform staffTipTransform;
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	public void InitializeStaffAimObject (Sprite sprite) {
		spriteRenderer.sprite = sprite;
	}
}
