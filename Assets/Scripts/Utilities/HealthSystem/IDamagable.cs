/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using System.Collections.Generic;
using System.Collections;

public interface IDamagable {

    void RegisterDamagableObject();
	void ApplyDamage(float damage);
    void Die ();
}