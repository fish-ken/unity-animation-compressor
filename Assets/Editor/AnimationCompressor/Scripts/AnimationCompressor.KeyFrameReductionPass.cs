using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public partial class Core
    {
        private void KeyFrameReductionPass()
        {
            var curveBindings = AnimationUtility.GetCurveBindings(originClip);

            foreach (var curveBinding in curveBindings)
            {
                var isTansformCurve = Util.IsTransformKey(curveBinding.propertyName);
                var originCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);
                var compressCurve = AnimationUtility.GetEditorCurve(originClip, curveBinding);

                // Only working on transform keys
                if (isTansformCurve)
                {
                    compressCurve.keys = null;
                    GenerateKeyFrameByCurveFitting(curveBinding, originCurve, compressCurve);
                }

                compressClip.SetCurve(curveBinding.path, curveBinding.type, curveBinding.propertyName, compressCurve);
            }
        }

        /// <summary>
        /// 없어도될 키프레임 제거
        /// </summary>
        /// <param name="originCurve"></param>
        /// <param name="compressCurve"></param>
        /// <param name="allowErrorRange"></param>
        private void KeyFrameReduction(EditorCurveBinding curveBinding, AnimationCurve originCurve, AnimationCurve compressCurve)
        {
            var propertyName = curveBinding.propertyName;
            var path = curveBinding.path;
            var depth = Util.GetDepth(path);
            var allowErrorRange = KeyFrameReduction_GetAllowErrorValue(propertyName, depth);

            Debug.Log($"{nameof(AnimationCompressor)} itrCount : {itrCount}");
        }

        private float KeyFrameReduction_GetAllowErrorValue(string propertyName, int depth = 1)
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
                    return option.ScaleAllowError;      /// fDepth;

                default:
                    return 0f;
            }
        }
    }
}