// MoveTo.cs
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMoveTo : MonoBehaviour {

    public Transform goal;

    void Start () {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position; 
    }
}