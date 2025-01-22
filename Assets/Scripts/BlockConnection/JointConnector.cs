using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JointConnector : MonoBehaviour
{
    [SerializeField] private Rigidbody rootRigidbody;
    [SerializeField] private Vehicle vehicle;

    private InputAction connectAction;
    private bool isConnected = false;
    private List<SnapSystem> snapSystems = new List<SnapSystem>();

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
        ConnectAllSnappedObjects();
    }

    private void ConnectAllSnappedObjects()
    {
        if (isConnected) return;

        List<Rigidbody> connectedBodies = new List<Rigidbody>();
        FindAllSnapSystems(transform, snapSystems); 

        foreach (SnapSystem snapSystem in snapSystems)
        {
            if (snapSystem.isRoot || snapSystem.GetComponent<Rigidbody>() == null) continue;

            Rigidbody childRb = snapSystem.GetComponent<Rigidbody>();
            Rigidbody parentRb = GetParentRigidbody(snapSystem.transform);

            if (parentRb != null && parentRb != childRb)
            {
                FixedJoint joint = parentRb.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = childRb;
                connectedBodies.Add(childRb);

                snapSystem.enabled = false;
            }
            else
            {
                Debug.LogWarning($"Parent Rigidbody not found for {snapSystem.name}");
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
            snapSystem.Detach(false);
            snapSystem.transform.SetParent(gameObject.transform);
        }

        isConnected = true;
        vehicle.FindComponents();
    }

    private Rigidbody GetParentRigidbody(Transform child)
    {
        Transform current = child.parent;

        while (current != null)
        {
            Rigidbody rb = current.GetComponent<Rigidbody>();
            if (rb != null)
            {
                return rb; 
            }
            current = current.parent; 
        }
        return null; 
    }


    private void FindAllSnapSystems(Transform current, List<SnapSystem> snapSystems)
    {
        SnapSystem snapSystem = current.GetComponent<SnapSystem>();
        if (snapSystem != null)
        {
            snapSystems.Add(snapSystem);
        }
        
        foreach (Transform child in current)
        {
            FindAllSnapSystems(child, snapSystems);
        }
    }

}