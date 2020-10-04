using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasMana : IVital
{
    void ApplyManaDamage(float manaDamage);
    bool SubtractManaCost(float manaCost);
    void RegenerateMana(float manaRegenAmount);
    void RegenerateManaPerSecond();
}