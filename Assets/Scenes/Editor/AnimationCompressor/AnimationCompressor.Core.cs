using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        public void Compress(AnimationClip clip, Option option)
        {
            if (clip == null)
            {
                Debug.LogError("AnimationCompressor.Core.Compress - AnimationClip is null");
                return;
            }

            var fileName = $"{clip.name}_Compressed";

            var copyClip = Object.Instantiate<AnimationClip>(clip);

            var curves = AnimationUtility.GetCurveBindings(clip);

            var outputPath = "";
            AssetDatabase.CreateAsset((Object)copyClip, outputPath);
            AssetDatabase.SaveAssets(); 
            AssetDatabase.DeleteAsset(fileName);
        }
    }
}