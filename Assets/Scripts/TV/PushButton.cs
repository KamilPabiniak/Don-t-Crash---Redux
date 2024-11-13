using UnityEngine;


public class PushButton : MonoBehaviour
{
    [SerializeField] private TVController tVController;

    [SerializeField] private int buttonType = 0; // 0 - left,
                                                 // 1 - right,
                                                 // 2 - enter
    void Start()
    {
        switch (buttonType)
        {
            case 0:
                GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>().selectEntered.AddListener(x => tVController.LeftArrow());
                break;
            case 1:
                GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>().selectEntered.AddListener(x => tVController.RightArrow());
                break;
            case 2:
                GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>().selectEntered.AddListener(x => tVController.Enter());
                break;
        }
    }
}
