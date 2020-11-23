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
    public SpellUI spellUI;
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
        if (onCooldown) {
            spellUI.UpdateSpellUICooldown (cdTimer/spellData.coolDown, cdTimer);
            cdTimer += Time.deltaTime;
            if (cdTimer >= spellData.coolDown) {
                onCooldown = false;
                spellUI.ActivateSpellUI ();
            }
        }
    }


    public virtual void CastSpell()
    {
        Debug.Log ("Casting Spell");
        if (!onCooldown) {
            onCooldown = true;
            cdTimer = 0;
            spellUI.GreyOutSpellUI ();
        }
    }

    public virtual void ChannelSpell()
    {

    }
    public virtual void EndSpell()
    {

    }
}
