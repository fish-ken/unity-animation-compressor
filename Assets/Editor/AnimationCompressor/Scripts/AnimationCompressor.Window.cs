using UnityEditor;
using UnityEngine;

namespace AnimationCompressor
{
    public class Window : EditorWindow
    {
        private Core core = null;
        private Option option = new Option();
        private AnimationClip targetClip = null;

        private void OnGUI()
        {
            DrawTargetClip();
            DrawOption();
            DrawButtons();
        }

        private void DrawTargetClip()
        {
            targetClip = (AnimationClip)EditorGUILayout.ObjectField("Target AnimationClip", targetClip, typeof(AnimationClip), true);
        }

        private void DrawOption()
        {
            option.OnGUI();
        }

        private void DrawButtons()
        {
            if (GUILayout.Button("Compress", GUILayout.Height(50)))
            {
                if (targetClip == null)
                    return;

                if (core == null)
                    core = new Core();

                core.Compress(targetClip, option);
            }
        }
    }
}