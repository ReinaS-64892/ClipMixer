
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rs.ClipMixer
{
    public class ClipMixer
    {
        public static void OverrideMixer(OverrideMixer cms)
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
                AssetDatabase.CreateAsset(kvp.Value, kvp.Key);
            }
        }



        public static AnimationClip MixClipToTag(Dictionary<char, AnimationClip> OverideTagPeaClip, string SourceOvrrideTags, AnimationClip SourceClip)
        {
            var Aniclip = new AnimationClip();

            foreach (char Tag in SourceOvrrideTags)
            {
                if (OverideTagPeaClip.ContainsKey(Tag))
                {
                    MixClip(ref Aniclip, OverideTagPeaClip[Tag]);
                }
                else
                {
                    Debug.LogWarning("タグ " + Tag + " が存在しません");
                }
            }

            MixClip(ref Aniclip, SourceClip);

            return Aniclip;
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

}
