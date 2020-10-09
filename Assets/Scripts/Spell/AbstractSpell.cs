using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellCastLocation
{
    Staff, Cursor, Player, Melee
}

public abstract class AbstractSpell : MonoBehaviour
{
    //static
    public int manaCost;
    public float coolDown;
    public SpellCastLocation spellCastLocation;
    protected GameObject spellObject;
    protected int poolSize;

    //dynamic
    public bool onCooldown;
    protected float cdTimer;


    private void Start()
    {
        PoolManager.instance.CreatePool(spellObject, poolSize);
    }
    private void Update()
    {
        if (onCooldown)
        {
            cdTimer -= Time.deltaTime;
            if (cdTimer <= 0)
                onCooldown = false;
        }
    }


    public virtual void CastSpell()
    {
        if (!onCooldown )
        onCooldown = true;
        cdTimer = coolDown;
    }

    public virtual void ChannelSpell()
    {

    }
    public virtual void EndSpell()
    {

    }
}
