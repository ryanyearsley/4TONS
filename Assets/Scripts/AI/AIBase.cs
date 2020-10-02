using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : AStarUnit {

	public SpriteRenderer spriteRenderer;

	private Animator animator;

	protected override void Awake () {
		base.Awake ();
		animator = GetComponent<Animator> ();
	}
}