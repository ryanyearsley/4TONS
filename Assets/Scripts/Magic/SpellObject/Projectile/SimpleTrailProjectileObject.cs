using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrailProjectileObject : ProjectileObject
{
	[SerializeField]
	private LineRenderer lineRenderer;

	public override void ReuseObject () {
		base.ReuseObject ();
		lineRenderer.positionCount = 2;
		lineRenderer.SetPosition (0, trans.position);
		lineRenderer.SetPosition (1, trans.position);
	}
}
