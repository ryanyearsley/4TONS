
using UnityEngine;

public class SoundObjectInstance : ObjectInstance {
	private SoundObject soundObject;
	public SoundObjectInstance (GameObject obj, Transform parentTransform) : base (obj, parentTransform) {

		if (obj.GetComponent<SoundObject> () != null) {
			soundObject = obj.GetComponent<SoundObject> ();
		}
	}
	public void Reuse (Vector3 position, Sound sound) {
		// Move to desired position then set it active
		go.transform.position = position;
		go.transform.rotation = Quaternion.identity;
		go.transform.parent = null;
		go.SetActive (true);

		if (soundObject != null) {
			soundObject.ReuseSoundObject (sound);
			soundObject.ReuseObject ();
		}
	}
}