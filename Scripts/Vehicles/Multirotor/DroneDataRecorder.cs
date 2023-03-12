using AirSimUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneDataRecorder : MonoBehaviour
{
    [SerializeField]
    private bool _recording;

    [SerializeField]
    private string _filePath;
    [SerializeField]
    private float _recordInterval;

    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private PlayerInput _playerInput;
    [SerializeField]
    private SaveRenderTexture _saveRenderTexture;
    [SerializeField]
    private LiveViewCameraFrame _liveViewCameraFrame;
    [SerializeField]
    private Camera _liveViewCamera;

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
        _writer = new StreamWriter(File.Open(_filePath, FileMode.Create, FileAccess.Write));
        _writer.WriteLine("#t\tpx\tpy\tpz\trx\try\trz\tthrottle\tyaw\troll\tpitch\tview\tzoom\tcapture\tpower");
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
                var positionAngles = $"{_totalElapsed}\t{position.x.ToString(FLOAT_FORMAT)}\t{position.y.ToString(FLOAT_FORMAT)}\t{position.z.ToString(FLOAT_FORMAT)}\t{angles.x.ToString(FLOAT_FORMAT)}\t{angles.y.ToString(FLOAT_FORMAT)}\t{angles.z.ToString(FLOAT_FORMAT)}";

                var leftStick = _playerInput.currentActionMap["Move"].ReadValue<Vector2>();
                var rightStick = _playerInput.currentActionMap["Look"].ReadValue<Vector2>().normalized;
                int view = _liveViewCameraFrame.Maximize ? 1 : 0;
                float fov = _liveViewCamera.fieldOfView;
                int captureCount = _saveRenderTexture.CaptureCount;
                int power = (int)_drone.PowerState;
                var controls = $"{leftStick.y}\t{leftStick.x}\t{rightStick.x}\t{rightStick.y}\t{view}\t{fov}\t{captureCount}\t{power}";
                _writer.WriteLine($"{positionAngles}\t{controls}");
            }
        }
    }
}
