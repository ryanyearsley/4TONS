using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleProjectileObject : ProjectileObject
{
	private float moveSpeedModifier;
	[SerializeField]
	private float moveSpeedVariation = 2f;

	[SerializeField]
	private float spreadRange = 10f;



	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Sprite[] sprites;

	public override void SetupObject () {
		base.SetupObject ();
		
	}
	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		int randomSpriteIndex = Random.Range(0, sprites.Length - 1);
		spriteRenderer.sprite = sprites [randomSpriteIndex];

		moveSpeedModifier = Random.Range (-moveSpeedVariation, moveSpeedVariation);
		CalculateRealMoveSpeed ();

		float randomRotation = Random.Range(-spreadRange, spreadRange);
		trans.Rotate (0, 0.0f, trans.rotation.z + randomRotation, Space.World);
	}
	public override float CalculateRealMoveSpeed () {
		return (moveSpeedModifier + base.CalculateRealMoveSpeed());
	}

}

