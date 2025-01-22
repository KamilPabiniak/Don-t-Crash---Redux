using UnityEngine;

public class EngineDataTransfer : MonoBehaviour
{
    [Header("Connected Components")]
    [SerializeField] private Vehicle vehicle;

    /// <summary>
    /// Updates connections by transferring data between components.
    /// </summary>
    public void UpdateComponentConnections()
    {
        if (vehicle.engines.Count == 0 || vehicle.wheels.Count == 0)
        {
            Debug.LogWarning("Vehicle has no engines or wheels connected.");
            return;
        }

        // Assuming we distribute power equally among wheels
        float totalPower = CalculateTotalEnginePower();
        float powerPerWheel = totalPower / vehicle.wheels.Count;

        foreach (Wheel wheel in vehicle.wheels)
        {
            wheel.SetMoveSpeed(powerPerWheel);
        }

        Debug.Log($"Total Power: {totalPower}, Power per Wheel: {powerPerWheel}");
    }

    /// <summary>
    /// Calculates the total power of all engines.
    /// </summary>
    /// <returns>Total engine power.</returns>
    private float CalculateTotalEnginePower()
    {
        float totalPower = 0f;
        foreach (Engine engine in vehicle.engines)
        {
            totalPower += engine.power;
        }
        return totalPower;
    }
}
