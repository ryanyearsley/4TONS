using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class StaggerTrailProjectileObject : ProjectileObject
{
	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private bool isBranch;

	[SerializeField]
	private bool lastTurnRight;

	private float currentRandomRotation;
	//-1 = left
	//0 = straight
	//1 = right

	[SerializeField]
	private float turnRange = 15;//in degrees


	[SerializeField]
	private float trailBreadcrumbInterval = 0.25f;

	[SerializeField]
	private int branchChance = 5;
	[SerializeField]
	private GameObject branchPrefab;

	private float tbcTimer;

	public override void SetupObject () {
		base.SetupObject ();
		if (!isBranch) {
			PoolManager.instance.CreateSpellObjectPool (branchPrefab, 6);
		}
	}

	public override void ReuseObject () {
		base.ReuseObject ();
		lineRenderer.positionCount = 0;
		HalfedFirstTurn ();
		tbcTimer = 0;

	}
	private void HalfedFirstTurn() {
		int directionRoll = Random.Range(0, 10);
		if (directionRoll > 5) {
			lastTurnRight = false;
			currentRandomRotation = (turnRange / 2) * -1f;
			AddLineRendererPointAndTurn (currentRandomRotation);
		} else {
			lastTurnRight = true;
			currentRandomRotation = (turnRange / 2);
			AddLineRendererPointAndTurn (currentRandomRotation);
		}
	}
	public override void Update () {
		base.Update ();
		lineRenderer.SetPosition (lineRenderer.positionCount - 1, trans.position);
		tbcTimer += Time.deltaTime;
		if (tbcTimer > trailBreadcrumbInterval) {
			ApplyTurn();
			tbcTimer = Random.Range (0, trailBreadcrumbInterval / 2);
		}
	}
	private void ApplyTurn() {
		if (lastTurnRight) {
			lastTurnRight = false;//turn left
			currentRandomRotation = Random.Range(turnRange * -1f, turnRange * -0.25f);
			AddLineRendererPointAndTurn (currentRandomRotation);
		} else {
			lastTurnRight = true;
			currentRandomRotation = Random.Range (turnRange * 0.25f, turnRange);
			AddLineRendererPointAndTurn (currentRandomRotation);
		}
	}
	private void AddLineRendererPointAndTurn(float turnDegrees) {


		if (!isBranch) {
			int branchRoll = Random.Range(0, 10);
			if (branchRoll < branchChance) {
				PoolManager.instance.ReuseSpellObject (branchPrefab, trans.position, trans.rotation, casterVitalsEntity);
			}
		}
		lineRenderer.positionCount++;
		lineRenderer.SetPosition (lineRenderer.positionCount - 1, trans.position);
		trans.Rotate (0, 0.0f, trans.rotation.z + turnDegrees, Space.World);
		lineRenderer.positionCount++;
		lineRenderer.SetPosition (lineRenderer.positionCount - 1, trans.position);

	}
}
