using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTransformHolder : MonoBehaviour
{

	#region Singleton
	public static WaypointTransformHolder instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	public Transform trans;
	private void Awake () {
		SingletonInitialization ();
		trans = transform;
	}
}