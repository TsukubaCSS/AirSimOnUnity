using System;
using System.Collections.Generic;
using AirSimUnity.DroneStructs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AirSimUnity {
    /*
     * Drone component that is used to control the drone object in the scene. This is based on Vehicle class that is communicating with AirLib.
     * This class depends on the AirLib's drone controllers based on FastPhysics engine. The controller is being used based on setting.json file in Documents\AirSim
     * The drone can be controlled either through keyboard or through client api calls.
     * This data is being constantly exchanged between AirLib and Unity through PInvoke delegates.
     */
    public class Drone : Vehicle {
        public Transform[] rotors;
        private List<RotorInfo> rotorInfos = new List<RotorInfo>();
        private float rotationFactor = 0.1f;
        [SerializeField]
        private PlayerInput playerInput;
        private Vector3 _previousPosition;

        [SerializeField]
        private float _throttleUpVelocity;
        [SerializeField]
        private float _throttleDownpVelocity;
        [SerializeField]
        private float _moveVelocity;
        [SerializeField]
        private float _rotateSpeedRatio;

        [SerializeField]
        private Vector3 _throttlePID;
        [SerializeField]
        private Vector3 _movePID;


        private class PIControl
        {
            private float _gainP;
            private float _gainI;
            private float _gainD;
            private float _iTotal;
            private float _previousDiff;

            public float Calculate(float target, float current)
            {
                var diff = target - current;
                _iTotal += diff;
                var value = (_gainP * diff) + (_gainI * _iTotal) + (_gainD * (diff - _previousDiff));
                _previousDiff = diff;
                return value;
            }

            public PIControl(float gainP, float gainI, float gainD)
            {
                _gainP = gainP;
                _gainI = gainI;
                _gainD = gainD;
            }

            public PIControl(Vector3 gains)
            {
                _gainP = gains.x;
                _gainI = gains.y;
                _gainD = gains.z;
            }
        }

        private PIControl _throttleControl;
        private PIControl _rollControl;
        private PIControl _pitchControl;

        private new void Start() {
            base.Start();

            _throttleControl = new(_throttlePID);
            _rollControl = new(_movePID);
            _pitchControl = new(_movePID);

            for (int i = 0; i < rotors.Length; i++) {
                rotorInfos.Add(new RotorInfo());
            }
        }

        private new void FixedUpdate() {
            if (isServerStarted)
            {
                if (resetVehicle)
                {
                    rcData.Reset();
                    currentPose = poseFromAirLib;
                    resetVehicle = false;
                }

                base.FixedUpdate();

                DataManager.SetToUnity(poseFromAirLib.position, ref position);
                DataManager.SetToUnity(poseFromAirLib.orientation, ref rotation);

                transform.SetPositionAndRotation(position, rotation);

                currentPose = poseFromAirLib;

                for (int i = 0; i < rotors.Length; i++)
                {
                    float rotorSpeed = (float) (rotorInfos[i].rotorSpeed * rotorInfos[i].rotorDirection * 180 /
                                                Math.PI * rotationFactor);
                    rotors[i].Rotate(Vector3.up, rotorSpeed * Time.deltaTime, Space.Self);
                }

                UpdateRCData();
            }
        }

        private void UpdateRCData()
        {
            var joystickNames = Input.GetJoystickNames();
            rcData.is_valid = (joystickNames.Length > 0) && (joystickNames[0].Trim().Length > 0);


            if (rcData.is_valid)
            {
                var leftStick = IgnoreTinyValue(playerInput.currentActionMap["Move"].ReadValue<Vector2>());
                var rightStick = IgnoreTinyValue(playerInput.currentActionMap["Look"].ReadValue<Vector2>().normalized);

                var position = transform.position;
                var velocity = (position - _previousPosition) / Time.deltaTime;
                var localVelocity = Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0) * velocity;
                _previousPosition = position;

                var targetThrottleVelocity = ((leftStick.y > 0) ? _throttleUpVelocity : _throttleDownpVelocity) * leftStick.y;

                rcData.throttle = _throttleControl.Calculate(targetThrottleVelocity, velocity.y);
                rcData.yaw = _rotateSpeedRatio * leftStick.x;

                var targetRollVelocity = _moveVelocity * rightStick.x;
                var targetPitchVelocity = _moveVelocity * rightStick.y;
                rcData.roll = _rollControl.Calculate(targetRollVelocity, localVelocity.x);
                rcData.pitch = _pitchControl.Calculate(targetPitchVelocity, localVelocity.z);

#if false
                //-1 to 1 --> 0 to 1
                var leftStick = playerInput.currentActionMap["Move"].ReadValue<Vector2>();
                var rightStick = playerInput.currentActionMap["Look"].ReadValue<Vector2>();
                rcData.throttle = (leftStick.y + 1) / 2;

                //-1 to 1
                rcData.yaw = leftStick.x;
                rcData.roll = rightStick.x;
                rcData.pitch = rightStick.y;

                //these will be available for devices like steering wheels
                //rcData.left_z = Input.GetButton("LeftZ") ? 1.0f : 0;
                //rcData.right_z = Input.GetButton("RightZ") ? 1.0f : 0;

                //rc_data_.switches = joystick_state_.buttons;
#endif
            }
        }

        private Vector2 IgnoreTinyValue(Vector2 value)
        {
            var x = Mathf.Abs(value.x) < 0.2f ? 0 : value.x;
            var y = Mathf.Abs(value.y) < 0.2f ? 0 : value.y;
            return new Vector2(x, y);
        }

        private new void LateUpdate() {
            if (isServerStarted)
            {
                //Image capture is being done in base class
                base.LateUpdate();
            }
        }

        public KinemticState GetKinematicState() {
            return airsimInterface.GetKinematicState();
        }

#region IVehicleInterface implementation

        // Sets the animation for rotors on the drone. This is being done by AirLib through Pinvoke calls
        public override bool SetRotorSpeed(int rotorIndex, RotorInfo rotorInfo) {
            rotorInfos[rotorIndex] = rotorInfo;
            return true;
        }

        //Gets the data specific to drones for saving in the text file along with the images at the time of recording
        public override DataRecorder.ImageData GetRecordingData() {
            DataRecorder.ImageData data;
            data.pose = currentPose;
            data.carData = new CarStructs.CarData();
            data.image = null;
            return data;
        }

#endregion IVehicleInterface implementation
    }
}