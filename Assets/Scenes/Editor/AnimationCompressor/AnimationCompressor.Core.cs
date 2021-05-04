using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Core
    {
        // TODO : Something
        public void Compress(AnimationClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AnimationCompressor.Core.Compress - AnimationClip is null");
                return;
            }

            var fileName = $"{clip.name}_Compressed";
            var compressedClip = Object.Instantiate<AnimationClip>(clip);

            var curves = AnimationUtility.GetAllCurves(clip);


            AssetDatabase.CreateAsset((Object)Object.Instantiate<AnimationClip>((M0)animClip), outputPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.DeleteAsset(fileName);

        }
    }
}
