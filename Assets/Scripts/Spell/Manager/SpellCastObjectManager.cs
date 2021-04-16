using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This manager is used solely for holding spellcast objects.
public class SpellCastObjectManager : MonoBehaviour
{
	#region Singleton
	public static SpellCastObjectManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	private void Awake () {
		SingletonInitialization ();
	}
}
