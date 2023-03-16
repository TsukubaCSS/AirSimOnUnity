using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraStabilizer : MonoBehaviour
{
    private  List<float> _rollDegrees = new();
    private float _rollAveragePeriod;

    private void Update()
    {
        var angles = transform.eulerAngles;

        _rollDegrees.Add((angles.z > 180.0f) ? angles.z - 360.0f : angles.z);
        _rollAveragePeriod += Time.deltaTime;
        if (_rollAveragePeriod > 0.15f)
        {
            _rollDegrees.RemoveAt(0);
        }
        var averageZ = _rollDegrees.Sum() / _rollDegrees.Count();

        transform.eulerAngles = new Vector3(0, angles.y, averageZ);
    }
}
