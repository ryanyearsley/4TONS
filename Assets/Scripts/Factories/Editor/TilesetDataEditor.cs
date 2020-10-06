using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor (typeof (TilesetData))]
public class TilesetDataEditor : Editor {

	private SerializedProperty tilePrefabs;
	private ReorderableList list;
	private TilesetData tilesetData;

	private void OnEnable () {
		tilePrefabs = serializedObject.FindProperty ("tilePrefabs");
		tilesetData = (TilesetData)target;
		list = new ReorderableList (serializedObject, tilePrefabs, true, true, true, true);
		list.drawHeaderCallback = DrawHeaderCallback;
		list.drawElementCallback = DrawElementCallback;
		list.elementHeightCallback += ElementHeightCallback;
		list.onAddCallback += OnAddCallback;
	}

	public override void OnInspectorGUI () {
		serializedObject.Update ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();
	}

	public void DrawHeaderCallback (Rect rect) {
		EditorGUI.LabelField (rect, "Tiles");
	}

	public void DrawElementCallback (Rect rect, int index, bool isActive, bool isFocusted) {
		SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex (index);
		rect.y += 2;

		GameObject prefab = tilesetData.tilePrefabs[index];
		string elementTitle = (prefab == null) ? "New Entry" : prefab.name;
		EditorGUI.PropertyField (
			new Rect (rect.x += 10, rect.y, Screen.width * 0.8f, height: EditorGUIUtility.singleLineHeight),
			element, new GUIContent (elementTitle));

		EditorGUI.LabelField (
			new Rect (rect.x += 10,
			rect.y += EditorGUIUtility.singleLineHeight,
			Screen.width * 0.8f,
			EditorGUIUtility.singleLineHeight),
			"CSV Index: " + (index + 1));

		if (prefab != null) {
			TileObject tileObject = prefab.GetComponent<TileObject> ();
			if (tileObject != null) {
				Texture2D texture = tileObject.spriteRenderer.sprite.texture;
				EditorGUI.DrawPreviewTexture (
					new Rect (
						rect.x += Screen.width - 200,
						rect.y += EditorGUIUtility.singleLineHeight,
						100,
						100),
					texture);
			} else {
				Debug.Log ("The Tile (" + prefab.name + ") is missing the TileObject.cs component\n" + prefab);
			}
		}
	}

	public float ElementHeightCallback (int index) {
		float propertyHeight = EditorGUI.GetPropertyHeight (list.serializedProperty.GetArrayElementAtIndex (index), false);
		float spacing = EditorGUIUtility.singleLineHeight / 2;
		return propertyHeight * spacing;
	}

	public void OnAddCallback (ReorderableList list) {
		var index = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;
		list.index = index;
		var element = list.serializedProperty.GetArrayElementAtIndex (index);
	}
}