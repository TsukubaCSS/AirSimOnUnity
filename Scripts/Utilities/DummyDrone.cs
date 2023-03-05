using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyDrone : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _rotors;


    private void Start()
    {
    }

    private void Update()
    {
        foreach (var rotor in _rotors)
        {
            rotor.transform.Rotate(Vector3.up, 123.4f, Space.Self);
        }
    }
}
