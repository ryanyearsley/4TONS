public class SpeedAlteringEffect {

	public float dps;
	public float mdps;
	public float timeRemaining;
	public float speedMultiplier;
	public bool canAttack;

	public SpeedAlteringEffect (float speedMultiplier, float timeRemaining, bool canCast) {
		this.speedMultiplier = speedMultiplier;
		this.timeRemaining = timeRemaining;
		this.canAttack = canCast;
	}
}