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
                var curve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                var newCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                newCurve.keys = null;

                while (true)
                {

                    var highKeyIdx = -1;
                    for (var i = 0; i < curve.keys.Length; i++)
                    {
                        var key = curve.keys[i];

                        if (highKeyIdx == -1)
                        {
                            highKeyIdx = i;
                            continue;
                        }
                        else
                        {
                            var highKeyValue = Math.Abs(curve.keys[highKeyIdx].value);
                            var curKeyValue = Math.Abs(key.value);

                            if (curKeyValue >= highKeyValue)
                                highKeyIdx = i;
                        }
                    }

                    if (highKeyIdx == -1)
                        break;

                    var highestValue = curve.keys[highKeyIdx].value;
                    if (Mathf.Abs(highestValue) >= GetAllowErrorValue(curveBinding.propertyName))
                    {
                        newCurve.AddKey(curve.keys[highKeyIdx]);
                        curve.RemoveKey(highKeyIdx);
                    }
                    else
                        break;
                }

                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, newCurve);
            }
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
                    return option.PositionAllowError;

                case "m_LocalScale":
                case "m_LocalScale.y":
                case "m_LocalScale.z":
                case "m_LocalScale.x":
                    return option.PositionAllowError;

                default:
                    return 0f;
            }
        }
    }
}