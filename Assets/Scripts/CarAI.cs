using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    private Path _path;

    [SerializeField] private Transform _previousNode;
    [SerializeField] private Transform _targetNode;
    
    private void Awake()
    {
        _path = FindObjectOfType<Game>().MainPath;
        _previousNode = transform;
    }
    
    private void Update()
    {
        // for testing
        if (Input.GetKey(KeyCode.Space))
        {
            if (_targetNode == null)
            {
                // find closest node in front of car
                List<Transform> nodes = _path.Nodes.Where(n => IsInFront(n, 75f)).ToList();
                nodes = nodes.Where(n => (n.transform.position - transform.position).magnitude > 0.1).ToList();
                TransformSorter.SortByDistance(ref nodes, gameObject);
                if (nodes.Count == 0) return;
            
                _targetNode = nodes.First();
            }
            else if (Vector2.Distance(transform.position, _targetNode.position) < 0.1)
            {
                _previousNode = _targetNode;
                _targetNode = _path.GetNext(_previousNode);
                Debug.Log($"Next: {_targetNode.name}");
            }

            // detect car in front
            
            // move there
            transform.Translate(Vector3.Normalize(_targetNode.position - transform.position) * 0.1f, Space.Self);
            // rotate towards node
            transform.GetChild(0).rotation = Quaternion.Lerp(_previousNode.rotation, _targetNode.rotation, Vector2.SqrMagnitude(_targetNode.position - _previousNode.position) / Vector2.SqrMagnitude(_targetNode.position - transform.position));
        }
    }
    
    public bool IsInFront(Transform objectTransform, float coneOfVisionAngle)
    {
        // Get the direction vector from the transform to the object.
        Vector3 direction = objectTransform.position - transform.position;

        // Calculate the angle between the transform's forward vector and the direction vector.
        float angle = Vector2.Angle(transform.right, direction);
        angle %= 360f;
        angle = angle > 180 ? angle - 360 : angle < -180 ? angle + 360 : angle;
        // Return true if the angle is less than the cone of vision angle.
        return angle < coneOfVisionAngle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_targetNode != null) Gizmos.DrawLine(transform.position, _targetNode.position);
    }
}