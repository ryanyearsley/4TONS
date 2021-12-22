using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICreatureComponent {
	void SetUpComponent (GameObject rootObject);

	void SubscribeToCreatureEvents ();//before OnSpawn
	void UnsubscribeFromCreatureEvents ();//OnDeath

	void SubscribeToGameEvents ();//before OnSpawn
	void UnsubscribeFromGameEvents ();//After Terminate

	//Life Cycle
	void OnSpawn (Vector3 position);
	void OnDeath ();

	void OnHit (HitInfo hitInfo);
	void OnAttack (AttackInfo attackInfo);
	void OnAddDebuff (SpeedAlteringEffect debuffInfo);

}
