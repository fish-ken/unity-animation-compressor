using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSphere : MonoBehaviour
{
    public GameObject sphere;

    private void Awake()
    {
        if (sphere == null)
            return;

        foreach(var tr in GetComponentsInChildren<Transform>())
        {
            var copy = Instantiate(sphere, tr);
        }
    }
}
