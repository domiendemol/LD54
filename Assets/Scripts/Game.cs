using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    enum State
    {
        Init,
        Playing
    }

    [SerializeField] private GameObject _carAIPrefab;
    [SerializeField] private Path _mainPath;
    
    private State _state;
    private ParkingSpace[] _parkingSpaces;

    public Path MainPath => _mainPath;

    void Start()
    {
        _parkingSpaces = FindObjectsOfType<ParkingSpace>();
        StartState(State.Init);
    }

    void Update()
    {
        
    }

    void StartState(State newState)
    {
        Debug.Log($"== Starting state: {newState}");
        if (newState == State.Init)
        {
            StartInitState();
        }
        else if (newState == State.Playing)
        {
            StartPlayingState();
        }

        _state = newState;
    }

    void StartInitState()
    {
        // TODO for now - add intro later
        StartState(State.Playing);
    }

    void StartPlayingState()
    {
        // Spawn new AI cars
        foreach (ParkingSpace space in _parkingSpaces)
        {
            if (Random.Range(0f, 1f) < 0.9f)
            {
                GameObject newCar = Instantiate(_carAIPrefab, space.Collider.transform.position, Quaternion.identity);
                newCar.transform.Rotate(0f, 0f,  space.Downwards ? -90f : 90f);
            }
        }
    }
    
    void UpdateState()
    {
        
    }
}
