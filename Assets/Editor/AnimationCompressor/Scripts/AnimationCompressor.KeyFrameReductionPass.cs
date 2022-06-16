using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public partial class Core
    {
        private void KeyFrameReductionPass()
        {
            var curveBindings = AnimationUtility.GetCurveBindings(compressClip);

            foreach (var curveBinding in curveBindings)
            {
                var isTansformCurve = Util.IsTransformKey(curveBinding.propertyName);
                var compressCurve = AnimationUtility.GetEditorCurve(compressClip, curveBinding);

                // Only working on transform keys
                if (isTansformCurve == false)
                    continue;

                KeyFrameReduction(curveBinding, compressCurve);
                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        /// <summary>
        /// 없어도될 키프레임 제거
        /// </summary>
        /// <param name="originCurve"></param>
        /// <param name="compressCurve"></param>
        /// <param name="allowErrorRange"></param>
        private void KeyFrameReduction(EditorCurveBinding curveBinding, AnimationCurve compressCurve)
        {
            var propertyName = curveBinding.propertyName;
            var path = curveBinding.path;
            var depth = Util.GetDepth(path);
            var allowErrorRange = GetAllowErrorValue(propertyName, depth);

            var keys = compressCurve.keys;
            var newKeyset = new List<Keyframe>();
            for (var i = 0; i < keys.Length - 1; i++)
            {
                var curKey = keys[i];
                var nextKey = keys[i + 1];

                var valueOffset = Mathf.Abs(curKey.value - nextKey.value);
                if (valueOffset >= allowErrorRange)
                {
                    newKeyset.Add(curKey);

                    if (i == keys.Length - 1)
                        newKeyset.Add(nextKey);
                }
                else
                {
                    var newValue = (curKey.value + nextKey.value) / 2f;
                    var newTime = (nextKey.time + curKey.time) / 2f;
                    var newKey = new Keyframe();
                    newKey.value = newValue;
                    newKey.time = newTime;
                }
            }

            compressCurve.keys = newKeyset.ToArray();
        }
    }
}