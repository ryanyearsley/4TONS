using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveShotBehaviour : RangedAttackBehaviour
{
	[SerializeField]
	private float[] waveRotationOffsets;



	public override void OnTaskStart (SensoryInfo sensoryInfo) {
		if (sensoryInfo.vitalsEntity.resource != null) {
			sensoryInfo.vitalsEntity.resource.SubtractResourceCost (resourceCost);
		}
		foreach (float rotOffset in waveRotationOffsets) {
			float zRotation = sensoryInfo.lookTransform.rotation.eulerAngles.z + rotOffset;
			Quaternion shotRotation = Quaternion.Euler(0, 0, zRotation);
			PoolManager.instance.ReuseSpellObject (rangedProjectilePrefab, sensoryInfo.lookTransform.position, shotRotation, sensoryInfo.vitalsEntity);
		}
	}
}
