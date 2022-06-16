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
            startEndPointDepth = maxDepth - option.EndPointNodesRange;

            var curveBindings = AnimationUtility.GetCurveBindings(compressClip);

            foreach (var curveBinding in curveBindings)
            {
                var isTansformCurve = Util.IsTransformKey(curveBinding.propertyName);
                var compressCurve = AnimationUtility.GetEditorCurve(compressClip, curveBinding);

                // Only working on transform keys
                if (isTansformCurve == false)
                    continue;

                CalculateEndPointNode(curveBinding, originCurve, compressCurve);
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
            if (depth < startEndPointDepth || boneMap.ContainsKey(path) == false)
                return;

            var propertyName = curveBinding.propertyName;
            var bone = boneMap[path];
            var keys = compressCurve.keys;
            
            for (var i = 0; i < keys.Length; i++)
            {
                var time = keys[i].time;
                var value = keys[i].value;

                var sampledValue = bone.Sample(propertyName, time);


            }

            var bonem
        }

    }
}

