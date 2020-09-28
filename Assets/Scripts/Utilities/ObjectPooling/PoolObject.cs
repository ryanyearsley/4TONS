using UnityEngine;
using System.Collections;

public class PoolObject : MonoBehaviour {

	public virtual void SetupObject () {
	}

	//Virtual Reset function so it can be overriden in classes with specific needs
	public virtual void ResetObject () {
	}

	protected void Destroy (float time = 0) {
		if (time <= 0) {
			TerminateObjectFunctions ();
			gameObject.SetActive (false);
		} else
			StartCoroutine (DestroyAfter (time));
	}

	private IEnumerator DestroyAfter (float time) {
		yield return new WaitForSeconds (time);
		TerminateObjectFunctions ();
		gameObject.SetActive (false);
	}

	public virtual void TerminateObjectFunctions () {
	}
}