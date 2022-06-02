using UnityEditor;

namespace AnimationCompressor
{
    public class Menu
    {
        [MenuItem("Assets/Optimization/Animation/Animation Compressor/Open")]
        private static void OpenMaterialRefCleaner()
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