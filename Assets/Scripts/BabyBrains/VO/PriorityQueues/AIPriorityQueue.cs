using System;
[Serializable]
public class AIPriorityQueue : BabyBrains.PriorityQueue<BabyBrainsBehaviour, int> { 
	public AIPriorityQueue(int priority) : base (priority) {
	}
}
