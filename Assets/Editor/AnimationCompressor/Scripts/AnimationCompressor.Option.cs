using UnityEditor;

namespace AnimationCompressor
{
    public class Option
    {
        /// <summary>
        /// Show compression log in debug console
        /// </summary>
        public bool Logging { get; set; } = true;

        /// <summary>
        /// TODO : Improved accuracy for endpoints such as feet and hands
        /// </summary>
        public bool AccurateEndPointNode { get; set; } = false;

        public void OnGUI()
        {
            Logging = EditorGUILayout.Toggle(nameof(Logging), Logging);
            AccurateEndPointNode = EditorGUILayout.Toggle(nameof(AccurateEndPointNode), AccurateEndPointNode);
        }
    }
}