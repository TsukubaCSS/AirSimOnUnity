using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DronePlayer : MonoBehaviour
{
    [SerializeField]
    private string _filePath;
    [SerializeField]
    private float _playOffset;
    [SerializeField]
    private GameObject _drone;

    private float _totalElapsed;
    private StreamReader _reader;
    private float _previousTime;
    private Vector3 _previousPosition;
    private Vector3 _previousAngles;
    private float _nextTime;
    private Vector3 _nextPosition;
    private Vector3 _nextAngles;


    private void Start()
    {
        _reader = new StreamReader(_filePath);
        // skip first line
        _reader.ReadLine();
        (_nextTime, _nextPosition, _nextAngles) = ParseLine(_reader.ReadLine());
        _totalElapsed = _playOffset;
    }

    private void Update()
    {
        _totalElapsed += Time.deltaTime;

        if (_nextTime < _totalElapsed)
        {
            var line = _reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line) || (line[0] == '#'))
            {
                _nextTime = float.MaxValue;
                return;
            }

            (_previousTime, _previousPosition, _previousAngles) = (_nextTime, _nextPosition, _nextAngles);
            (_nextTime, _nextPosition, _nextAngles) = ParseLine(line);
        }
        else
        {
            var t = (_totalElapsed - _previousTime) / (_nextTime - _previousTime);
            //Debug.Log(_nextTime + ", " + _totalElapsed + ", " + t);
            _drone.transform.position = Vector3.Lerp(_previousPosition, _nextPosition, t);
            _drone.transform.eulerAngles =
                new Vector3(
                    Mathf.LerpAngle(_previousAngles.x, _nextAngles.x, t),
                    Mathf.LerpAngle(_previousAngles.y, _nextAngles.y, t),
                    Mathf.LerpAngle(_previousAngles.z, _nextAngles.z, t));
        }
    }

    private (float, Vector3, Vector3) ParseLine(string line)
    {
        var tokens = line.Split();
        var timestamp = float.Parse(tokens[0]);
        var position = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
        var angles = new Vector3(float.Parse(tokens[4]), float.Parse(tokens[5]), float.Parse(tokens[6]));
        return (timestamp, position, angles);
    }
}
