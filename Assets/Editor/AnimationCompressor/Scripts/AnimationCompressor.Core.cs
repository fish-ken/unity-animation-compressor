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
            compressClip = Object.Instantiate(originClip);
            compressClip.ClearCurves();

            PreCompress();
            Compress();

            if(File.Exists(outputPath))
                AssetDatabase.DeleteAsset(outputPath);

            AssetDatabase.CreateAsset(compressClip, outputPath);
            AssetDatabase.SaveAssets();
        }

        private void PreCompress()
        {
            GenerateBoneMapPass();
        }

        private void Compress()
        {
            CompressByKeyframeReductionPass();
        }
    }
}