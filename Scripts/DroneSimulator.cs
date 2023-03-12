using AirSimUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSimulator : MonoBehaviour
{
    private void Awake()
    {
        AirSimSettings.Initialize();
        AirSimSettings.GetSettings().SimMode = "Multirotor";
    }
}
