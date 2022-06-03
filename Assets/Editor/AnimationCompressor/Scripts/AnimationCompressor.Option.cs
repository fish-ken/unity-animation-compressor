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
        public float PositionAllowError { get; set; } = 0.1f;

        /// <summary>
        /// Allowable range rotation error
        /// </summary>
        public float RotationAllowError { get; set; } = 0.1f;

        /// <summary>
        /// Allowable range scale error
        /// </summary>
        public float ScaleAllowError { get; set; } = 0.3f;

        /// <summary>
        /// TODO : Improved accuracy for endpoints such as feet and hands
        /// </summary>
        //public bool AccurateEndPointNode { get; set; } = false;

        public void OnGUI()
        {
            Logging = EditorGUILayout.Toggle(nameof(Logging), Logging);
            PositionAllowError = EditorGUILayout.FloatField(nameof(PositionAllowError), PositionAllowError);
            RotationAllowError = EditorGUILayout.FloatField(nameof(RotationAllowError), RotationAllowError);
            ScaleAllowError = EditorGUILayout.FloatField(nameof(ScaleAllowError), ScaleAllowError);

            //AccurateEndPointNode = EditorGUILayout.Toggle(nameof(AccurateEndPointNode), AccurateEndPointNode);
        }
    }
}