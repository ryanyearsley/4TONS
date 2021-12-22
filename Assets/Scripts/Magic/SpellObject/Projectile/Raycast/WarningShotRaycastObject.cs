using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningShotRaycastObject : RaycastObject {
	[SerializeField]
	private float warningTime;

	[SerializeField]
	protected LayerMask warningMask;
	[SerializeField]
	private Material warningLineMat;

	[SerializeField]
	private Material damageLineMat;

	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		casterVitalsEntity = vitalsEntity;
		if (spellObjectSound.singleClip != null)
			AudioManager.instance.PlaySound (spellObjectSound.clipName);
		lifeTimer = 0;
		isAlive = true;
		float randomRotation = Random.Range(-accuracy, accuracy);
		trans.Rotate (0, 0.0f, trans.rotation.z + randomRotation, Space.World);
		StartCoroutine(WarningShotRaycastRoutine ());
	}

	public IEnumerator WarningShotRaycastRoutine () {
		lineRenderer.material = warningLineMat; 
		RaycastHit2D rayHit = Physics2D.Raycast(trans.position, trans.right, raycastDistance, warningMask);
		lineRenderer.SetPosition (0, trans.position);
		lineRenderer.SetPosition (1, rayHit.point);
		yield return new WaitForSeconds (warningTime);
		lineRenderer.material = damageLineMat;
		RaycastProcedure ();
	}
}
