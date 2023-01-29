using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Rs.ClipMixer
{

    [CreateAssetMenu(fileName = "ClipMixerSetting", menuName = "ClipMixer/ClipMixerSetting", order = 0)]
    public class ClipMixerSetting : ScriptableObject
    {
        public List<ClipMixisingData> MixisingList;
    }
    [CustomEditor(typeof(ClipMixerSetting))]
    public class ClipMixerSettingEditor : Editor
    {
        Dictionary<int, bool> Foldout = new Dictionary<int, bool>();
        public override void OnInspectorGUI()
        {
            ClipMixerSetting mytarget = target as ClipMixerSetting;

            if (GUILayout.Button("Mixer execution!!"))
            {
                ClipMixer.ClipMixingFromSetting(mytarget);
            }

            var PropMixisingList = serializedObject.FindProperty("MixisingList");
            //MixisingListのListsizeの変更。
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("List");

            if (GUILayout.Button("+"))
            {
                PropMixisingList.arraySize += 1;
                serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("-"))
            {
                if (PropMixisingList.arraySize > 1)
                {
                    PropMixisingList.arraySize -= 1;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int cout = 0; cout < PropMixisingList.arraySize; cout += 1)
            {
                var serializeprop = PropMixisingList.GetArrayElementAtIndex(cout);
                var serializeName = serializeprop.FindPropertyRelative("ExprotClipName");

                if (!Foldout.ContainsKey(cout))
                {
                    Foldout.Add(cout, false);
                }

                if (Foldout[cout] = EditorGUILayout.Foldout(Foldout[cout], serializeName.stringValue))
                {



                    var serializeMixClips = serializeprop.FindPropertyRelative("MixClips");
                    //clipのListsizeの変更。
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("Clips");

                    if (GUILayout.Button("+"))
                    {
                        serializeMixClips.arraySize += 1;
                        serializedObject.ApplyModifiedProperties();
                    }
                    if (GUILayout.Button("-"))
                    {
                        if (serializeMixClips.arraySize > 1)
                        {
                            serializeMixClips.arraySize -= 1;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    //アニメーションクリップの表示
                    for (int lcout = 0; lcout < serializeMixClips.arraySize; lcout += 1)
                    {
                        var serializeClip = serializeMixClips.GetArrayElementAtIndex(lcout);
                        EditorGUILayout.PropertyField(serializeClip);
                    }

                    //path周りのGUI
                    var serializePath = serializeprop.FindPropertyRelative("ExprotPath");
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("PathSelect"))
                    {
                        var MyPath = AssetDatabase.GetAssetPath(mytarget);
                        var ParentPath = new DirectoryInfo(MyPath).Parent.FullName;

                        var getpaht = FileUtil.GetProjectRelativePath(EditorUtility.OpenFolderPanel("PathSelect", ParentPath, ""));
                        if (!string.IsNullOrEmpty(getpaht))
                        {
                            serializePath.stringValue = getpaht;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }

                    EditorGUILayout.LabelField(serializePath.stringValue);

                    var fildvalue = EditorGUILayout.TextField(serializeName.stringValue);
                    if (!string.IsNullOrEmpty(fildvalue))
                    {
                        serializeName.stringValue = fildvalue.EndsWith(".anim") ? fildvalue : fildvalue + ".anim";
                        serializedObject.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndHorizontal();

                }

            }

        }

    }
}