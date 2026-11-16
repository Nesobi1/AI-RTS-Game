using NUnit.Framework;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class UnitScript : MonoBehaviour
{
    public static List<UnitScript> all = new List<UnitScript>();
    private NavMeshAgent agent;

    [SerializeField] private GameObject selectionRing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MoveTo(Vector3 position)
    {
        Debug.Log("Should be moving");
        agent.velocity = Vector3.zero;
        agent.SetDestination(position);
        Debug.Log($"On NavMesh: {agent.isOnNavMesh}, destination: {position}");
    }

    public void SetSelected(bool selected)
    {
        selectionRing.SetActive(selected);
    }
















    private void OnEnable()
    {
        all.Add(this);
    }

    private void OnDisable()
    {
        all.Remove(this);
    }
}
