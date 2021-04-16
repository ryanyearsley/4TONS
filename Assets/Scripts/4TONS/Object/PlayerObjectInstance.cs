using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectInstance : ObjectInstance
{
	private PlayerObject playerObject;
	public PlayerObjectInstance(GameObject obj, Transform parentTransform) : base (obj, parentTransform) {

		if (obj.GetComponent<PlayerObject> () != null) {
			playerObject = obj.GetComponent<PlayerObject> ();
		}
	}
	public void Reuse (Vector3 position, Player player) {
		playerObject.name = "Player" + "_" + player.playerIndex + "_" + player.wizardSaveData.wizardName;
		playerObject.tag = "Player" + (player.playerIndex + 1);
		Transform[] childTransforms = playerObject.GetComponentsInChildren<Transform>();
		for (int i = 0; i < childTransforms.Length; i++) {
			childTransforms [i].tag = playerObject.tag;
		}
		player.currentPlayerObject = playerObject.GetComponent<PlayerObject> ();

		// Reset the object as specified within it's own class and the PoolObject class

		// Move to desired position then set it active
		go.transform.position = position;
		go.transform.rotation = Quaternion.identity;
		go.transform.parent = null;
		go.SetActive (true);

		if (playerObject != null) {
			playerObject.ReusePlayerObject (player);
			playerObject.ReuseObject ();
		}
	}
}
