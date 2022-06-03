using System;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        private Option option = null;

        public void Compress(AnimationClip originClip, Option option)
        {
            if (originClip == null)
            {
                Debug.LogError("AnimationCompressor.Core.Compress - AnimationClip is null");
                return;
            }

            this.option = option;

            var outputPath = GetOutputPath(originClip);
            var compressClip = UnityEngine.Object.Instantiate<AnimationClip>(originClip);
            compressClip.ClearCurves();

            Compress(originClip, compressClip);

            AssetDatabase.CreateAsset(compressClip, outputPath);
            AssetDatabase.SaveAssets();
        }

        private void Compress(AnimationClip originClip, AnimationClip compressClip)
        {
            var curveBindings = AnimationUtility.GetCurveBindings(originClip);

            foreach (var curveBinding in curveBindings)
            {
                var originCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                var compressCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                compressCurve.keys = null;

                // Key frame reduction by Option.AllowErrorRange
                CompressByKeyframeReduction(originCurve, compressCurve, GetAllowErrorValue(curveBinding.propertyName));

                // Interpolate end point node (hand, feet)
                InterpolateAccurateEndPointNode(originCurve, compressCurve);

                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        private void CompressByKeyframeReduction(AnimationCurve originCurve, AnimationCurve compressCurve, float allowErrorRange)
        {
            while (true)
            {
                var highKeyIdx = -1;
                for (var i = 0; i < originCurve.keys.Length; i++)
                {
                    var key = originCurve.keys[i];

                    if (highKeyIdx == -1)
                    {
                        highKeyIdx = i;
                        continue;
                    }
                    else
                    {
                        var highKeyValue = Math.Abs(originCurve.keys[highKeyIdx].value);
                        var curKeyValue = Math.Abs(key.value);

                        if (curKeyValue >= highKeyValue)
                            highKeyIdx = i;
                    }
                }

                if (highKeyIdx == -1)
                    break;

                var highestValue = originCurve.keys[highKeyIdx].value;
                if (Mathf.Abs(highestValue) >= allowErrorRange)
                {
                    compressCurve.AddKey(originCurve.keys[highKeyIdx]);
                    originCurve.RemoveKey(highKeyIdx);
                }
                else
                    break;
            }
        }

        private void InterpolateAccurateEndPointNode(AnimationCurve originCurve, AnimationCurve compressCurve)
        {
            if (option.AccurateEndPointNode == false)
                return;


        }

        private string GetOutputPath(AnimationClip originClip)
        {
            if (originClip == null)
                return string.Empty;

            var originPath = AssetDatabase.GetAssetPath(originClip);
            var outputPath = originPath.Replace($"{originClip.name}.anim", $"{originClip.name}_Compressed.anim");

            if (option.Logging)
                Debug.Log($"{nameof(AnimationCompressor)} Output path : {outputPath}");

            return outputPath;
        }

        private float GetAllowErrorValue(string propertyName)
        {
            switch (propertyName)
            {
                case "m_LocalPositio":
                case "m_LocalPosition.x":
                case "m_LocalPosition.y":
                case "m_LocalPosition.z":
                    return option.PositionAllowError;

                case "m_LocalRotation":
                case "m_LocalRotation.x":
                case "m_LocalRotation.y":
                case "m_LocalRotation.z":
                case "m_LocalRotation.w":
                    return option.RotationAllowError;

                case "m_LocalScale":
                case "m_LocalScale.y":
                case "m_LocalScale.z":
                case "m_LocalScale.x":
                    return option.ScaleAllowError;

                default:
                    return 0f;
            }
        }
    }
}