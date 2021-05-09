using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasResource
{
    void ApplyResourceDamage(float manaDamage);
    bool HasEnoughMana (float manaCost);
    void SubtractResourceCost(float manaCost);
    void RegenerateMana(float manaRegenAmount);
    void RegenerateResourcePerSecond();
}