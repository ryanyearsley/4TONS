using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitInfo
{
	public float damage;
	public Vector2 direction;
	public float healthRemaining;

	public HitInfo(float damage, Vector2 direction, float healthRemaining) {
		this.damage = damage;
		this.direction = direction;
		this.healthRemaining = healthRemaining;
	}
}
