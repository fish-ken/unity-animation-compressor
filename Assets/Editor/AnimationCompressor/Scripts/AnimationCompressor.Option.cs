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
        /// Allowable range position error
        /// </summary>
        public float PositionAllowError { get; set; } = 0.02f;

        /// <summary>
        /// Allowable range rotation error
        /// </summary>
        public float RotationAllowError { get; set; } = 0.01f;

        /// <summary>
        /// Allowable range scale error
        /// </summary>
        public float ScaleAllowError { get; set; } = 0.05f;

        /// <summary>
        /// Improved accuracy for endpoints such as feet and hands
        /// </summary>
        public bool AccurateEndPointNodes { get; set; } = true;

        /// <summary>
        /// Endpoint computation tolerance
        /// if 3, Allow up to 3 layers from the end
        /// </summary>
        public int EndPointNodesDepthMin { get; set; } = 4;

        /// <summary>
        /// Endpoint computation tolerance
        /// if 3, Allow up to 3 layers from the end
        /// </summary>
        public int EndPointNodesDepthMax { get; set; } = 10;

        public void OnGUI()
        {
            Logging = EditorGUILayout.Toggle(nameof(Logging), Logging);
            PositionAllowError = EditorGUILayout.FloatField(nameof(PositionAllowError), PositionAllowError);
            RotationAllowError = EditorGUILayout.FloatField(nameof(RotationAllowError), RotationAllowError);
            ScaleAllowError = EditorGUILayout.FloatField(nameof(ScaleAllowError), ScaleAllowError);
            AccurateEndPointNodes = EditorGUILayout.Toggle(nameof(AccurateEndPointNodes), AccurateEndPointNodes);
            EndPointNodesDepthMin = EditorGUILayout.IntField(nameof(EndPointNodesDepthMin), EndPointNodesDepthMin);
            EndPointNodesDepthMax = EditorGUILayout.IntField(nameof(EndPointNodesDepthMax), EndPointNodesDepthMax);
        }
    }
}