
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rs.ClipMixer
{
    public class ClipMixer
    {
        public static void ClipMixingFromSetting(ClipMixerSetting ClipMixerSetting)
        {
            Dictionary<string, AnimationClip> OutputAniClips = new Dictionary<string, AnimationClip>();
            foreach (var clipMixisingData in ClipMixerSetting.MixisingList)
            {

                var aniclip = ClipListMix(clipMixisingData.MixClips);
                var outputpaht = clipMixisingData.GetFullPath();

                OutputAniClips.Add(outputpaht, aniclip);
            }
            SaveAniClips(OutputAniClips);
        }
        public static AnimationClip ClipListMix(List<AnimationClip> MixClips)
        {
            if (MixClips.Count == 0) throw new System.ArgumentNullException("MixClips Is Null!");

            AnimationClip OutPutClip = new AnimationClip();
            if (MixClips[0] != null)
            {
                var firstClipSettings = AnimationUtility.GetAnimationClipSettings(MixClips[0]);
                AnimationUtility.SetAnimationClipSettings(OutPutClip, firstClipSettings);
            }
            foreach (var clip in MixClips)
            {
                if (clip == null) continue;
                MixClip(ref OutPutClip, clip);
            }

            return OutPutClip;

        }
        public static void OverrideMixerFromSetting(OverrideMixer cms)
        {
            var OverrideClipDict = ToTagPeaClips(FiltalingExtension(Directory.EnumerateFiles(cms.OverrideClipsPath), ".anim"));

            Dictionary<string, AnimationClip> OutputAniClips = new Dictionary<string, AnimationClip>();

            string OutputPath = cms.OutputPath + "/";

            foreach (string SouseClipPath in FiltalingExtension(Directory.EnumerateFiles(cms.SourceClipsPath), ".anim"))
            {
                string SaveClipName = Path.GetFileName(SouseClipPath).Replace(TagListUp(SouseClipPath) + "_", string.Empty);

                string SavePath = OutputPath + SaveClipName;

                AnimationClip SaveClip = MixClipToTag(OverrideClipDict, TagListUp(SouseClipPath), AssetDatabase.LoadAssetAtPath<AnimationClip>(SouseClipPath));

                OutputAniClips.Add(SavePath, SaveClip);
            }

            SaveAniClips(OutputAniClips);

        }
        public static IEnumerable<string> FiltalingExtension(IEnumerable<string> strings, string Extension)
        {
            List<string> Maths = new List<string>();

            foreach (var str in strings)
            {

                if (Path.GetExtension(str) == Extension)
                {
                    Maths.Add(str);
                }
            }
            return Maths;

        }
        public static void SaveAniClips(Dictionary<string, AnimationClip> SaveClips)
        {


            foreach (var kvp in SaveClips)
            {
                SaveAsset(kvp.Key,kvp.Value,typeof(AnimationClip),false);
            }
            AssetDatabase.SaveAssets();

        }
        public static void SaveAsset(string path, Object SaveAsset, System.Type type,bool SaveAssetFlag = true)
        {
            var loadasset = AssetDatabase.LoadAssetAtPath(path, type);
            if (loadasset == null)
            {
                AssetDatabase.CreateAsset(SaveAsset,path);
            }
            else
            {
                EditorUtility.CopySerialized(SaveAsset,loadasset);
                if(SaveAssetFlag) AssetDatabase.SaveAssets();
            }
        }
        public static AnimationClip MixClipToTag(Dictionary<char, AnimationClip> OverideTagPeaClip, string SourceOvrrideTags, AnimationClip SourceClip)
        {
            var OutputAniclip = new AnimationClip();
            var SourceClipSetting = AnimationUtility.GetAnimationClipSettings(SourceClip);
            AnimationUtility.SetAnimationClipSettings(OutputAniclip, SourceClipSetting);

            foreach (char Tag in SourceOvrrideTags)
            {
                if (OverideTagPeaClip.ContainsKey(Tag))
                {
                    MixClip(ref OutputAniclip, OverideTagPeaClip[Tag]);
                }
                else
                {
                    Debug.LogWarning("タグ " + Tag + " が存在しません");
                }
            }

            MixClip(ref OutputAniclip, SourceClip);

            return OutputAniclip;
        }
        public static void MixClip(ref AnimationClip Base, AnimationClip mix)
        {
            var MixBinds = AnimationUtility.GetCurveBindings(mix);

            foreach (var MixBind in MixBinds)
            {
                AnimationUtility.SetEditorCurve(Base, MixBind, AnimationUtility.GetEditorCurve(mix, MixBind));
            }

        }
        public static string TagListUp(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Split('_')[0];
        }
        public static Dictionary<char, AnimationClip> ToTagPeaClips(IEnumerable<string> ClipPaths)
        {
            Dictionary<char, AnimationClip> TagPeaClips = new Dictionary<char, AnimationClip>();

            foreach (var ClipPath in ClipPaths)
            {
                var ClipName = Path.GetFileNameWithoutExtension(ClipPath);

                var tag = TagListUp(ClipName);

                if (tag.Length != 1) { Debug.LogWarning(ClipName + "の命名規則が正しくありません"); continue; }

                TagPeaClips.Add(tag[0], AssetDatabase.LoadAssetAtPath<AnimationClip>(ClipPath));

            }
            return TagPeaClips;
        }

    }

    [System.Serializable]
    public struct ClipMixisingData
    {
        public string ExprotClipName;
        public string ExprotPath;
        public List<AnimationClip> MixClips;

        public string GetFullPath()
        {
            return ExprotPath + "/" + ExprotClipName;
        }
    }
}
