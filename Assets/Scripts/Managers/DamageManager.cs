using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{

    public static DamageManager Instance { get; private set; }

   
    public Dictionary<int, IDamagable> damageableObjects = new Dictionary<int, IDamagable>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void RegisterDamagableObject(int objectId, IDamagable damagable)
    {
        damageableObjects.Add(objectId, damagable);
    }

    public void ApplyDamage(int objectId, float damage)
    {
        IDamagable target = damageableObjects[objectId];
        target.ApplyDamage(damage);
    }
    
}
