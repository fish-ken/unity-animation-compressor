using UnityEngine;

public class AnimationEventChecker : MonoBehaviour
{
    public void SomeEvent(string param)
    {
        Debug.Log($"{nameof(SomeEvent)} - {param}");
    }
}