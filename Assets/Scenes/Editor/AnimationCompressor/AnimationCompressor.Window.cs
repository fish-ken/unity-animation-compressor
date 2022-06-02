using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AnimationCompressor
{
    public class Window : EditorWindow
    {
        private AnimationClip targetClip = null;

        private Option option = new Option();

        private void OnGUI()
        {
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

        }
    }
}
