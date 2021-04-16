using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellCastLocation
{
    Staff, Cursor, Player, Melee
}

public abstract class Spell : MonoBehaviour
{

    protected PlayerObject playerObject;

    //static
    public SpellData spellData; 

    //dynamic
    public bool onCooldown;

    protected Transform spellCastTransform;
    protected float cdTimer;
    public SpellUI spellUI;

	private void Start() {
        PoolManager.instance.CreateSpellObjectPool (spellData.spellObject, spellData.poolSize);
    }

    public void ConfigureSpellToPlayer(PlayerObject playerObject) {
        this.playerObject = playerObject;
        transform.parent = playerObject.transform;
        switch (spellData.spellCastLocation) {
            case SpellCastLocation.Staff:
                spellCastTransform = playerObject.creaturePositions.staffAimTransform;
                break;
            case SpellCastLocation.Cursor:
                spellCastTransform = playerObject.creaturePositions.targetTransform;
                break;
            case SpellCastLocation.Player:
                spellCastTransform = playerObject.creaturePositions.feetTransform;
                break;
        }
    }

    private void Update()
    {
        if (onCooldown) {
            cdTimer -= Time.deltaTime;

            if (cdTimer <= 0) {
                onCooldown = false;
            }

            if (spellUI != null) {
                float fillPercentage = 1 - cdTimer/spellData.coolDown;
                spellUI.UpdateSpellUICooldown (fillPercentage, cdTimer);
            }
        }
    }

    public void OnBindSpell() {

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
