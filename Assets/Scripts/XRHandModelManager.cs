using UnityEngine;
using UnityEngine.InputSystem;

public class XRHandModelManager : MonoBehaviour
{
    public InputActionProperty gripAction;
    public InputActionProperty triggerAction;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();    
    }

    private void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        float gripValue = gripAction.action.ReadValue<float>();

        animator.SetFloat("Grip", gripValue);
        animator.SetFloat("Trigger", triggerValue);
    }
}
