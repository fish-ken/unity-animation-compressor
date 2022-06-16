using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public partial class Core
    {
        private Option option = null;
        private AnimationClip originClip = null;
        private AnimationClip compressClip = null;

        private readonly int TotalStep = 4;

        private void UpdateProgressBar(string desc, int step)
        {
            EditorUtility.DisplayProgressBar(nameof(AnimationCompressor), desc, (float)step / TotalStep);
        }

        private void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

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

            compressClip = AssetDatabase.LoadMainAssetAtPath(outputPath) as AnimationClip;
            var isOutputExist = compressClip != null;

            if (isOutputExist)
                EditorUtility.CopySerialized(originClip, compressClip);
            else
                compressClip = Object.Instantiate(originClip);

            EditorUtility.CopySerialized(originClip, compressClip);
            compressClip.ClearCurves();

            PreCompress();
            Compress();

            if (isOutputExist)
                AssetDatabase.SaveAssets();
            else
                AssetDatabase.CreateAsset(compressClip, outputPath);

            AssetDatabase.Refresh();
            ClearProgressBar();
        }

        private void PreCompress()
        {
            UpdateProgressBar(nameof(GenerateOriginalAnimationBoneMap), 1);
            GenerateOriginalAnimationBoneMap();
        }

        private void Compress()
        {
            UpdateProgressBar(nameof(GenerateKeyFrameByCurveFittingPass), 2);
            GenerateKeyFrameByCurveFittingPass();

            // 집어치자 - 거지같음 ㅇㅇㅇㅇㅇㅇㅇㅇ
            //UpdateProgressBar(nameof(KeyFrameReductionPass), 3);
            //KeyFrameReductionPass();

            if (option.EnableAccurateEndPointNodes)
            {
                UpdateProgressBar(nameof(CalculateEndPointNode), 4);
                CalculateEndPointNodePass();
            }
        }
    }
}