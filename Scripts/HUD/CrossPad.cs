using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class CrossPad : MonoBehaviour
{
    [SerializeField]
    private Slider _horizontalSlider;
    [SerializeField]
    private Slider _verticalSlider;

    private float _length;


    private void Awake()
    {
        _length = _horizontalSlider.GetComponent<RectTransform>().sizeDelta.x;
    }


    public void SetValues(Vector2 position)
    {
        _horizontalSlider.transform.localPosition = new Vector3(0, _length / 2 * position.y);
        _verticalSlider.transform.localPosition = new Vector3(_length / 2 * position.x, 0);
        _verticalSlider.value = (position.y + 1.0f) / 2.0f;
        // !!!
    }
}
