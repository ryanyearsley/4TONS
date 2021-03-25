using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VitalsEntity {
    public CreatureData creatureData;
    public Transform trans;
    public CreatureObject creatureObject;
    public Collider2D[] colliders;
    public string tag;
    [SerializeField]
    public HealthComponent health;
    [SerializeField]
    public ResourceComponent resource;
    [SerializeField]
    public MovementComponent movement;

    public VitalsEntity(GameObject go) {
        trans = go.transform;
        creatureObject = go.GetComponent<CreatureObject>();
        colliders = go.GetComponentsInChildren<Collider2D> ();
        tag = go.tag;
        if (go.TryGetComponent<HealthComponent> (out HealthComponent healthController)) {
            health = healthController;
        }
        if (go.TryGetComponent<ResourceComponent> (out ResourceComponent manaController)) {
            resource = manaController;
        }
        if (go.TryGetComponent<MovementComponent> (out MovementComponent movementController)) {
            movement = movementController;
        }
    }

    public void EnableVitals() {

        VitalsManager.Instance.RegisterVitals (this);
        VitalsManager.Instance.DeclareAllegiance (this);
        for (int i = 0; i < colliders.Length; i++) {
            colliders [i].enabled = true;
        }
    }
    public void DisableVitals() {
        VitalsManager.Instance.DeregisterVitals (this);
        health.DisableVitals ();
        if (resource != null)
            resource.DisableVitals ();
        for (int i = 0; i < colliders.Length; i++) {
            colliders [i].enabled = false;
		}
    }

}
