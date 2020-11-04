using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitInfo
{
	public float damage;
	public Vector2 direction;
	public float healthRemaining;

	public OnHitInfo(float damage, Vector2 direction, float healthRemaining) {
		this.damage = damage;
		this.direction = direction;
		this.healthRemaining = healthRemaining;
	}
}
