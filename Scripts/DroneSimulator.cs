using AirSimUnity;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Text;
using System;
using PlasticPipe.PlasticProtocol.Server;

public class DroneSimulator : MonoBehaviour
{
    private TcpClient _client;


    private void Awake()
    {
        AirSimSettings.Initialize();
        AirSimSettings.GetSettings().SimMode = "Multirotor";
    }

    public void ResetDrone()
    {
        _client = new TcpClient
        {
            SendTimeout = 10,
            ReceiveTimeout = 1
        };
        _client.Connect("127.0.0.1", 41451);

        var pingData = new byte[] { 0x94, 0x00, 0x00, 0xa4, 0x70, 0x69, 0x6e, 0x67, 0x90 };
        Write(pingData);

        var resetData = new byte[] { 0x94, 0x00, 0x05, 0xa5, 0x72, 0x65, 0x73, 0x65, 0x74, 0x90 };
        Write(resetData);
    }
    public void SetWind(Vector3 direction)
    {
        var x = (byte)Mathf.CeilToInt(direction.x);
        var y = (byte)Mathf.CeilToInt(direction.y);
        var z = (byte)Mathf.CeilToInt(direction.z);
        var data = new byte[] {
            0x94, 0x00, 0x05, 0xaa, 0x73, 0x69, 0x6d, 0x53, 0x65, 0x74, 0x57, 0x69, 0x6e, 0x64, 0x91, 0x83, 
            0xa5, 0x78, 0x5f, 0x76, 0x61, 0x6c, z,
            0xa5, 0x79, 0x5f, 0x76, 0x61, 0x6c, x,
            0xa5, 0x7a, 0x5f, 0x76, 0x61, 0x6c, y, 
        };
        Write(data);
    }

    private void Write(byte[] data)
    {
        var stream = _client.GetStream();
        stream.Write(data);

        var buffer = new byte[256];
        stream.Read(buffer, 0, buffer.Length);
    }
}
