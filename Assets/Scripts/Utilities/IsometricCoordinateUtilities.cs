using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IsometricCoordinateUtils {
	static Vector2 isometricScaling = new Vector2 (1f, 0.5f);

	//*************************** GENERAL ******************************

	//ROUNDS A VECTOR2 
	public static Vector2 RoundCoordinate (this Vector2 coord) {
		return new Vector2 (Mathf.RoundToInt (coord.x), Mathf.RoundToInt (coord.y));
	}
	public static Vector3 RoundCoordinateV3 (this Vector2 coord) {
		return new Vector2 (Mathf.RoundToInt (coord.x), Mathf.RoundToInt (coord.y));
	}
	//This is used to convert a scene coordinate to isometric coordinate.
	public static Vector2 TranslateSceneToIso (this Vector3 pos) {
		Vector2 tileExtents = new Vector2(0.5f, 0.25f);
		float x = (pos.x / tileExtents.x + pos.y / tileExtents.y) * 0.5f;
		float y = (pos.y / tileExtents.y - pos.x / tileExtents.x) * 0.5f;
		return new Vector2 (x, y);
	}
	public static Vector3 TranslateIsoToScene (this Vector2 isoPos) {
		Vector2 tileExtents = new Vector2(0.5f, 0.25f);
		float x = tileExtents.x * (isoPos.x - isoPos.y);
		float y = tileExtents.y * (isoPos.x + isoPos.y);
		return new Vector3 (x, y, 0);
	}
	//Used to convert scene coordinate to nearest tile center 
	public static Vector2 RoundSceneToNearestIso (this Vector3 trans) {
		Vector2 convertedCoords = TranslateSceneToIso(trans);
		Vector2 roundedTrans = new Vector3(Mathf.RoundToInt(convertedCoords.x), Mathf.RoundToInt(convertedCoords.y), 0);
		return TranslateIsoToScene (roundedTrans);

	}

	public static Vector3 TranslateCartToIso (this Vector2 cart) {
		cart /= 2;
		float x = cart.x - cart.y;
		float y = (cart.x + cart.y) / 2;
		return new Vector3 (x, y, 0);
	}
	//************************** SPELLGEM EQUATIONS ******************************
	//Update entire array of spellgem blocks at once.

	public static void RotateCoordinates (Vector2 [] cartCoord, Vector3 [] isoCoord) {
		for (int i = 0; i < cartCoord.Length; i++) {
			cartCoord [i] = Quaternion.Euler (0, 0, 90) * cartCoord [i];
			isoCoord [i] = TranslateCartToIso (cartCoord [i]);
		}
	}
	public static void MirrorCoordinate (Vector2 [] cartCoord, Vector3 [] isoCoord) {
		for (int i = 0; i < cartCoord.Length; i++) {
			cartCoord [i] = new Vector2 (cartCoord [i].x * -1, cartCoord [i].y);
			isoCoord [i] = TranslateCartToIso (cartCoord [i]);
		}
	}
	public static void UpdateBlockLocalPositions (Vector2 [] cartCoord, Vector3 [] blockLocalPosition) {
		for (int i = 0; i < cartCoord.Length; i++) {
			blockLocalPosition [i] = TranslateCartToIso (cartCoord [i]);
		}
	}

	public static float IsoDistanceBetweenPoints (Transform pointA, Transform pointB) {
		return Vector2.Distance (pointA.position * isometricScaling, pointB.position * isometricScaling);


	}
	public static float RawDistanceBetweenPoints (Transform pointA, Transform pointB) {
		return Vector2.Distance (pointA.position, pointB.position);
	}
}
