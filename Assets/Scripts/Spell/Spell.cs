﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellCastLocation
{
    Staff, Cursor, Player, Melee
}

public abstract class Spell : MonoBehaviour
{

    protected PlayerObject stateController;

    //static
    public SpellData spellData; 

    //dynamic
    public bool onCooldown;

    protected Transform spellCastTransform;
    protected float cdTimer;
    public SpellUI spellUI;

	private void Start() {
        stateController = GetComponentInParent<PlayerObject> ();
        PoolManager.instance.CreateSpellObjectPool(spellData.spellObject, spellData.poolSize);
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
            float fillPercentage = 1 - cdTimer/spellData.coolDown;
            spellUI.UpdateSpellUICooldown (fillPercentage, cdTimer);
            cdTimer -= Time.deltaTime;
            if (cdTimer <= 0) {
                onCooldown = false;
                spellUI.ActivateSpellUI ();
            }
        }
    }

    public virtual bool isCastEligible () {
        if (onCooldown)
            return false;
        return true;
    }

    public virtual void CastSpell()
    {
            onCooldown = true;
            cdTimer = spellData.coolDown;
            spellUI.GreyOutSpellUI ();
    }

    public virtual void ChannelSpell()
    {

    }
    public virtual void EndSpell()
    {

    }
}
