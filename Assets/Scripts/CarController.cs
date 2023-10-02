using UnityEngine;
using UnityEngine.Serialization;

public class CarController : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    [SerializeField] private float _maxSteerSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _brakeSpeed;

    private Rigidbody2D _rb;
    private Vector3 _angle;
    private float _curSteerSpeed;
    private Quaternion _rotation;
    private Vector2 _carNormal;

    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _angle.z = transform.rotation.eulerAngles.z;
    }

    void FixedUpdate()
    {
        _carNormal = new Vector2(Mathf.Sin((-_angle.z) * Mathf.Deg2Rad), Mathf.Cos((-_angle.z) * Mathf.Deg2Rad));
        _rotation.eulerAngles = _angle;
        transform.rotation = _rotation;
        // Debug.Log ("["+ (-_angle.z)+"] "+"("+_carNormal.x + ", " + _carNormal.y + ")");

        HandleForwardMotion();
        HandleSteering();
    }

    private void HandleForwardMotion()
    {
        // Forward
        if (Input.GetAxis("Vertical") > 0)
        {
            if (_curSteerSpeed < _maxSteerSpeed)
                _curSteerSpeed += 0.02f;

            //This will make changing direction better and easier
            _speed += (_speed < 0) ? _speed += _acceleration / 15 : _acceleration / 30;
            
            _speed = Mathf.Clamp(_speed, -_maxSpeed, _maxSpeed);
            _rb.velocity = _carNormal * _speed;
        }
        // Backward
        else if (Input.GetAxis("Vertical") < 0)
        {
            if (_speed > 0) // braking
            {
                _speed -= _brakeSpeed / 30;
                if (_curSteerSpeed > 0) _curSteerSpeed -= 0.03f;
            }
            else if (_speed < 0) // backwards
            {
                _speed -= _acceleration / 30;
                if (_curSteerSpeed < _maxSteerSpeed) _curSteerSpeed += 0.035f;
            }
            else 
            {
                _speed -= -_deceleration / 30;
                if (_curSteerSpeed < _maxSteerSpeed) _curSteerSpeed += 0.01f;
            }
            
            _speed = Mathf.Clamp(_speed, -_maxSpeed, _maxSpeed);
            _rb.velocity = _carNormal * _speed;
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            if (_curSteerSpeed > 0) _curSteerSpeed -= 0.02f;
            _speed -= (_speed > 0) ? _deceleration / 10 : _speed += _deceleration / 10;
            
            if (Mathf.Abs(_speed) > 0.01) _speed = 0;
            
            _rb.velocity = _carNormal * _speed;
        }

        if (_speed < 0.1 && _speed > -0.1)
            _curSteerSpeed = 0;
    }

    private void HandleSteering()
    {
        //Steering Motion
        if (_speed != 0)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (_speed > 0)
                    _angle += new Vector3(0, 0, -_curSteerSpeed);
                else if (_speed < 0)
                    _angle += new Vector3(0, 0, _curSteerSpeed);
                _rotation.eulerAngles = _angle;
                transform.rotation = _rotation;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (_speed > 0)
                    _angle += new Vector3(0, 0, +_curSteerSpeed);
                if (_speed < 0)
                    _angle += new Vector3(0, 0, -_curSteerSpeed);
                _rotation.eulerAngles = _angle;
                transform.rotation = _rotation;
            }
        }
    }
} 
