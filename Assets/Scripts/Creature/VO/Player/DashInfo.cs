using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashInfo
{
	public float invulnerableTime;
	public float animationTime;
	public float dashSpeedMultiplier;
	public float cooldown;

	public DashInfo (float invulnerableTime, float animationTime, float dashSpeedMultiplier, float cooldown) {
		this.invulnerableTime = invulnerableTime;
		this.animationTime = animationTime;
		this.dashSpeedMultiplier = dashSpeedMultiplier;
		this.cooldown = cooldown;
	}
}
