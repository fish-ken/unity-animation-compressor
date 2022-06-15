using System.IO;
using UnityEditor;
using UnityEditor.Experimental;
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
            compressClip = Object.Instantiate(originClip);

            if (File.Exists(outputPath))
            {
                var exist = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);
                EditorUtility.CopySerialized(exist, compressClip);
            }

            compressClip.ClearCurves();

            PreCompress();
            Compress();

            
            AssetDatabase.CreateAsset(compressClip, outputPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //File.Move(outputPath, )
        }

        private void PreCompress()
        {
            GenerateBoneMapPass();
        }

        private void Compress()
        {
            GenerateKeyFrameByCurveFittingPass();
        }
    }
}