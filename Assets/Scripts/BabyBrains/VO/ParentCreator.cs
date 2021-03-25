using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//in BabyBrains, this represents the parent creature/object
public class ParentCreator {
	public ParentCreatorType parentCreatorType;
	public Transform parentTransform;
	public VitalsEntity parentVitals;

	public ParentCreator (ParentCreatorType parentCreatorType, Transform parentTransform) {
		this.parentCreatorType = parentCreatorType;
		this.parentTransform = parentTransform;
		if (this.parentCreatorType == ParentCreatorType.PLAYER || this.parentCreatorType == ParentCreatorType.AI) {
			parentVitals = parentTransform.GetComponent<CreatureObject> ().vitalsEntity;
		}
	}

}

public enum ParentCreatorType {
	LEVEL_MANAGER, PLAYER, AI
}
