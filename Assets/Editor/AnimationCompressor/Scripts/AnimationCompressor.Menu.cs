using UnityEditor;

namespace AnimationCompressor
{
    public class Menu
    {
        [MenuItem("Tools/Optimization/Animation/Animation Compressor/Open")]
        public static void OpenMaterialRefCleaner()
        {
            ShowWindow();
        }

        private static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<Window>("Anim. Compressor");
            window.Show();
        }
    }
}