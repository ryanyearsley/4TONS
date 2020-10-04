/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections.Generic;
using System.Collections;

public interface IDamageable {

    void ApplyDamage(float damage);
    void Heal(float healAmount);
    void Die();
}