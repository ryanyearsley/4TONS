using UnityEngine;

[System.Serializable]
public class SpeedAlteringEffect {

	public float dps;
	public float mdps;
	public float effectTimer;
	public float effectTime;
	public float speedMultiplier;
	public bool canAttack;
	public bool isVelocityOverride;
	public Vector2 velocityOverride = Vector2.zero;

	public SpeedAlteringEffect (float speedMultiplier, float effectTime, bool canCast) {
		this.speedMultiplier = speedMultiplier;
		this.effectTime = effectTime;
		this.effectTimer = 0;
		this.canAttack = canCast;
	}
}