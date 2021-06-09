using System;
using UnityEngine;

public class AnimationHashID {

	public readonly string animationName;
	public readonly int hashId;
	public AnimationHashID (string animName) {
		this.animationName = animName;
		hashId = Animator.StringToHash (animName);
	}
}
