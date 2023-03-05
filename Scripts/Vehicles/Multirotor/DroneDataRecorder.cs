using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DroneDataRecorder : MonoBehaviour
{
    [SerializeField]
    private bool _recording;

    [SerializeField]
    private string _filePath;
    [SerializeField]
    private float _recordInterval;

    private float _totalElapsed;
    private StreamWriter _writer;
    private float _elapsed;

    private const string FLOAT_FORMAT = "0.0000";


    private void Start()
    {
        if (_recording)
        {
            Record();
        }
    }
    private void OnApplicationQuit()
    {
        Stop();
    }

    public void Record()
    {
        _elapsed = 0;
        _writer = new StreamWriter(File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.Write));
        _recording = true;
    }
    public void Stop()
    {
        if (_writer != null)
        {
            _writer.Close();
            _writer = null;
            _recording = false;
        }
    }

    private void FixedUpdate()
    {
        var deltaTime = Time.deltaTime;
        _totalElapsed += deltaTime;

        if (_recording)
        {
            _elapsed += deltaTime;
            if (_elapsed > _recordInterval)
            {
                _elapsed -= _recordInterval;

                var position = transform.position;
                var angles = transform.eulerAngles;
                _writer.WriteLine($"{_totalElapsed}\t{position.x.ToString(FLOAT_FORMAT)}\t{position.y.ToString(FLOAT_FORMAT)}\t{position.z.ToString(FLOAT_FORMAT)}\t{angles.x.ToString(FLOAT_FORMAT)}\t{angles.y.ToString(FLOAT_FORMAT)}\t{angles.z.ToString(FLOAT_FORMAT)}");
            }
        }
    }
}
