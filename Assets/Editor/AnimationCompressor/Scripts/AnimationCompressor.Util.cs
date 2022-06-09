using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public static class Util
    {
        /// <summary>
        /// Get animation curve's path depth
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetDepth(string path)
        {
            var matches = Regex.Matches(path, "/");
            return matches.Count;
        }

        /// <summary>
        /// Get upper depth
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUpperDepth(string path)
        {
            if (path.Contains("/") == false)
                return null;

            var idx = path.LastIndexOf('/');
            var cnt = path.Length - idx;
            return path.Remove(idx, cnt);
        }

        /// <summary>
        /// Get destination output path
        /// </summary>
        /// <param name="originClip"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string GetOutputPath(AnimationClip originClip)
        {
            if (originClip == null)
                return string.Empty;

            // From asdf/asdf/asdf@atk.FBX
            var originPath = AssetDatabase.GetAssetPath(originClip);
            var split = originPath.Split('.');

            // To asdf/asdf/asdf@atk
            var builder = new StringBuilder();
            for (var i = 0; i < split.Length - 1; i++)
                builder.Append(split[i]);

            // To asdf/asdf/asdf@atk_Compressed.anim
            builder.Append("_Compressed.anim");
            return builder.ToString();
        }
    }
}