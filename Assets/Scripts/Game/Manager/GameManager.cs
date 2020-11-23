using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Calls to...
 * MapGenerator (Builds level based on map data)
 * ConstantsManager (to build playerObject)
 * PlayerManager (takes currentPlayers for spawning playerObjects)
 */
public class GameManager : MonoBehaviour {

	private void Start () {
		MapGenerator.instance.GenerateMap ();
		foreach (Player player in PlayerManager.Instance.currentPlayers) {
			Vector2Int spawnCoordinate = MapGenerator.instance.spawnPoints.playerSpawnPoints[player.playerIndex];
			GameObject playerObject = Instantiate(ConstantsManager.instance.playerWizardTemplatePrefab);
			playerObject.name = "Player" + player.playerIndex + "_" + player.currentWizard.wizardName;
			player.currentPlayerStateController = playerObject.GetComponent<PlayerStateController> ();
			playerObject.GetComponent<PlayerInitializer> ().InitializePlayer (player);
			MapGenerator.instance.PlaceObjectOnGrid (playerObject.transform, spawnCoordinate);
			UIManager.Instance.playerUIs [player.playerIndex].InitializePlayerUI (player);
			if (PlayerManager.Instance.currentPlayers.Count == 1) {
				PlayerManagement.CameraController.instance.SetCameraDynamic (playerObject.GetComponent<PlayerMovementController> ());
			}
		}
	}


}