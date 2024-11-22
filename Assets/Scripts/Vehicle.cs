using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<Engine> engines = new List<Engine>();
    [SerializeField]  private List<Wheel> wheels = new List<Wheel>();
    [SerializeField] private List<Booster> boosters = new List<Booster>();
    [SerializeField] private List<Battery> batteries = new List<Battery>();

    [Header("Engine State")]
    public bool TurnOnEngine = false;

    [Header("Turning Settings")]
    public float turnSpeed = 45f;
    [Range(-45f, 45f)] public float currentTurnAngle = 0f; 



    private void Update()
    {
        HandleTurning();
        if (TurnOnEngine)
        {
            MoveWheels();
        }
    }

    private void HandleTurning()
    {
        //transform.rotation = Quaternion.Euler(0f, currentTurnAngle * turnSpeed * Time.deltaTime, 0f);

        foreach (Wheel wheel in wheels)
        {
            wheel.turnAngle = currentTurnAngle;
        }
    }


    public void FindComponents()
    {
        engines.Clear();
        wheels.Clear();
        boosters.Clear();
        batteries.Clear();

    
        SearchComponentsRecursively(transform);

        Debug.Log($"Components Found - Engines: {engines.Count}, Wheels: {wheels.Count}, Boosters: {boosters.Count}, Batteries: {batteries.Count}");
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

    private void MoveWheels()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.Move();
        }
    }
}
