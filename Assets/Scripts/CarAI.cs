using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public enum CarState
    {
        PARKED,
        CIRCLING,
        IN,
        OUT
    }
    
    [SerializeField] private CarState _startState;
    [SerializeField] private Transform _previousNode;
    [SerializeField] private Transform _targetNode;
    [SerializeField] private Transform _front;
    private Path _path;
    private CarState _carState;
    private ParkingSpace _parkingSpace;
    private bool _goingHome;

    public ParkingSpace ParkingSpace
    {
        get => _parkingSpace;
        set => _parkingSpace = value;
    }

    private void Awake()
    {
        _path = FindObjectOfType<Game>().MainPath;
        _previousNode = transform;
        _carState = _startState;
    }
    
    private void Update()
    {
        if (_carState == CarState.CIRCLING)
        {
            bool stop = false;
            RaycastHit2D hit = Physics2D.Raycast(_front.position, _front.right, 2f, LayerMask.GetMask("Car"));
            if (hit && hit.collider != null)
            {
                if (hit.collider.GetComponent<CarController>() != null) stop = true; 
                // if (hit.collider.GetComponent<CarController>() != null) Debug.Log($"Distance: {hit.distance}");
                CarAI car = hit.transform.GetComponent<CarAI>();
                if (car != null && car != this && car.State != CarState.PARKED) stop = true;
            }
            
            if (!stop) FollowPath(true);
        }
        else if (_carState == CarState.IN)
        {
            FollowPath(false);
        }
        else if (_carState == CarState.OUT)
        {
            FollowPath(false, true);
        }
    }

    public void LeaveSpace()
    {
        if (_carState != CarState.PARKED) return;
        _path = _parkingSpace.OutPath;
        _previousNode = transform;
        _targetNode = null;
        _carState = CarState.OUT;
        _goingHome = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_carState == CarState.CIRCLING && !_goingHome && 
            other.gameObject.name.Contains("Node") && other.gameObject.GetComponentInParent<ParkingSpace>().Free)
        {
            _carState = CarState.IN;
            _previousNode = transform;
            _targetNode = null;
            _path = other.gameObject.GetComponentInParent<Path>();
            _parkingSpace = other.gameObject.GetComponentInParent<ParkingSpace>();
            _parkingSpace.Free = false;
        }
    }

    private void FollowPath(bool loop, bool reverse = false)
    {
        if (_path == null) return;
        
        if (_targetNode == null)
        {
            // find closest node in front of car
            List<Transform> nodes = _path.Nodes.Where(n => IsInFront(n, 60f, reverse)).ToList();
            nodes = nodes.Where(n => (n.transform.position - transform.position).magnitude > 0.1).ToList();
            TransformSorter.SortByDistance(ref nodes, gameObject);
            if (nodes.Count == 0) return;

            _targetNode = nodes.First();
        }
        else if (Vector2.Distance(transform.position, _targetNode.position) < 0.1)
        {
            _previousNode = _targetNode;
            _targetNode = _path.GetNext(_previousNode, loop, reverse);
            if (_targetNode == null)
            {
                if (_carState == CarState.IN) _carState = CarState.PARKED;
                if (_carState == CarState.OUT)
                {
                    _carState = CarState.CIRCLING;
                    _parkingSpace.Free = true;
                    _path = FindObjectOfType<Game>().MainPath;
                    _previousNode = transform;
                }
                return;
            }
        }

        // move there
        transform.position += Vector3.Normalize(_targetNode.position - transform.position) * 2 * Time.deltaTime;
        // rotate towards node
        transform.GetChild(0).rotation = Quaternion.Lerp(_previousNode.rotation, _targetNode.rotation,
            Vector2.SqrMagnitude(_targetNode.position - _previousNode.position) /
            Vector2.SqrMagnitude(_targetNode.position - transform.position));
    }

    public bool IsInFront(Transform objectTransform, float coneOfVisionAngle, bool reverse)
    {
        // Get the direction vector from the transform to the object.
        Vector3 direction = objectTransform.position - transform.position;

        // Calculate the angle between the transform's forward vector and the direction vector.
        float angle = Vector2.Angle(reverse ? -_front.right : _front.right, direction);
        angle %= 360f;
        angle = angle > 180 ? angle - 360 : angle < -180 ? angle + 360 : angle;
        // Return true if the angle is less than the cone of vision angle.
        return angle < coneOfVisionAngle;
    }

    public CarState State
    {
        get => _carState;
        set => _carState = value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_targetNode != null) Gizmos.DrawLine(transform.position, _targetNode.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_front.position, _front.position + _front.right*2);
    }
}