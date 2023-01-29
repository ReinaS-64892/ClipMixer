
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rs.ClipMixer
{

    [CreateAssetMenu(fileName = "OverrideMixerSetting", menuName = "ClipMixer/OverrideMixerSetting")]
    public class OverrideMixer : ScriptableObject
    {
        public string SourceClipsPath;
        public string OverrideClipsPath;
        public string OutputPath;

    }
    [CustomEditor(typeof(OverrideMixer))]
    public class OverrideMixerEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            OverrideMixer mytarget = target as OverrideMixer;

            var MyPath = AssetDatabase.GetAssetPath(mytarget);

            var ParentPath = new DirectoryInfo(MyPath).Parent.FullName;

            var SourceClipsPathProp = serializedObject.FindProperty("SourceClipsPath");

            EditorGUILayout.LabelField("SourceClipsPath   " + SourceClipsPathProp.stringValue);

            if (GUILayout.Button("SourceClipsPath Select"))
            {
                var getpaht = FileUtil.GetProjectRelativePath(EditorUtility.OpenFolderPanel("Select SourceClipsPath", ParentPath, ""));

                if (!string.IsNullOrEmpty(getpaht))
                {
                    SourceClipsPathProp.stringValue = getpaht;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            var OverrideClipsPathProp = serializedObject.FindProperty("OverrideClipsPath");

            EditorGUILayout.LabelField("OverrideClipsPath   " + OverrideClipsPathProp.stringValue);

            if (GUILayout.Button("OverrideClipsPath Select"))
            {
                var getpaht = FileUtil.GetProjectRelativePath(EditorUtility.OpenFolderPanel("Select OverrideClipsPath", ParentPath, ""));

                if (!string.IsNullOrEmpty(getpaht)) 
                {
                    OverrideClipsPathProp.stringValue = getpaht;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            var OutputPathProp = serializedObject.FindProperty("OutputPath");

            EditorGUILayout.LabelField("OutputPath   " + OutputPathProp.stringValue);

            if (GUILayout.Button("OutputPath Select"))
            {
                var getpaht = FileUtil.GetProjectRelativePath(EditorUtility.OpenFolderPanel("Select OutputPath", ParentPath, ""));

                if (!string.IsNullOrEmpty(getpaht))
                {
                    OutputPathProp.stringValue = getpaht;
                    serializedObject.ApplyModifiedProperties();
                }
            }


            GUILayout.Label("");

            if (GUILayout.Button("ClipMix!!"))
            {
                ClipMixer.OverrideMixer(mytarget);
            }




        }

    }
}
