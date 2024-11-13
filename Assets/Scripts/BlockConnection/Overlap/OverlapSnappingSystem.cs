using System.Collections.Generic;
using UnityEngine;


public class OverlapSnappingSystem : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{

    [SerializeField] private bool isRoot = false; // Flag indicating if this object is a root object
    [SerializeField] private LayerMask layerMask; // Layer mask to filter colliders for snapping


    [SerializeField] private GameObject[] snappingTriggers; // Array of trigger objects for snapping
    public List<AttachmentPoint> listOfAttachedPoints;
    private Vector3[] directions = null; // Array of directions for snapping


    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        float multiply = 0.5f; // Multiplier for calculating snap directions
        directions = new Vector3[]
        {
            Vector3.down * multiply,
            Vector3.up * multiply,
            Vector3.left * multiply,
            Vector3.right * multiply,
            Vector3.back * multiply,
            Vector3.forward * multiply
        };
    }


    // Activate the snap mechanism
    public void ConnectBlocks()
    {
        if (!isRoot)
        {
            SetTriggersActive(false);
            short currentSide = 0;
            foreach (Vector3 direction in directions)
            {
                TryParent(currentSide, CheckCollidersInDirection(direction)); // Try to snap to a nearby collider
                currentSide++;
            }
            SetTriggersActive(true);
        }
    }



    // Check colliders in a direction for snapping
    private Collider CheckCollidersInDirection(Vector3 direction)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + direction, transform.localScale.x / 2, layerMask, QueryTriggerInteraction.Collide); // Check colliders in the specified direction
        Collider nearestTrigger = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position); // Calculate distance to collider
            if (distance < nearestDistance) // Check if the distance is smaller than the nearest distance
            {
                nearestTrigger = collider; // Update the nearest collider
                nearestDistance = distance; // Update the nearest distance
            }
        }
        return nearestTrigger;
    }

    // Set the active state of snapping triggers
    private void SetTriggersActive(bool isActive)
    {
        foreach (var trigger in snappingTriggers)
        {
            trigger.SetActive(isActive);
        }
    }

    // Try to parent the object to a nearby collider
    private void TryParent(int currentIndexTrigger, Collider outTrigger)
    {
        if (outTrigger != null && outTrigger.transform.parent.hierarchyCount > 6)
        {
            AttachmentPoint attachmentPoint1 = snappingTriggers[currentIndexTrigger].GetComponent<AttachmentPoint>();
            AttachmentPoint attachmentPoint2 = outTrigger.GetComponent<AttachmentPoint>();

            Debug.Log("attachmentPoint1: ", attachmentPoint1);
            Debug.Log("attachmentPoint2: ", attachmentPoint2);
            // ���������, ���� �� ��������� ����� ��������� � ����� ������
            if (attachmentPoint1 != null && attachmentPoint2 != null &&
            !attachmentPoint1.isConnected && !attachmentPoint2.isConnected)
            {
                transform.position = outTrigger.transform.position;
                transform.rotation = outTrigger.transform.rotation;

                transform.SetParent(outTrigger.GetComponentInParent<Transform>().parent); // Set the parent to the trigger's

                attachmentPoint1.isConnected = true;
                attachmentPoint2.isConnected = true;

                listOfAttachedPoints.Add(attachmentPoint1);
                listOfAttachedPoints.Add(attachmentPoint2);

                Debug.Log(attachmentPoint1 + " connected to: ", attachmentPoint2);

                TurnOnGravity(false);
            }
        }
    }


    // Unsnap the object from its parent
    public void DisconnectBlocks()
    {
        // Store current position and rotation
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        transform.SetParent(null, true);

        // Restore current position and rotation
        transform.position = currentPosition;
        transform.rotation = currentRotation;

        foreach (var attachmentPoint in listOfAttachedPoints)
        {
            attachmentPoint.isConnected = false;
            //listOfAttachedPoints.Remove(attachmentPoint);
        }

        TurnOnGravity(true); // Turn gravity back on
    }

    // Turn gravity on or off for the object
    private void TurnOnGravity(bool state)
    {
        rb.isKinematic = !state; // Set kinematic state to control gravity
    }
}