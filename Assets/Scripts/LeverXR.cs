using UnityEngine;
using System.Collections;

public class LeverXR : MonoBehaviour
{
    [SerializeField] private HingeJoint hingeJoint;   // Przypisz Hinge Joint dźwigni w Inspectorze
    [SerializeField] private TVController tvController; // Przypisz TVController

    private float forwardLimit;   // Maksymalne wychylenie do przodu
    private float backwardLimit;  // Maksymalne wychylenie do tyłu

    [SerializeField] private float cooldownTime = 2f; // Czas cooldownu (sekundy)

    private bool isOnCooldown = false; // Flaga cooldownu
    private bool leverInCooldownZone = false; // Czy dźwignia nadal jest w pozycji granicznej?

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
            TryStartCooldown(tvController.NavigateUp);
        }
        else if (angle <= backwardLimit)
        {
            TryStartCooldown(tvController.NavigateDown);
        }
        else
        {
            // Jeśli dźwignia wraca do neutralnej pozycji, resetujemy flagę
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
