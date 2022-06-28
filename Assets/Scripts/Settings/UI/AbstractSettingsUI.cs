using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractSettingsUI : MonoBehaviour {

	private void Start () {
		InitializeUI ();
	}

	public virtual void InitializeUI () {
		LoadSettings ();
	}

	public virtual void LoadSettings() {

	}

	public virtual void ApplySettingsUpdate () {

	}

	public virtual void ResetDefaults() {

	}
}
