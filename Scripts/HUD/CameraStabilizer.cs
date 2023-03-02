using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStabilizer : MonoBehaviour
{
    private void Update()
    {
        var angles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, angles.y, angles.z);
    }
}
