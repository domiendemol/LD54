using System;
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
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private AudioSource _hitAudioSource;

    private Rigidbody2D _rb;
    private Vector3 _angle;
    private float _curSteerSpeed;
    private Quaternion _rotation;
    private Vector2 _carNormal;
    private bool _parked;
    private Vector3 _startPos;
    private Quaternion _startRotation;
    
    public bool Parked => _parked;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _angle.z = transform.rotation.eulerAngles.z;
        _startPos = transform.position;
        _startRotation = transform.rotation;
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
                _curSteerSpeed += 0.015f;

            //This will make changing direction better and easier
            _speed += (_speed < 0) ? _acceleration / 15 : _acceleration / 30;
            
            _speed = Mathf.Clamp(_speed, -_maxSpeed, _maxSpeed);
            _rb.velocity = _carNormal * _speed;
        }
        // Backward
        else if (Input.GetAxis("Vertical") < 0)
        {
            if (_speed > 0) // braking
            {
                _speed -= _brakeSpeed / 30;
            }
            else if (_speed < 0) // backwards
            {
                _speed -= _acceleration / 30;
            }
            else 
            {
                _speed -= -_deceleration / 30;
            }
            
            _speed = Mathf.Clamp(_speed, -_maxSpeed, _maxSpeed);
            _rb.velocity = _carNormal * _speed;
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            _speed -= (_speed > 0) ? _deceleration / 10 : _speed += _deceleration / 10;
            
            if (Mathf.Abs(_speed) > 0.01) _speed = 0;
            _rb.velocity = _carNormal * _speed;
        }

        _curSteerSpeed = _curve.Evaluate(Mathf.Abs(_speed) / _maxSpeed) * _maxSteerSpeed;
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

    private void OnCollisionEnter2D(Collision2D c)
    {
        _hitAudioSource.Play();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // check rotation!
        float angle = Math.Abs(Mathf.Repeat(transform.eulerAngles.z + 180, 360) - 180);
        if (other.transform.name.Contains("spot") && (180 - angle < 5 || angle < 5) && _speed < 0.05f)
        {
            _parked = true;
        }
    }

    public void Reset()
    {
        _parked = false;

        transform.position = _startPos;
        transform.rotation = _startRotation;
        _angle = Vector3.zero;
        _angle.z = transform.rotation.eulerAngles.z;
    }
} 
