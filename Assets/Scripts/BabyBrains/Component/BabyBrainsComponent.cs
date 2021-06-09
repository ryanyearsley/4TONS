using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyBrainsComponent : CreatureComponent, IBabyBrainsComponent
{
	protected BabyBrainsObject babyBrainsObject;

	public override void SetUpComponent (GameObject rootObject) {
		babyBrainsObject = rootObject.GetComponent<BabyBrainsObject> ();
		base.SetUpComponent (rootObject);
	}
	public override void SubscribeToCreatureEvents () {
		Debug.Log ("baby brains subscribing to events.");
		base.SubscribeToCreatureEvents ();
		babyBrainsObject.OnChangeBabyBrainsEvent += OnChangeBabyBrainsState;
	}

	public override void UnsubscribeFromCreatureEvents () {
		base.UnsubscribeFromCreatureEvents ();
		babyBrainsObject.OnChangeBabyBrainsEvent -= OnChangeBabyBrainsState;

	}
	public virtual void OnChangeBabyBrainsState (BabyBrainsState enemyState) {

	}
}
