using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        private Option option = null;

        private Bone RootBone = null;

        private int maxDepth = -1;

        public void Compress(AnimationClip originClip, Option option)
        {
            if (originClip == null)
            {
                Debug.Log($"{nameof(AnimationCompressor)} AnimationClip is null");
                return;
            }

            this.option = option;

            var outputPath = Util.GetOutputPath(originClip);
            var compressClip = Object.Instantiate(originClip);
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
                maxDepth = -1;
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

                if (option.Logging)
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
                var path = curveBinding.path;
                var depth = GetPathDepth(path);
                var originCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                var compressCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                compressCurve.keys = null;

                // Key frame reduction by Option.AllowErrorRange
                CompressByKeyframeReduction(originCurve, compressCurve, GetAllowErrorValue(curveBinding.propertyName, depth));

                // WIP : looks like trash now
                // Interpolate end point node (hand, feet)
                //InterpolateAccurateEndPointNode(originClip, curveBinding.path, originCurve, compressCurve);

                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        private void CompressByKeyframeReduction(AnimationCurve originCurve, AnimationCurve compressCurve, float allowErrorRange)
        {
            var itrCount = 0f;

            compressCurve.AddKey(originCurve.keys[0]);
            compressCurve.AddKey(originCurve.keys[originCurve.keys.Length - 1]);

            while (true)
            {
                var tick = 0f;
                var term = 0.01f;
                var time = originCurve.keys[originCurve.keys.Length - 1].time;

                var highestOffset = -1f;
                var highestOffsetTick = -1f;

                while (tick < time)
                {
                    var orgEv = originCurve.Evaluate(tick);
                    var compEv = compressCurve.Evaluate(tick);
                    var offset = Mathf.Abs(orgEv - compEv);

                    if (offset >= allowErrorRange)
                    {
                        if (offset > highestOffset)
                        {
                            highestOffset = offset;
                            highestOffsetTick = tick;
                        }
                    }

                    tick += term;
                }

                if (highestOffset == -1)
                    break;

                var key = new Keyframe();
                key.time = highestOffsetTick;
                key.value = originCurve.Evaluate(highestOffsetTick);

                compressCurve.AddKey(key);
                itrCount++;
            }

            if (option.Logging)
                Debug.Log($"{nameof(AnimationCompressor)} itrCount : {itrCount}");
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

            while (true)
            {
                var originEv = originCurve.Evaluate(tick);
                var compressEv = compressCurve.Evaluate(tick);

                if (Mathf.Abs(originEv - compressEv) > 0.01f)
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

        private string GetNodeMapCacheKey(EditorCurveBinding curveBinding)
        {
            return $"{curveBinding.path}_{curveBinding.propertyName}";
        }

        private float GetAllowErrorValue(string propertyName, int depth = 1)
        {
            // Restrict divide by zero
            depth = Mathf.Max(1, depth);

            var fDepth = (float)depth;

            var revDepth = maxDepth - depth;
            var multiplier = (float)revDepth;

            switch (propertyName)
            {
                case "m_LocalPosition":
                case "m_LocalPosition.x":
                case "m_LocalPosition.y":
                case "m_LocalPosition.z":
                    return option.PositionAllowError / fDepth;

                case "m_LocalRotation":
                case "m_LocalRotation.x":
                case "m_LocalRotation.y":
                case "m_LocalRotation.z":
                case "m_LocalRotation.w":
                    return option.RotationAllowError / fDepth;

                case "m_LocalScale":
                case "m_LocalScale.y":
                case "m_LocalScale.z":
                case "m_LocalScale.x":
                    return option.ScaleAllowError / fDepth;

                default:
                    return 0f;
            }
        }
    }
}