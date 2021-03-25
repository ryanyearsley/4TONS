using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePositions {

	public Transform targetTransform;
	public Transform feetTransform;
	public Transform staffAimTransform;

	public CreaturePositions (Transform targetTransform, Transform feetTransform, Transform staffAimTransform) {
		this.targetTransform = targetTransform;
		this.feetTransform = feetTransform;
		this.staffAimTransform = staffAimTransform;
	}
}