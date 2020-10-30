using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor (typeof (TilesetData))]
public class TilesetDataEditor : Editor {

	private SerializedProperty tilePrefabs;
	private SerializedProperty setPiecePrefabs;
	private ReorderableList tileList;
	private ReorderableList setPieceList;
	private TilesetData tilesetData;

	private bool tileListFoldout;
	private bool setPieceListFoldout;

	private void OnEnable () {
		tilesetData = (TilesetData)target;
		tilePrefabs = serializedObject.FindProperty ("tilePrefabs");
		tileList = new ReorderableList (serializedObject, tilePrefabs, true, true, true, true);
		tileList.drawHeaderCallback = DrawTileHeaderCallback;
		tileList.drawElementCallback = DrawTileElementCallback;
		tileList.elementHeightCallback += TileElementHeightCallback;
		tileList.onAddCallback += OnAddCallback;

		setPiecePrefabs = serializedObject.FindProperty ("setPiecePrefabs");
		setPieceList = new ReorderableList (serializedObject, setPiecePrefabs, true, true, true, true);
		setPieceList.drawHeaderCallback = DrawSetPieceHeaderCallback;
		setPieceList.drawElementCallback = DrawSetPieceElementCallback;
		setPieceList.elementHeightCallback += SetPieceElementHeightCallback;
		setPieceList.onAddCallback += OnAddCallback;
	}

	public override void OnInspectorGUI () {
		serializedObject.Update ();

		tileListFoldout = EditorGUILayout.BeginFoldoutHeaderGroup (tileListFoldout, "Tiles");
		if (tileListFoldout)
			tileList.DoLayoutList ();
		EditorGUILayout.EndFoldoutHeaderGroup ();

		setPieceListFoldout = EditorGUILayout.BeginFoldoutHeaderGroup (setPieceListFoldout, "Set Pieces");
		if (setPieceListFoldout)
			setPieceList.DoLayoutList ();
		EditorGUILayout.EndFoldoutHeaderGroup ();

		serializedObject.ApplyModifiedProperties ();
	}

	public void DrawTileHeaderCallback (Rect rect) {
		EditorGUI.LabelField (rect, "Tiles");
	}

	public void DrawSetPieceHeaderCallback (Rect rect) {
		EditorGUI.LabelField (rect, "Set Pieces");
	}

	public void DrawTileElementCallback (Rect rect, int index, bool isActive, bool isFocusted) {
		SerializedProperty element = tileList.serializedProperty.GetArrayElementAtIndex (index);
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

	public void DrawSetPieceElementCallback (Rect rect, int index, bool isActive, bool isFocusted) {
		SerializedProperty element = setPieceList.serializedProperty.GetArrayElementAtIndex (index);
		rect.y += 2;

		GameObject prefab = tilesetData.setPiecePrefabs[index];
		string elementTitle = (prefab == null) ? "New Entry" : prefab.name;
		EditorGUI.PropertyField (
			new Rect (rect.x += 10, rect.y, Screen.width * 0.8f, height: EditorGUIUtility.singleLineHeight),
			element, new GUIContent (elementTitle));

		EditorGUI.LabelField (
			new Rect (rect.x += 10,
			rect.y += EditorGUIUtility.singleLineHeight,
			Screen.width * 0.8f,
			EditorGUIUtility.singleLineHeight),
			"CSV Index: " + (tilesetData.tilePrefabs.Count + index + 1));

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
				Debug.Log ("The Set Piece (" + prefab.name + ") is missing the TileObject.cs component\n" + prefab);
			}
		}
	}

	public float TileElementHeightCallback (int index) {
		float propertyHeight = EditorGUI.GetPropertyHeight (tileList.serializedProperty.GetArrayElementAtIndex (index), false);
		float spacing = EditorGUIUtility.singleLineHeight / 2;
		return propertyHeight * spacing;
	}

	public float SetPieceElementHeightCallback (int index) {
		float propertyHeight = EditorGUI.GetPropertyHeight (setPieceList.serializedProperty.GetArrayElementAtIndex (index), false);
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