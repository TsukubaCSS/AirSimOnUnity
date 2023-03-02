using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LiveViewCameraFrame : MonoBehaviour
{
    [SerializeField]
    private float _maximizeScaleValue = 3.48f;
    [SerializeField]
    private Vector2 _maximizePositionOffset;


    private Vector2 _firstPosition;
    private bool _maximized;

    private void Awake()
    {
        _firstPosition = transform.localPosition;
    }

    public bool Maximize
    {
        get => _maximized;
        set
        {
            if (value)
            {
                transform.localScale = Vector3.one * _maximizeScaleValue;
                transform.localPosition = _firstPosition + _maximizePositionOffset;
            }
            else
            {
                transform.localScale = Vector3.one;
                transform.localPosition = _firstPosition;
            }

            _maximized = value;
        }
    }

    public void ToggleMaximize()
    {
        Maximize = !Maximize;
    }

    public void OnPress(InputAction.CallbackContext content)
    {
        if (!content.performed)
        {
            return;
        }

        ToggleMaximize();
    }
}
