using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePositions {

	public Transform cursorTransform;
	public Transform feetTransform;
	public Transform staffTransform;

	public CreaturePositions (Transform cursorTransform, Transform feetTransform, Transform staffTransform) {
		this.cursorTransform = cursorTransform;
		this.feetTransform = feetTransform;
		this.staffTransform = staffTransform;
	}
}