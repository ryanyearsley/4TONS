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
    public int manaCost;
    public float coolDown;
    public SpellCastLocation spellCastLocation;
    public Transform spellCastTransform;
    public GameObject spellObject;
    [SerializeField]
    private int poolSize;

    //dynamic
    public bool onCooldown;
    protected float cdTimer;


    private void Start()
    {
        Debug.Log ("Start Spell");
        PoolManager.instance.CreatePool(spellObject, poolSize);
        PlayerStateController stateController = transform.root.GetComponent<PlayerStateController>();
        switch (spellCastLocation) {
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
        cdTimer = coolDown;
    }

    public virtual void ChannelSpell()
    {

    }
    public virtual void EndSpell()
    {

    }
}
