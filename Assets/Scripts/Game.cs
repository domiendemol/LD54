using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    enum State
    {
        Init,
        Playing,
        End
    }

    [SerializeField] private GameObject _carAIPrefab;
    [SerializeField] private Path _mainPath;
    [SerializeField] private Transform _spawn;
    [SerializeField] private List<Transform> _fixedSpawns;
    [SerializeField] private List<ParkingSpace> _freeSpaces;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _endPanel;
    [SerializeField] private TextMeshProUGUI _endText;
    [SerializeField] private GameObject _outPoint;
    
    private State _state;
    private ParkingSpace[] _parkingSpaces;
    private CarController _carController;

    public Path MainPath => _mainPath;
    public GameObject OutPoint => _outPoint;
    public static Game Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _parkingSpaces = FindObjectsOfType<ParkingSpace>();
        _carController = FindObjectOfType<CarController>();
        StartState(State.Init);
        _startPanel.GetComponentInChildren<Button>().onClick.AddListener(StartButtonClicked);
        _endPanel.GetComponentInChildren<Button>().onClick.AddListener(EndButtonClicked);
    }

    void Update()
    {
        UpdateState();
    }

    void StartState(State newState)
    {
        Debug.Log($"== Starting state: {newState}");
        StopState(_state);
        if (newState == State.Init)
        {
            StartInitState();
        }
        else if (newState == State.Playing)
        {
            StartPlayingState();
        }
        else if (newState == State.End)
        {
            StartEndState();
        }

        _state = newState;
    }

    void StopState(State oldState)
    {
        if (oldState == State.Playing)
        {
            StopCoroutine(CarDepartCoroutine());
        }
        else if (oldState == State.End)
        {
            CarAI[] cars = FindObjectsOfType<CarAI>();
            foreach (CarAI car in cars)
            {
                Destroy(car.gameObject);
            }
            _carController.Reset();
            _endPanel.SetActive(false);
        }
    }

    void StartInitState()
    {
        // TODO for now - add intro later
        _startPanel.SetActive(true);
    }

    void StartPlayingState()
    {
        _startPanel.SetActive(false);
        
        // Spawn new parked cars
        foreach (ParkingSpace space in _parkingSpaces)
        {
            if (!_freeSpaces.Contains(space))
            {
                GameObject newCar = Instantiate(_carAIPrefab, space.Collider.transform.position, Quaternion.identity);
                newCar.transform.Rotate(0f, 0f,  space.Downwards ? -90f : 90f);
                newCar.name = "CarAI-" + Random.Range(100, 999);
                newCar.GetComponent<CarAI>().ParkingSpace = space;
                space.Free = false;
            }
        }
        
        // Spawn circling cars
        foreach (Transform fixedSpawn in _fixedSpawns)
        {
            SpawnNewCar(fixedSpawn);
        }

        StartCoroutine(CarDepartCoroutine());
    }

    void StartEndState()
    {
        _endText.text = "Congratulations! You managed to get a parking spot!\n\n" +
                        "Let's hope the doctor is running behind schedule too and this minor delay won't cost you another visit.";
        _endPanel.SetActive(true);
    }
    
    private IEnumerator CarDepartCoroutine()
    {
        for (int i=0; i<25; i++)
        {
            yield return new WaitForSeconds(6);
            FindObjectsOfType<CarAI>()[Random.Range(0, 30)].LeaveSpace();
            if (i%2==0) SpawnNewCar(_spawn);
        }
    }

    public void SpawnNewCar(Transform spawn)
    {
        GameObject newCirclingCar = Instantiate(_carAIPrefab, spawn.position, spawn.rotation);
        newCirclingCar.name = "CarAI-" + Random.Range(100, 999);
        newCirclingCar.GetComponent<CarAI>().State = CarAI.CarState.CIRCLING;
    }
    
    void UpdateState()
    {
        if (_state == State.Init)
        {
            
        }
        else if (_state == State.Playing)
        {
            if (_carController.Parked) StartState(State.End);
        }
        else if (_state == State.End)
        {
            
        }
    }

    void StartButtonClicked()
    {
        StartState(State.Playing);
    }
    void EndButtonClicked()
    {
        StartState(State.Init);
    }
    
}
