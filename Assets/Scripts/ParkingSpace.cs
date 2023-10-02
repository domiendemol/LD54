using System;
using UnityEngine;

public class ParkingSpace : MonoBehaviour
{
    [SerializeField] private bool _downwards = false;
    
    private BoxCollider2D _collider;
    public BoxCollider2D Collider => _collider;
    public bool Downwards => _downwards;

    private void Awake()
    {
        _collider = GetComponentInChildren<BoxCollider2D>();
    }

    public void HasParkedPlayer()
    {
        // TODO 
    }
}