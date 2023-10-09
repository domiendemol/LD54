using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ParkingSpace : MonoBehaviour
{
    [SerializeField] private bool _downwards = false;
    [SerializeField] private Path _inPath;
    [SerializeField] private Path _outPath;
    [SerializeField] private Vector3 _pathScale = Vector3.one;
    [SerializeField] private Collider2D _spotCollider;

    private BoxCollider2D _collider;
    private bool _free = true;
    public BoxCollider2D Collider => _collider;
    public bool Downwards => _downwards;

    public bool Free
    {
        get => _free;
        set => _free = value;
    }

    public Path InPath => _inPath;
    public Path OutPath => _outPath;

    private void Awake()
    {
        _collider = GetComponentInChildren<BoxCollider2D>();
        _inPath.transform.localScale = _pathScale;
        _outPath.transform.localScale = _pathScale;
        //_outPath.transform.localScale = new Vector3(-_pathScale.x, _pathScale.y, _pathScale.z);
        _free = true;
    }
    
    public void HasParkedPlayer()
    {
        // TODO 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Free ? Color.green : Color.red;
        Gizmos.DrawSphere(_spotCollider.transform.position, 0.4f);
    }
}