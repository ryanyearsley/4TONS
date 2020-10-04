using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalsManager : MonoBehaviour
{

    public static VitalsManager Instance { get; private set; }

   [SerializeField]
    public Dictionary<int, IDamageable> damageableObjects = new Dictionary<int, IDamageable>();
    public Dictionary<int, IHasMana> hasManaObjects = new Dictionary<int, IHasMana>();
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        InvokeRepeating("RegenerateManaPerSecond", 1, 1);
    }

    public void RegisterDamageableObject(int objectId, IDamageable damagable)
    {
        damageableObjects.Add(objectId, damagable);
    }
    public void DeregisterDamageableObject(int objectId)
    {
        damageableObjects.Remove(objectId);
    }
    public void RegisterHasManaObject(int objectId, IHasMana hasMana)
    {
        hasManaObjects.Add(objectId, hasMana);
    }
    public void DeregisterHasManaObject(int objectId)
    {
        hasManaObjects.Remove(objectId);
    }

    public void ApplyDamage(int objectId, float damage)
    {
        Debug.Log("Attempting Apply dmg");
        if (damageableObjects.ContainsKey(objectId))
        {
            Debug.Log("Found key! Apply dmg");
            IDamageable target = damageableObjects[objectId];
            target.ApplyDamage(damage);
        }
    }
    public void Heal(int objectId, float healAmount)
    {
        if (damageableObjects.ContainsKey(objectId))
        {
            IDamageable target = damageableObjects[objectId];
            target.Heal(healAmount);
        }
    }

    public void ApplyManaDamage(int objectId, float manaDamage)
    {
        if (hasManaObjects.ContainsKey(objectId))
        {
            IHasMana target = hasManaObjects[objectId];
            target.ApplyManaDamage(manaDamage);
        }
    }

    public bool SubtractManaCost(int objectId, float manaCost)
    {
        if (hasManaObjects.ContainsKey(objectId))
        {
            IHasMana target = hasManaObjects[objectId];
            return target.SubtractManaCost(manaCost);
        }
        return false;
    }

    public void RegenerateMana(int objectId, float manaRegenAmount)
    {
        if (hasManaObjects.ContainsKey(objectId))
        {
            IHasMana target = hasManaObjects[objectId];
            target.RegenerateMana(manaRegenAmount);
        }
    }
    public void RegenerateManaPerSecond()
    {
        foreach (KeyValuePair<int, IHasMana> manaEntity in hasManaObjects)
        {
            manaEntity.Value.RegenerateManaPerSecond();
        }
    }

}
