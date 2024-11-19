using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JointConnector : MonoBehaviour
{
    [SerializeField] private Rigidbody rootRigidbody;

    private InputAction connectAction;
    private bool isConnected = false;
    private List<Rigidbody> connectedBodies = new List<Rigidbody>();

    private void OnEnable()
    {
        connectAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/1");
        connectAction.performed += OnConnectActionPerformed;
        connectAction.Enable();
    }

    private void OnDisable()
    {
        connectAction.Disable();
        connectAction.performed -= OnConnectActionPerformed;
    }

    private void OnConnectActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Connect action triggered"); 
        ConnectAllSnappedObjects();
    }

    private void ConnectAllSnappedObjects()
    {
        if (isConnected) return;

        List<SnapSystem> snapSystems = new List<SnapSystem>();
        FindAllSnapSystems(transform, snapSystems); 
        
        foreach (SnapSystem snapSystem in snapSystems)
        {
            if (snapSystem.isRoot || snapSystem.GetComponent<Rigidbody>() == null) continue;

            Rigidbody childRb = snapSystem.GetComponent<Rigidbody>();
            Transform parentTransform = snapSystem.transform.parent;

            if (parentTransform != null)
            {
                Rigidbody parentRb = parentTransform.GetComponent<Rigidbody>();

                if (parentRb != null && parentRb != childRb)
                {
                    FixedJoint joint = parentRb.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = childRb;
                    connectedBodies.Add(childRb);
                    snapSystem.enabled = false;
                }
            }
        }
        
        rootRigidbody.useGravity = true;
        rootRigidbody.isKinematic = false;

        foreach (Rigidbody rb in connectedBodies)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        
        foreach (SnapSystem snapSystem in snapSystems)
        {
            snapSystem.Detach();
            Debug.Log($"Detaching {snapSystem.name}");
        }

        isConnected = true;
    }


    private void FindAllSnapSystems(Transform current, List<SnapSystem> snapSystems)
    {
        SnapSystem snapSystem = current.GetComponent<SnapSystem>();
        if (snapSystem != null)
        {
            snapSystems.Add(snapSystem);
            Debug.Log(snapSystem);
        }
        
        foreach (Transform child in current)
        {
            FindAllSnapSystems(child, snapSystems);
        }
    }

}