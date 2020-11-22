using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{
    public static VitalsManager Instance { get; private set; }

    public Dictionary<int, VitalsEntity> vitalsObjects = new Dictionary<int, VitalsEntity>();
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public VitalsEntity GetObjectByID(int objectId) {
        if (vitalsObjects.ContainsKey (objectId)) {
            return vitalsObjects [objectId];
        } else return null;
	}

    public void DeregisterVitalsObject(int objectId)
    {
        vitalsObjects.Remove(objectId);
    }
    public void ApplyDamage(int objectId, float damage)
    {
        if (vitalsObjects.ContainsKey(objectId) && vitalsObjects[objectId].iDamageable != null)
        {
            vitalsObjects[objectId].iDamageable.ApplyDamage(damage);
        }
    }
    public void Heal(int objectId, float healAmount)
    {
        if (vitalsObjects.ContainsKey(objectId) && vitalsObjects[objectId].iDamageable != null)
        {
            vitalsObjects[objectId].iDamageable.Heal(healAmount);
        }
    }


    public void ApplyManaDamage(int objectId, float manaDamage)
    {
        if (vitalsObjects.ContainsKey(objectId) && vitalsObjects[objectId].iHasMana != null)
        {
            vitalsObjects[objectId].iHasMana.ApplyManaDamage(manaDamage);
        }
    }

    public bool SubtractManaCost(int objectId, float manaCost)
    {

        if (vitalsObjects.ContainsKey(objectId) && vitalsObjects[objectId].iHasMana != null)
        {
           return vitalsObjects[objectId].iHasMana.SubtractManaCost(manaCost);
        }
        return false;
    }

    public void RegenerateMana(int objectId, float manaRegenAmount)
    {
        if (vitalsObjects.ContainsKey(objectId) && vitalsObjects[objectId].iHasMana != null)
        {
            vitalsObjects[objectId].iHasMana.RegenerateMana(manaRegenAmount);
        }
    }

}
