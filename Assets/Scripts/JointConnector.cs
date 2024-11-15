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

        SnapSystem[] snapSystems = FindObjectsOfType<SnapSystem>();

        foreach (SnapSystem snapSystem in snapSystems)
        {
            if (snapSystem.transform.IsChildOf(rootRigidbody.transform) || snapSystem.isRoot)
            {
                Rigidbody rb = snapSystem.GetComponent<Rigidbody>();
                if (rb != null && rb != rootRigidbody)
                {
                    FixedJoint joint = rootRigidbody.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = rb;
                    connectedBodies.Add(rb);

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

        isConnected = true;
    }
}