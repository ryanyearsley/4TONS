using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {



	private void Start () {
		MapGenerator.instance.GenerateMap ();
		foreach (Player player in PlayerManager.Instance.currentPlayers) {
			Vector2Int spawnCoordinate = MapGenerator.instance.spawnPoints.playerSpawnPoints[player.playerIndex];
			GameObject playerObject = Instantiate(ConstantsManager.instance.playerWizardTemplatePrefab);
			playerObject.GetComponent<PlayerInitializer> ().InitializePlayer (player);
			MapGenerator.instance.PlaceObjectOnGrid (playerObject.transform, spawnCoordinate);
			if (PlayerManager.Instance.currentPlayers.Count == 1) {
				PlayerManagement.CameraController.instance.SetCameraDynamic (playerObject.GetComponent<PlayerMovementController> ());
			}
		}
	}


}