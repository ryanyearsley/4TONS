public class DebuffInfo {

	public float timeRemaining;
	public float speedMultiplier;
	public bool canCast;

	public DebuffInfo (float timeRemaining, float speedMultiplier, bool canCast) {
		this.timeRemaining = timeRemaining;
		this.speedMultiplier = speedMultiplier;
		this.canCast = canCast;
	}
}