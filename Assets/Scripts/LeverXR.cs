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
    private bool doMethod;
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

        if (angle >= forwardLimit)
        {
            if (doMethod) return;
            StartHolding(tvController.NavigateUp);
        }
        else if (angle <= backwardLimit)
        {
            if (doMethod) return;
            StartHolding(tvController.NavigateDown);
        }
        else
        {
            ResetHolding();
        }

        HoldCheck();
    }

    // ✅ Poprawiona metoda do restartowania rotacji dźwigni
    public void ResetLeverRotation()
    {
        transform.position = levelTrans.position;
        transform.rotation = levelTrans.rotation;
        ResetHolding();
    }

    private void StartHolding(System.Action action)
    {
        if (doMethod) return;
        holdTimer = 0f;
        doMethod = true;
        holdAction = action;
    }

    private void HoldCheck()
    {
        if (doMethod && holdAction != null)
        {
            doMethod = false;
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0)
            {
                holdTimer = holdTimeThreshold; // Resetujemy licznik, aby powtarzać akcję
                holdAction.Invoke();
                doMethod = true;
            }
        }
    }

    private void ResetHolding()
    {
        holdTimer = 0f;
        doMethod = false;
        holdAction = null;
    }
}