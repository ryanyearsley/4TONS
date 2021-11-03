using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This component goes on any object that the player can interact with.
public class InteractableObject : PoolObject
{
	public Sprite interactSprite;
	public Transform trans;

	public override void SetupObject () {
		trans = transform;
	}
	public virtual void HighlightInteractable() {

	}
	public virtual void UnhighlightInteractable () {
	}

	public virtual void InteractWithObject () {

	}
}
