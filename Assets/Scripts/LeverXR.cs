using UnityEngine;
using System.Collections;

public class LeverXR : MonoBehaviour
{
    public HingeJoint hingeJoint;  
    [SerializeField] private TVController tvController; 

    private float forwardLimit;   
    private float backwardLimit;  

    [SerializeField] private float cooldownTime = 2f; 
    
    private bool isOnCooldown = false; 
    private bool leverInCooldownZone = false;

    public float GetForwadLimit() => forwardLimit;
    public float GetBackwardLimit() => backwardLimit;
    private void Start()
    {
        if (hingeJoint == null)
            hingeJoint = GetComponent<HingeJoint>();

        forwardLimit = hingeJoint.limits.max;
        backwardLimit = hingeJoint.limits.min;
    }

    private void Update()
    {
        float angle = hingeJoint.angle;

        if (angle >= forwardLimit)
        {
            TryStartCooldown(tvController.NavigateDown);
        }
        else if (angle <= backwardLimit)
        {
            TryStartCooldown(tvController.NavigateUp);
        }
        else
        {
            leverInCooldownZone = false;
        }
    }

    /// <summary>
    /// Sprawdza, czy można uruchomić cooldown i metodę.
    /// </summary>
    private void TryStartCooldown(System.Action action)
    {
        // Jeśli już jesteśmy na cooldownie albo dźwignia nie opuściła strefy granicznej od ostatniego użycia, nie rób nic.
        if (isOnCooldown || leverInCooldownZone)
            return;

        // Wywołujemy akcję i rozpoczynamy cooldown
        action.Invoke();
        leverInCooldownZone = true; // Blokujemy ponowne wywołanie bez wychodzenia
        StartCoroutine(CooldownCoroutine());
    }

    /// <summary>
    /// Obsługuje cooldown, blokując kolejne wywołania na określony czas.
    /// </summary>
    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true; // Aktywujemy cooldown
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false; // Po czasie odblokowujemy wywołania
    }
}
