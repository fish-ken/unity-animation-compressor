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
        var clipCount = aniComp.GetClipCount();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayAni(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAni(1);
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