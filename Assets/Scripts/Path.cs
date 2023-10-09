using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Transform> _nodes;
    [SerializeField] private Color _debugColor;
    public List<Transform> Nodes => _nodes;

    private void Awake()
    {
        _nodes = new List<Transform>();
        foreach (Transform t in transform)
            _nodes.Add(t);
    }

    public Transform GetNext(Transform node, bool loop, bool reverse)
    {
        int index = _nodes.IndexOf(node);
        if (reverse) {
            if (index == 0) return loop ? _nodes[^1] : null;
        }
        else {
            if (index == _nodes.Count - 1) return loop ? _nodes[0] : null;
        }
        
        return reverse ? _nodes[_nodes.IndexOf(node) - 1] : _nodes[_nodes.IndexOf(node) + 1];
    }

    public void Reverse()
    {
        transform.localScale = new Vector3(1f, -1f, 1f); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _debugColor;
        Transform prev = null;
        foreach (Transform node in _nodes)
        {
            if (prev != null)
            {
                Gizmos.DrawLine(prev.position, node.position);
            }

            prev = node;
        }
    }
}

// TODO MOVE
public class TransformSorter
{
    public static void SortByDistance(ref List<Transform> transforms, GameObject gameObject)
    {
        // Create a delegate that compares two transforms based on their distance to the GameObject.
        Comparison<Transform> comparison = (a, b) => Vector3.Distance(a.position, gameObject.transform.position).CompareTo(Vector3.Distance(b.position, gameObject.transform.position));

        // Use the Sort() method on the list, passing in the delegate as a parameter.
        transforms.Sort(comparison);
    }
}