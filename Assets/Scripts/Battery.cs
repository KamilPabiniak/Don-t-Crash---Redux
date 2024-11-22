using UnityEngine;

public class Battery : MonoBehaviour
{
    [Header("Battery Properties")]
    public float chargeLevel = 100f; // Placeholder property

    public void DrainBattery(float amount)
    {
        chargeLevel -= amount;
        chargeLevel = Mathf.Max(0, chargeLevel);
        Debug.Log($"Battery drained. Remaining charge: {chargeLevel}%");
    }
}
