using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Vehicle : MonoBehaviour
{
    [Header("Components")]
    public List<Engine> engines = new List<Engine>();
    public List<Wheel> wheels = new List<Wheel>();
    public List<Booster> boosters = new List<Booster>();
    public List<Battery> batteries = new List<Battery>();
    
    [Header("Engine")] 
    [SerializeField] private EngineDataTransfer dataTransfer;
    public bool turnOnEngine;

    [Header("Turning Settings")]
    [Range(-45f, 45f)] public float currentTurnAngle;

    private Rigidbody rb;
    

    private void Update()
    {
        HandleTurning();
        if (turnOnEngine)
        {
            MoveWheels();
        }
    }

    public void FindComponents()
    {
        engines.Clear();
        wheels.Clear();
        boosters.Clear();
        batteries.Clear();

    
        SearchComponentsRecursively(transform);
        rb = engines[0].GetComponent<Rigidbody>();
        dataTransfer.UpdateComponentConnections();
    }

    private void SearchComponentsRecursively(Transform parent)
    {

        if (parent.TryGetComponent(out Engine engine))
        {
            engines.Add(engine);
        }

        if (parent.TryGetComponent(out Wheel wheel))
        {
            wheels.Add(wheel);
        }

        if (parent.TryGetComponent(out Booster booster))
        {
            boosters.Add(booster);
        }

        if (parent.TryGetComponent(out Battery battery))
        {
            batteries.Add(battery);
        }

        foreach (Transform child in parent)
        {
            SearchComponentsRecursively(child);
        }
    }

    private void HandleTurning()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.turnAngle = currentTurnAngle;
        }
    }
    
    private void MoveWheels()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.ApplyForce(rb);
        }
    }
}
