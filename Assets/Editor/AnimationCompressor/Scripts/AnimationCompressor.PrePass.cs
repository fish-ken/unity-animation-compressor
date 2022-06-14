using System.Collections.Generic;
using UnityEditor;

namespace AnimationCompressor
{
    public partial class Core
    {
        private Dictionary<string, Bone> boneMap = new Dictionary<string, Bone>();
        private int maxDepth = -1;

        private void GenerateBoneMapPass()
        {
            boneMap.Clear();

            // Create bone map
            var bindings = AnimationUtility.GetCurveBindings(originClip);
            foreach (var binding in bindings)
            {
                var path = binding.path;
                var propertyName = binding.propertyName;
                var curve = AnimationUtility.GetEditorCurve(originClip, binding);
                Bone bone;

                if (boneMap.ContainsKey(path) == false)
                    boneMap[path] = new Bone(path);

                bone = boneMap[path];
                bone.SetCurve(propertyName, curve);
            }

            // Set parent
            foreach (var bone in boneMap.Values)
            {
                var upperPath = Util.GetUpperDepth(bone.Path);

                while (string.IsNullOrEmpty(upperPath) == false)
                {
                    if (boneMap.ContainsKey(upperPath))
                    {
                        bone.SetParent(boneMap[upperPath]);
                        break;
                    }

                    upperPath = Util.GetUpperDepth(bone.Path);
                }
            }
        }
    }
}