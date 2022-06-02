using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        public void Compress(AnimationClip originClip, Option option)
        {
            if (originClip == null)
            {
                Debug.LogError("AnimationCompressor.Core.Compress - AnimationClip is null");
                return;
            }

            var fileName = $"{originClip.name}_Compressed";

            var copyClip = Object.Instantiate<AnimationClip>(originClip);

            var curves = AnimationUtility.GetCurveBindings(originClip);

            var outputPath = GetOutputPath(originClip);

            AssetDatabase.CreateAsset(copyClip, outputPath);
            AssetDatabase.SaveAssets(); 
            //AssetDatabase.DeleteAsset(fileName);
        }

        private string GetOutputPath(AnimationClip originClip)
        {
            if (originClip == null)
                return string.Empty;


            var originPath = AssetDatabase.GetAssetPath(originClip);
            Debug.Log(originPath);
            return originPath;
        }
    }
}