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

    private State _state;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartState(State newState)
    {
        if (newState == State.Init)
        {
        }
        else if (newState == State.Playing)
        {
            
        }

        _state = newState;
    }

    void UpdateState()
    {
        
    }
    
    
}
