using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayerObject : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	private LayerData layerData;


	private Transform repeatSpriteTransform;
	private bool repeatSpriteOnRight;
	public void SetUpLayerObject(LayerData layerData) {
		this.layerData = layerData;
		spriteRenderer.sprite = layerData.sprite;
		spriteRenderer.color = new Color (1, 1, 1, layerData.opacity);
		spriteRenderer.transform.localPosition = layerData.spriteOffset;
		if (layerData.distanceScaleX < 0.8) {
			GameObject repeatLayerGo = new GameObject("RepeatSpriteGO");
			repeatSpriteTransform = repeatLayerGo.transform;
			repeatLayerGo.transform.parent = this.transform;
			SpriteRenderer repeatSprite = repeatLayerGo.AddComponent<SpriteRenderer>();
			repeatSprite.sprite = layerData.sprite;
			repeatSprite.color = new Color (1, 1, 1, layerData.opacity);
			repeatSpriteTransform.localPosition = layerData.spriteOffset;
			repeatSpriteTransform.localPosition = Vector2.left * 32 + layerData.spriteOffset;
		}
	}
	public void UpdateLayerObject (Vector3 cameraPosition, int layerOffset) {
		Vector3 parallaxOffset = new Vector3 (cameraPosition.x * layerData.distanceScaleX, cameraPosition.y * layerData.distanceScaleY, 0);
		transform.position = parallaxOffset + Vector3.forward * layerOffset;
		if (repeatSpriteTransform != null) {

			if (repeatSpriteOnRight && cameraPosition.x < 0) {
				repeatSpriteOnRight = false;
				repeatSpriteTransform.localPosition = Vector2.left * 32 + layerData.spriteOffset;
			} else if (!repeatSpriteOnRight && cameraPosition.x > 0) {
				repeatSpriteOnRight = true;
				repeatSpriteTransform.localPosition = Vector2.right * 32 + layerData.spriteOffset;
			}
		}
	}

	private void UpdateRepeatSprite () {
		
	}
}
