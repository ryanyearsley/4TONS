using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : SpellObject
{
    [SerializeField]
    private float moveSpeed;

    public virtual void FixedUpdate()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile Colliding with: " + other.gameObject.name);
        if (other.tag != this.tag)
        {
            VitalsManager.Instance.ApplyDamage(other.gameObject.GetInstanceID(), spellObjectData.damage);
        }
    }

}
