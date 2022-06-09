using UnityEditor;

namespace AnimationCompressor
{
    public class Option
    {
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
        public int EndPointNodesRange { get; set; } = 4;

        public void OnGUI()
        {
            PositionAllowError = EditorGUILayout.FloatField(nameof(PositionAllowError), PositionAllowError);
            RotationAllowError = EditorGUILayout.FloatField(nameof(RotationAllowError), RotationAllowError);
            ScaleAllowError = EditorGUILayout.FloatField(nameof(ScaleAllowError), ScaleAllowError);
            EndPointNodesRange = EditorGUILayout.IntField(nameof(EndPointNodesRange), EndPointNodesRange);
        }
    }
}