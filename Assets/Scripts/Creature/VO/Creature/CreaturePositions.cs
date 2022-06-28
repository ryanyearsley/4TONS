using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePositions {

	public Transform targetTransform;
	public Transform feetTransform;
	public Transform centerTransform;
	public Transform staffAimTransform;

	public CreaturePositions (Transform targetTransform, Transform feetTransform, Transform centerTransform, Transform staffAimTransform) {
		this.targetTransform = targetTransform;
		this.feetTransform = feetTransform;
		this.centerTransform = centerTransform;
		this.staffAimTransform = staffAimTransform;
	}
}