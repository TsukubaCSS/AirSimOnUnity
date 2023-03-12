using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JoysticksPanel : MonoBehaviour
{
    [SerializeField]
    private CrossPad _leftCrossPad;
    [SerializeField]
    private CrossPad _rightCrossPad;


    public void SetStickValues(Vector2 left, Vector2 right)
    {
        _leftCrossPad.SetValues(left);
        _rightCrossPad.SetValues(right);
    }
}
