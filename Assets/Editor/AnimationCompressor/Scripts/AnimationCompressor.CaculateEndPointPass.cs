using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public partial class Core
    {
        private int startEndPointDepth = -1;

        private void CalculateEndPointNodePass()
        {
            GenerateCurrentCompressedAnimationBoneMap();
            startEndPointDepth = maxDepth - option.EndPointNodesRange;

            var curveBindings = AnimationUtility.GetCurveBindings(compressClip);
            foreach (var curveBinding in curveBindings)
            {
                var isTansformCurve = Util.IsTransformKey(curveBinding.propertyName);
                var compressCurve = AnimationUtility.GetEditorCurve(compressClip, curveBinding);

                // Only working on transform keys
                if (isTansformCurve == false)
                    continue;

                CalculateEndPointNode(curveBinding, compressCurve);
                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        /// <summary>
        /// 키프레임 재생성
        /// </summary>
        /// <param name="originCurve"></param>
        /// <param name="compressCurve"></param>
        /// <param name="allowErrorRange"></param>
        private void CalculateEndPointNode(EditorCurveBinding curveBinding, AnimationCurve compressCurve)
        {
            var path = curveBinding.path;
            var depth = Util.GetDepth(curveBinding.path);

            if (depth < startEndPointDepth)
                return;
            else if(originBoneMap == null || originBoneMap.ContainsKey(path) == false)
                return;
            else if (compressBoneMap == null || compressBoneMap.ContainsKey(path) == false)
                return;

            var keys = compressCurve.keys;
            var propertyName = curveBinding.propertyName;
            var originBone = originBoneMap[path];
            var compressBone = compressBoneMap[path];

            var tick = 0f;
            var time = compressClip.length;
            var term = compressClip.frameRate / 60f;
            while(tick <= time)
            {
                var orgSampleValue = originBone.Sample(propertyName, tick);
                var compSampleValue = compressBone.Sample(propertyName, tick);

                var rawOffset = compSampleValue - orgSampleValue;
                var offset = Mathf.Abs(rawOffset);

                if (offset > 0.01)
                {
                    // Psyche 
                    var newKey = new Keyframe();
                    newKey.value = compSampleValue + rawOffset;
                    newKey.time = tick;
                    newKey.inWeight = newKey.outWeight = 1 / 3f;
                    compressCurve.AddKey(newKey);
                }

                tick += term;
            }

            //for (var i = 0; i < keys.Length; i++)
            //{
            //    var time = keys[i].time;
            //    var orgSampleValue = originBone.Sample(propertyName, time);
            //    var compSampleValue = compressBone.Sample(propertyName, time);

            //    var rawOffset = compSampleValue - orgSampleValue;
            //    var offset = Mathf.Abs(rawOffset);

            //    if (offset < 0.01)
            //        continue;

            //    keys[i].value += rawOffset;
            //}
        }

    }
}

