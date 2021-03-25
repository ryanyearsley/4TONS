using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "BehaviourData", menuName = "ScriptableObjects/BabyBrains/Behaviour Priority Data", order = 2)]

public class BehaviourPriorityData : ScriptableObject {

	public BehaviourData[] movementBehaviourWeights;

	public BehaviourData[] abilityBehaviourWeights;

	public void AssignBehaviourWeights() {
		UpdateWeights (movementBehaviourWeights, 1);
		UpdateWeights (abilityBehaviourWeights, movementBehaviourWeights.Length + 1);
	}

	public void UpdateWeights(BehaviourData[] behaviourWeights, int offset) {

		for (int i = 0; i < behaviourWeights.Length; i++) {
			BehaviourData behaviourData = behaviourWeights[i];
			if (behaviourData != null) {
				behaviourData.Weight = i + offset;
			}
		}
	}
}
