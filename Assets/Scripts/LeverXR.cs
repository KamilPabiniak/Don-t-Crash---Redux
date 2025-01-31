using UnityEngine;

public class LeverXR : MonoBehaviour
{
    [SerializeField] private HingeJoint hingeJoint; // Przypisz Hinge Joint dźwigni w Inspectorze
    [SerializeField] private TVController tvController; // Przypisz TVController
    private Transform levelTrans;

    private float forwardLimit;  // Maksymalne wychylenie do przodu
    private float backwardLimit; // Maksymalne wychylenie do tyłu

    [SerializeField] private float holdTimeThreshold = 1.5f; // Czas trzymania przed ponownym wywołaniem
    private float holdTimer;
    private bool reapetAgain;
    private System.Action holdAction;

    private void Start()
    {
        if (hingeJoint == null)
            hingeJoint = GetComponent<HingeJoint>();

        forwardLimit = hingeJoint.limits.max;
        backwardLimit = hingeJoint.limits.min;
        levelTrans = transform;
    }

    private void Update()
    {
        float angle = hingeJoint.angle;

        // Jeśli kąt dźwigni osiągnie limit do przodu
        if (angle >= forwardLimit)
        {
            StartHolding(tvController.NavigateUp);
        }
        // Jeśli kąt dźwigni osiągnie limit do tyłu
        else if (angle <= backwardLimit)
        {
            StartHolding(tvController.NavigateDown);
        }
        else
        {
            ResetHolding(); // Resetowanie, jeśli kąt jest poza limitami
        }

        HoldCheck(); // Sprawdzanie, czy należy wykonać akcję
    }

    // ✅ Metoda do restartowania rotacji dźwigni
    public void ResetLeverRotation()
    {
        transform.position = levelTrans.position;
        transform.rotation = levelTrans.rotation;
        ResetHolding();
    }

    private void StartHolding(System.Action action)
    {
        if (reapetAgain) return;

        holdAction = action; // Przypisz akcję do wykonania
        holdTimer = holdTimeThreshold; // Inicjalizuj timer
        reapetAgain = true; // Ustaw flagę, że zaczynamy trzymanie
    }

    private void HoldCheck()
    {
        if (reapetAgain && holdAction != null)
        {
            holdTimer -= Time.deltaTime; // Zmniejszaj timer w czasie rzeczywistym

            if (holdTimer <= 0)
            {
                holdAction.Invoke(); // Wywołaj przypisaną akcję
                Debug.Log("Robię akcję");

                // Resetuj timer, aby powtórzyć akcję po upływie holdTimeThreshold
                holdTimer = holdTimeThreshold;
                reapetAgain = false;
            }
        }
    }

    private void ResetHolding()
    {
        reapetAgain = false; // Zatrzymaj trzymanie akcji
        holdTimer = 0f; // Resetuj timer
        holdAction = null; // Zresetuj akcję
    }
}
