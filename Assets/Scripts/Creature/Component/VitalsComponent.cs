using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class VitalsComponent : CreatureComponent {

    [SerializeField]
    protected float currentValue;
    protected float maxValue;

    [SerializeField]
    protected float overheadUIHeight = 1.25f;

    public GameObject overheadUIPrefab;
    private OverheadVitalsBarUI overheadUI;
	#region CreatureComponent Callbacks
	public override void SetUpComponent(GameObject rootObject) {
        base.SetUpComponent (rootObject);
        if (overheadUIPrefab != null) {
            GameObject go = Instantiate(overheadUIPrefab, rootObject.transform.position, rootObject.transform.rotation);
            go.transform.SetParent (rootObject.transform) ;
            overheadUI = go.GetComponent<OverheadVitalsBarUI> ();
            overheadUI.InitializeUI (overheadUIHeight);
        }
        maxValue = creatureObject.creatureData.maxHealth;
        currentValue = maxValue;
        ResetVitals ();
    }

    public float GetValue() {
        return currentValue;
	}
	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
        ResetVitals ();
	}

    public override void OnDeath () {
        base.OnDeath ();
        DisableVitals ();
    }
    #endregion
    public void ResetVitals() {
        if (overheadUI != null)
            overheadUI.gameObject.SetActive (true);
        currentValue = maxValue;
        UpdateVitalsBar ();
    }
    public void DisableVitals() {
        if (overheadUI != null)
            overheadUI.gameObject.SetActive (false);
    }
    public void UpdateVitalsBar () {
        if (overheadUI != null)
            overheadUI.UpdateVitalsBar (currentValue / maxValue);
    }


}
