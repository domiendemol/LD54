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
    [SerializeField] private Transform _spawn;
    [SerializeField] private List<ParkingSpace> _freeSpaces;
    
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
        // Spawn new parked cars
        foreach (ParkingSpace space in _parkingSpaces)
        {
            if (!_freeSpaces.Contains(space))
            {
                GameObject newCar = Instantiate(_carAIPrefab, space.Collider.transform.position, Quaternion.identity);
                newCar.transform.Rotate(0f, 0f,  space.Downwards ? -90f : 90f);
                newCar.GetComponent<CarAI>().ParkingSpace = space;
                space.Free = false;
            }
        }

        StartCoroutine(SpawnNewCars());
    }
    
    private IEnumerator SpawnNewCars()
    {
        for (int i=0; i<5; i++)
        {
            yield return new WaitForSeconds(6);
            FindObjectsOfType<CarAI>()[Random.Range(0, 30)].LeaveSpace();
            SpawnNewCar();
        }
    }

    public void SpawnNewCar()
    {
        GameObject newCirclingCar = Instantiate(_carAIPrefab, _spawn.position, Quaternion.identity);
        newCirclingCar.name = "CarAI-" + Random.Range(100, 999);
        newCirclingCar.GetComponent<CarAI>().State = CarAI.CarState.CIRCLING;
    }
    
    void UpdateState()
    {
        
    }
}
