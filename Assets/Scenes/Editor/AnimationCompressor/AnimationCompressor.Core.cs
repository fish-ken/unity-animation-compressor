using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Option
    {
        /// <summary>
        /// TODO : Improved accuracy for endpoints such as feet and hands
        /// </summary>
        public bool AccurateEndPointNode { get; set; } = false;

        public void OnGUI()
        {
            AccurateEndPointNode = EditorGUILayout.Toggle(nameof(AccurateEndPointNode), AccurateEndPointNode);
        }
    }

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
            
            var copyClip = Object.Instantiate<AnimationClip>(clip);


            var curves = AnimationUtility.GetCurveBindings(clip);

            var outputPath = "";
            AssetDatabase.CreateAsset((Object)copyClip, outputPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.DeleteAsset(fileName);

        }
    }
}
