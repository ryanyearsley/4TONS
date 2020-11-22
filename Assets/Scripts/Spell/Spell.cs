using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellCastLocation
{
    Staff, Cursor, Player, Melee
}

public abstract class Spell : MonoBehaviour
{
    //static
    public SpellData spellData; 

    //dynamic
    public bool onCooldown;

    protected Transform spellCastTransform;
    protected float cdTimer;


    private void Start()
    {
        PoolManager.instance.CreatePool(spellData.spellObject, spellData.poolSize);
        PlayerStateController stateController = GetComponentInParent<PlayerStateController>();
        switch (spellData.spellCastLocation) {
            case SpellCastLocation.Staff:
                spellCastTransform = stateController.creaturePositions.staffAimTransform;
                break;
            case SpellCastLocation.Cursor:
                spellCastTransform = stateController.creaturePositions.targetTransform;
                break;
            case SpellCastLocation.Player:
                spellCastTransform = stateController.creaturePositions.feetTransform;
                break;

        }
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

        Debug.Log ("Casting Spell");
        if (!onCooldown )
        onCooldown = true;
        cdTimer = spellData.coolDown;
    }

    public virtual void ChannelSpell()
    {

    }
    public virtual void EndSpell()
    {

    }
}
