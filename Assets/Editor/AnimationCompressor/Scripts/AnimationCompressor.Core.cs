using System;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        private Option option = null;
        private readonly Type curveType = typeof(Transform);

        public void Compress(AnimationClip originClip, Option option)
        {
            if (originClip == null)
            {
                Debug.LogError("AnimationCompressor.Core.Compress - AnimationClip is null");
                return;
            }

            this.option = option;


            var outputPath = GetOutputPath(originClip);
            var curveBindings = AnimationUtility.GetCurveBindings(originClip);
            var copyClip = UnityEngine.Object.Instantiate<AnimationClip>(originClip);
            copyClip.ClearCurves();


            foreach(var curveBinding in curveBindings)
            {
                //copyClip.
                var curve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                copyClip.SetCurve(curveBinding.path, curveType, curveBinding.propertyName, curve);
            }


            AssetDatabase.CreateAsset(copyClip, outputPath);
            AssetDatabase.SaveAssets(); 
        }

        private string GetOutputPath(AnimationClip originClip)
        {
            if (originClip == null)
                return string.Empty;

            var originPath = AssetDatabase.GetAssetPath(originClip);
            var outputPath = originPath.Replace($"{originClip.name}.anim", $"{originClip.name}_Compressed.anim");

            if(option.Logging)
                Debug.Log(outputPath);

            return outputPath;
        }
    }
}