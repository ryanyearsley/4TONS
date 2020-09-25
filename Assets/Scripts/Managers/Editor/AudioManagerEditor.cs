using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

[CustomEditor (typeof (AudioManager))]
public class AudioManagerEditor : Editor {

    private AudioManager audioManager;
    private SerializedObject property;
    private SerializedProperty soundList;
    private Dictionary<int, bool> soundFoldoutDictionary;

    private void OnEnable () {
        audioManager = (AudioManager)target;
        property = new SerializedObject (audioManager);
        soundList = property.FindProperty ("sounds");
        soundFoldoutDictionary = new Dictionary<int, bool> ();
        if (soundList.arraySize > 0) {
            for (int i = 0; i < soundList.arraySize; i++) {
                soundFoldoutDictionary.Add (i, false);
            }
        }
    }

    public override void OnInspectorGUI () {
        property.Update ();

        for (int i = 0; i < soundList.arraySize; i++) {
            SerializedProperty sound = soundList.GetArrayElementAtIndex(i);
            SerializedProperty clipName = sound.FindPropertyRelative ("clipName");

            string clipTitle = "";
            if (clipName.stringValue == clipTitle) {
                clipTitle = "New Sound";
            } else {
                clipTitle = clipName.stringValue;
            }

            soundFoldoutDictionary [i] = EditorGUILayout.BeginFoldoutHeaderGroup (soundFoldoutDictionary [i], clipTitle);
            if (soundFoldoutDictionary[i])
                EditorGUILayout.BeginVertical (EditorStyles.helpBox);

            if (soundFoldoutDictionary [i]) {
                SerializedProperty mixerGroup = sound.FindPropertyRelative ("mixerGroup");
                SerializedProperty volume = sound.FindPropertyRelative ("volume");
                SerializedProperty pitch = sound.FindPropertyRelative ("pitch");
                SerializedProperty loop = sound.FindPropertyRelative ("loop");
                SerializedProperty playOnAwake = sound.FindPropertyRelative ("playOnAwake");
                SerializedProperty randomize = sound.FindPropertyRelative ("randomize");
                SerializedProperty randomClips = sound.FindPropertyRelative ("randomClips");
                SerializedProperty singleClip = sound.FindPropertyRelative ("singleClip");

                clipName.stringValue = EditorGUILayout.TextField ("Sound Name", clipName.stringValue);
                mixerGroup.objectReferenceValue = EditorGUILayout.ObjectField ("Mixer Group", mixerGroup.objectReferenceValue, typeof (AudioMixerGroup), false);
                volume.floatValue = EditorGUILayout.Slider ("Volume", volume.floatValue, 0f, 1f);
                pitch.floatValue = EditorGUILayout.Slider ("Pitch", pitch.floatValue, 0f, 2f);

                EditorGUILayout.BeginHorizontal ();
                //GUILayout.FlexibleSpace ();
                loop.boolValue = EditorGUILayout.Toggle ("Loop", loop.boolValue);
                playOnAwake.boolValue = EditorGUILayout.Toggle ("Play On Awake", playOnAwake.boolValue);
                EditorGUILayout.EndHorizontal ();

                randomize.boolValue = EditorGUILayout.Toggle ("Randomize", randomize.boolValue);

                if (randomize.boolValue) {
                    EditorGUILayout.BeginHorizontal ();
                    //GUILayout.FlexibleSpace ();
                    EditorGUILayout.LabelField ("Random Clips");
                    if (GUILayout.Button ("New Random Clip")) {
                        randomClips.InsertArrayElementAtIndex (randomClips.arraySize);
                    }
                    EditorGUILayout.EndHorizontal ();

                    for (int a = 0; a < randomClips.arraySize; a++) {
                        SerializedProperty clip = randomClips.GetArrayElementAtIndex (a);
                        EditorGUILayout.BeginHorizontal ();
                        EditorGUILayout.LabelField ("Random Clip (" + a.ToString () + ")", GUILayout.MaxWidth (120));
                        clip.objectReferenceValue = EditorGUILayout.ObjectField (clip.objectReferenceValue, typeof (AudioClip), false);
                        if (GUILayout.Button ("-", GUILayout.MaxWidth (15), GUILayout.MaxHeight (15))) {
                            randomClips.DeleteArrayElementAtIndex (a);
                        }
                        EditorGUILayout.EndHorizontal ();
                    }
                } else {
                    singleClip.objectReferenceValue = EditorGUILayout.ObjectField ("Single Clip", singleClip.objectReferenceValue, typeof (AudioClip), false);
                }

                //EditorGUILayout.Space ();


                if (GUILayout.Button ("Remove This Clip")) {
                    soundList.DeleteArrayElementAtIndex (i);
                    soundFoldoutDictionary.Remove (i);
                }
            }

            if (soundFoldoutDictionary[i])
                EditorGUILayout.EndVertical ();
            EditorGUILayout.EndFoldoutHeaderGroup ();
        }

        if (GUILayout.Button ("Add Clip")) {
            soundFoldoutDictionary.Add (soundList.arraySize, true);
            soundList.InsertArrayElementAtIndex (soundList.arraySize);
        }

        property.ApplyModifiedProperties ();
    }
}