using UnityEditor;

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
}