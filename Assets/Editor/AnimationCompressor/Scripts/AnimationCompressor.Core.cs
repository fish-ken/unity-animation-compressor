using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public partial class Core
    {
        private Option option = null;
        private AnimationClip originClip = null;
        private AnimationClip compressClip = null;

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

            //if (File.Exists(outputPath))
            //{
            //    var exist = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);
            //    EditorUtility.CopySerialized(exist, compressClip);
            //}

            compressClip = AssetDatabase.LoadMainAssetAtPath(outputPath) as AnimationClip;
            var isOutputExist = compressClip != null;

            if (isOutputExist)
            {
                EditorUtility.CopySerialized(originClip, compressClip);
                //AssetDatabase.SaveAssets();
            }
            else
            {
                compressClip = Object.Instantiate(originClip);
                //AssetDatabase.CreateAsset(compressClip, outputPath);
            }

            EditorUtility.CopySerialized(originClip, compressClip);
            compressClip.ClearCurves();

            PreCompress();
            Compress();

            if (isOutputExist)
                AssetDatabase.SaveAssets();
            else
                AssetDatabase.CreateAsset(compressClip, outputPath);

            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void PreCompress()
        {
            GenerateBoneMapPass();
        }

        private void Compress()
        {
            GenerateKeyFrameByCurveFittingPass();
            KeyFrameReductionPass();

            if (option.EnableAccurateEndPointNodes)
                CalculateEndPointNode();
        }
    }
}