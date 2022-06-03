using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        private Option option = null;

        // Key - Path
        // Value - PropertyName, EditorCurveBinding
        private Dictionary<string, Dictionary<string, EditorCurveBinding>> nodeMap = new Dictionary<string, Dictionary<string, EditorCurveBinding>>();

        // Cache end point node path name
        private HashSet<string> endPointNodeSet = new HashSet<string>();

        public void Compress(AnimationClip originClip, Option option)
        {
            if (originClip == null)
            {
                Debug.Log($"{nameof(AnimationCompressor)} AnimationClip is null");
                return;
            }

            this.option = option;

            var outputPath = GetOutputPath(originClip, option);
            var compressClip = UnityEngine.Object.Instantiate(originClip);
            compressClip.ClearCurves();

            PreCompress(originClip, compressClip);
            Compress(originClip, compressClip);

            AssetDatabase.CreateAsset(compressClip, outputPath);
            AssetDatabase.SaveAssets();
        }

        private void PreCompress(AnimationClip originClip, AnimationClip compressClip)
        {
            nodeMap.Clear();

            if (option.AccurateEndPointNodes)
            {
                // Cache node map for accurate end point nodes
                // Cache what node is end point 
                var maxDepth = -1;
                var curveBindings = AnimationUtility.GetCurveBindings(originClip);
                foreach (var curveBinding in curveBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                    var path = curveBinding.path;
                    var propertyName = curveBinding.propertyName;
                    var pathDepth = GetPathDepth(path);

                    if (nodeMap.ContainsKey(path) == false)
                        nodeMap[path] = new Dictionary<string, EditorCurveBinding>();

                    nodeMap[path][propertyName] = curveBinding;

                    if (pathDepth > maxDepth)
                        maxDepth = pathDepth;
                }

                if(option.Logging)
                {
                    Debug.Log($"{nameof(AnimationCompressor)} maxDepth : {maxDepth}");
                }

                foreach (var path in nodeMap.Keys)
                {
                    var depth = GetPathDepth(path);
                    if (depth >= option.EndPointNodesDepthMin && depth <= option.EndPointNodesDepthMax)
                        endPointNodeSet.Add(path);
                }

                if (option.Logging)
                    Debug.Log($"{nameof(AnimationCompressor)} endPointNodeSet : {string.Join("\n", endPointNodeSet)}");
            }
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
                InterpolateAccurateEndPointNode(originClip, curveBinding.path, originCurve, compressCurve);

                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        private void CompressByKeyframeReduction(AnimationCurve originCurve, AnimationCurve compressCurve, float allowErrorRange)
        {
            var processedIdx = new HashSet<int>();

            while (true)
            {
                var highestIdx = -1;
                for (var i = 0; i < originCurve.keys.Length; i++)
                {
                    if (processedIdx.Contains(i))
                        continue;

                    var key = originCurve.keys[i];

                    if (highestIdx == -1)
                    {
                        highestIdx = i;
                        continue;
                    }
                    else
                    {
                        var highestValue = Math.Abs(originCurve.keys[highestIdx].value);
                        var value = Math.Abs(key.value);

                        if (value >= highestValue)
                            highestIdx = i;
                    }
                }

                if (highestIdx == -1)
                    break;

                var targetValue = originCurve.keys[highestIdx].value;
                if (Mathf.Abs(targetValue) >= allowErrorRange)
                {
                    compressCurve.AddKey(originCurve.keys[highestIdx]);
                    //originCurve.RemoveKey(highKeyIdx);
                    processedIdx.Add(highestIdx);
                }
                else
                    break;
            }
        }

        private void InterpolateAccurateEndPointNode(AnimationClip originClip, string path, AnimationCurve originCurve, AnimationCurve compressCurve)
        {
            if (option.AccurateEndPointNodes == false)
                return;

            if (endPointNodeSet.Contains(path) == false)
                return;

            var tick = 0f;
            var term = 0.01f;
            var max = originCurve.keys[originCurve.keys.Length - 1].time;

            while(true)
            {
                var originEv = originCurve.Evaluate(tick);
                var compressEv =compressCurve.Evaluate(tick);

                if(Mathf.Abs(originEv - compressEv) > 0.01f)
                {
                    var key = new Keyframe();
                    key.time = tick;
                    key.value = originEv;

                    compressCurve.AddKey(key);
                }


                tick += term;
                if (term >= max)
                    break;
            }
        }

        private string GetOutputPath(AnimationClip originClip, Option option)
        {
            if (originClip == null)
                return string.Empty;

            var originPath = AssetDatabase.GetAssetPath(originClip);
            var outputPath = originPath.Replace($"{originClip.name}.anim", $"{originClip.name}_Compressed.anim");

            if (option.Logging)
                Debug.Log($"{nameof(AnimationCompressor)} Output path : {outputPath}");

            return outputPath;
        }

        private string GetNodeMapCacheKey(EditorCurveBinding curveBinding)
        {
            return $"{curveBinding.path}_{curveBinding.propertyName}";
        }

        private int GetPathDepth(string path)
        {
            var matches = Regex.Matches(path, "/");
            return matches.Count;
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