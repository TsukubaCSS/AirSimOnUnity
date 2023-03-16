using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraStabilizer : MonoBehaviour
{
    private List<float> _rollDegrees = new();

    private void Update()
    {
        var angles = transform.eulerAngles;

        _rollDegrees.Add((angles.z > 180.0f) ? angles.z - 360.0f : angles.z);
        if (_rollDegrees.Count > 5)
        {
            _rollDegrees.RemoveAt(0);
        }
        var averageZ = _rollDegrees.Sum() / _rollDegrees.Count;

        transform.eulerAngles = new Vector3(0, angles.y, averageZ);
    }
}
