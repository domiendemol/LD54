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
        OUT,
        HOME
    }
    
    [SerializeField] private CarState _startState;
    [SerializeField] private Transform _previousNode;
    [SerializeField] private Transform _targetNode;
    [SerializeField] private Transform _front;
    [SerializeField] private Transform _back;
    private Path _path;
    [SerializeField] private CarState _carState;
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
            if (!ShouldStop(false)) FollowPath(true);
        }
        else if (_carState == CarState.IN)
        {
            //if (!ShouldStop(false))
            FollowPath(false);
        }
        else if (_carState == CarState.OUT)
        {
            if (!ShouldStop(true)) FollowPath(false, true);
        }
        else if (_carState == CarState.HOME)
        {
            if (!ShouldStop(false)) transform.position += transform.GetChild(0).right * 2 * Time.deltaTime;
        }
    }

    private bool ShouldStop(bool reverse)
    {
        if (reverse)
        {
            RaycastHit2D hitBack = Physics2D.Raycast(_back.position, _back.right, 0.5f, LayerMask.GetMask("Car"));
            return ShouldStop(hitBack);
        }
        else
        {
            RaycastHit2D hitFront = Physics2D.Raycast(_front.position, _front.right, 2f, LayerMask.GetMask("Car"));
            // TODO these only for other CarState OUT AIs not our car
            RaycastHit2D hitFront2 = Physics2D.Raycast(_front.position, Quaternion.Euler(0,0,30) *_front.right, 3.5f, LayerMask.GetMask("Car"));
            RaycastHit2D hitFront3 = Physics2D.Raycast(_front.position, Quaternion.Euler(0,0,-30) *_front.right, 3.5f, LayerMask.GetMask("Car"));
            RaycastHit2D hitFront4 = Physics2D.Raycast(_front.position, Quaternion.Euler(0,0,-45) *_front.right, 3.5f, LayerMask.GetMask("Car"));
            RaycastHit2D hitFront5 = Physics2D.Raycast(_front.position, Quaternion.Euler(0,0,45) *_front.right, 3.5f, LayerMask.GetMask("Car"));
            return ShouldStop(hitFront) || ShouldStop(hitFront2) || ShouldStop(hitFront3);
        }
    }
    

    private bool ShouldStop(RaycastHit2D hit)
    {
        bool stop = false;
        if (hit && hit.collider != null)
        {
            if (hit.collider.GetComponent<CarController>() != null) stop = true;
            // if (hit.collider.GetComponent<CarController>() != null) Debug.Log($"Distance: {hit.distance}");
            CarAI car = hit.transform.GetComponent<CarAI>();
            if (car != null && car != this && car.State != CarState.PARKED) stop = true;
        }

        return stop;
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
        else if (_carState == CarState.CIRCLING && _goingHome && other.gameObject.Equals(Game.Instance.OutPoint))
        {
            _carState = CarState.HOME;
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
//            Debug.Log($"Next node: {_targetNode.name} for {gameObject.name}");
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
        if (_carState == CarState.CIRCLING)
        {
            Gizmos.color = Color.red;
            if (_targetNode != null) Gizmos.DrawLine(transform.position, _targetNode.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_front.position, _front.position + _front.right*2);
            Gizmos.DrawLine(_front.position, _front.position + Quaternion.Euler(0,0,30) * _front.right*3.5f);
            Gizmos.DrawLine(_front.position, _front.position + Quaternion.Euler(0,0,45) * _front.right*3.5f);
            Gizmos.DrawLine(_front.position, _front.position + Quaternion.Euler(0,0,-30) * _front.right*3.5f);           
            Gizmos.DrawLine(_front.position, _front.position + Quaternion.Euler(0,0,-45) * _front.right*3.5f);           
            Gizmos.DrawLine(_front.position, _front.position + Quaternion.Euler(0,0,-45) * _front.right*3.5f);           
        }
    }
}