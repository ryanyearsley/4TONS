using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Rewired;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.ProjectWindowCallback;

[InitializeOnLoad]
public class NewSceneSetup : Editor {

	static NewSceneSetup () {
		EditorSceneManager.newSceneCreated += OnNewScene;
	}

	private static void OnNewScene (Scene scene, UnityEditor.SceneManagement.NewSceneSetup setup, NewSceneMode mode) {
		Object initializerPrefab = Resources.Load ("RewiredInitializer");
		GameObject go = (GameObject)Instantiate (initializerPrefab, Vector3.zero, Quaternion.identity);
		go.name = "RewiredInitializer";
	}

	[MenuItem ("Assets/Create/New Scene With Rewired", priority = 1)]
	private static void OnCreateMenuNewScene () {
		//Object selectedObject = Selection.activeObject;
		//Scene newScene = EditorSceneManager.NewScene (UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		//EditorSceneManager.SaveScene (newScene, GetClosestDirectory (selectedObject) + "/New Scene.unity");

		//string path = GetClosestDirectory (selectedObject) + "/New Scene.unity";
		ProjectWindowUtil.StartNameEditingIfProjectWindowExists (
			0,
			ScriptableObject.CreateInstance<DoCreateScene> (),
			"New Scene.unity",
			null,//EditorGUIUtility.FindTexture ("SceneAsset Icon"),
			null);
	}

	private static string GetClosestDirectory (Object obj) {
		string path = "Assets";
		if (obj != null) {
			path = AssetDatabase.GetAssetPath (obj.GetInstanceID ());

			if (path.Length < 0)
				path = "Assets";

			else if (!Directory.Exists (path)) {
				Regex regex = new Regex ("/(?!.*/).*");
				string toRemove = regex.Match (path).Value;
				path = path.Replace (toRemove, "");
			}
		}

		return path;
	}

	private class DoCreateScene : EndNameEditAction {
		public override void Action (int instanceId, string pathName, string resourceFile) {
			Scene newScene = EditorSceneManager.NewScene (UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
			EditorSceneManager.SaveScene (newScene, pathName);
			//GUIUtility.ExitGUI ();
		}
	}
}