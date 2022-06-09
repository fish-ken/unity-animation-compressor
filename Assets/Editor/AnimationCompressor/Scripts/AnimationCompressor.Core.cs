using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        private Option option = null;
        private AnimationClip originClip = null;
        private AnimationClip compressClip = null;

        private Dictionary<string, Bone> boneMap = new Dictionary<string, Bone>();

        private int maxDepth = -1;

        public void Compress(AnimationClip originClip, Option option)
        {
            if (originClip == null)
            {
                Debug.Log($"{nameof(AnimationCompressor)} AnimationClip is null");
                return;
            }

            this.option = option;
            this.originClip = originClip;

            ProcessCompress();
        }

        private void ProcessCompress()
        {
            var outputPath = Util.GetOutputPath(originClip);
            compressClip = Object.Instantiate(originClip);
            compressClip.ClearCurves();

            PreCompress();
            Compress();

            AssetDatabase.CreateAsset(compressClip, outputPath);
            AssetDatabase.SaveAssets();
        }

        private void PreCompress()
        {
            GenerateBoneMap();
        }

        private void GenerateBoneMap()
        {
            boneMap.Clear();

            // Create bone map
            var bindings = AnimationUtility.GetCurveBindings(originClip);
            foreach (var binding in bindings)
            {
                var path = binding.path;
                var propertyName = binding.propertyName;
                var curve = AnimationUtility.GetEditorCurve(originClip, binding);
                Bone bone;

                if (boneMap.ContainsKey(path) == false)
                    boneMap[path] = new Bone(path);

                bone = boneMap[path];
                bone.SetCurve(propertyName, curve);

            }

            // Set parent 
            foreach (var bone in boneMap.Values)
            {
                var upperPath = Util.GetUpperDepth(bone.Path);

                while(string.IsNullOrEmpty(upperPath) == false)
                {
                    if(boneMap.ContainsKey(upperPath))
                    {
                        bone.SetParent(boneMap[upperPath]);
                        break;
                    }

                    upperPath = Util.GetUpperDepth(bone.Path);
                }
            }
        }


        private void Compress()
        {
            var curveBindings = AnimationUtility.GetCurveBindings(originClip);

            foreach (var curveBinding in curveBindings)
            {
                var path = curveBinding.path;
                var depth = Util.GetDepth(path);
                var originCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                var compressCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                compressCurve.keys = null;

                CompressByKeyframeReduction(originCurve, compressCurve, GetAllowErrorValue(curveBinding.propertyName, depth));

                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        /// <summary>
        /// allow range 보다 큰 지점에 key 추가
        /// </summary>
        /// <param name="originCurve"></param>
        /// <param name="compressCurve"></param>
        /// <param name="allowErrorRange"></param>
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

            Debug.Log($"{nameof(AnimationCompressor)} itrCount : {itrCount}");
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
                    return option.PositionAllowError; // fDepth;

                case "m_LocalRotation":
                case "m_LocalRotation.x":
                case "m_LocalRotation.y":
                case "m_LocalRotation.z":
                case "m_LocalRotation.w":
                    return option.RotationAllowError; // fDepth;

                case "m_LocalScale":
                case "m_LocalScale.y":
                case "m_LocalScale.z":
                case "m_LocalScale.x":
                    return option.ScaleAllowError;/// fDepth;

                default:
                    return 0f;
            }
        }
    }
}