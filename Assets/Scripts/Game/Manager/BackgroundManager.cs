using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
	#region Singleton
	public static BackgroundManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	public Transform cameraTransform;
	public GameObject parallaxLayerPrefab;

	public List<ParallaxLayerObject> layerObjects = new List<ParallaxLayerObject>();

	void Awake () {
		SingletonInitialization ();
	}

	void Start () {
		cameraTransform = Camera.main.transform;
		if (GameManager.instance != null) {
			SetUpCameraBackground (GameManager.instance.gameContext.zoneData.backgroundGroup);
		}
	}

	void Update () {
		UpdateBackgroundLayers ();
	}

	public void SetUpCameraBackground (ParallaxBackgroundData backgroundGroup) {
		GameObject backgroundManagerObject = new GameObject ("BackgroundManager");
		backgroundManagerObject.transform.position = Vector3.forward * 10;
		foreach (LayerData layerData in backgroundGroup.layerDatas) {
			GameObject layerGo = Instantiate (parallaxLayerPrefab, backgroundManagerObject.transform);
			ParallaxLayerObject layerObject = layerGo.GetComponent<ParallaxLayerObject> ();
			layerObject.SetUpLayerObject (layerData);
			layerObjects.Add (layerObject);
		}
	}


	private void UpdateBackgroundLayers () {
		//Debug.Log ("Updating background layers");
		int layerOffset = 1;
		foreach (ParallaxLayerObject layerObject in layerObjects) {
			layerObject.UpdateLayerObject (cameraTransform.position, layerOffset);
			layerOffset++;
		}
	}


}
