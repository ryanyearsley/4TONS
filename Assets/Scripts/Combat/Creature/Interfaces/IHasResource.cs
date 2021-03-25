using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasResource
{
    void ApplyResourceDamage(float manaDamage);
    bool SubtractResourceCost(float manaCost);
    void RegenerateMana(float manaRegenAmount);
    void RegenerateResourcePerSecond();
}