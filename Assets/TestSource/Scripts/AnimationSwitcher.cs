using System.Collections.Generic;
using UnityEngine;

public class AnimationSwitcher : MonoBehaviour
{
    private Animation aniComp;

    private void Awake()
    {
        aniComp = GetComponent<Animation>();
    }

    private void Update()
    {
        var keys = new List<KeyCode>()
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };

        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                var idx = (int)key - (int)KeyCode.Alpha1;
                PlayAni(idx);
                break;
            }
        }
    }

    private void PlayAni(int idx)
    {
        if (aniComp == null)
            return;

        int i = 0;
        foreach (AnimationState aniState in aniComp)
        {
            if (i == idx)
            {
                aniComp.Play(aniState.clip.name);
                return;
            }
            i++;
        }
    }
}