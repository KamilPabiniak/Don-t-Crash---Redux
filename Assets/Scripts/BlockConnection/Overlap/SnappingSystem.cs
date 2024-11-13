using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnappingSystem : MonoBehaviour
{

    [SerializeField] private bool isRoot = false; // Flag indicating if this object is a root object
    [SerializeField] private float defaultOverlapRadius = 0.25f;
    [SerializeField] private float closerOverlapRadius = 0.05f;

    [SerializeField] private float snapRadiusMultiply = 1f;

    private bool isSnapped = false;
    [SerializeField] private LayerMask layerMask; // Layer mask to filter colliders for snapping


    [SerializeField] private GameObject[] snappingTriggers; // Array of trigger objects for snapping
    public List<AttachmentPoint> listOfAttachedPoints;
    private Vector3[] directions = null; // Array of directions for snapping


    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        snapRadiusMultiply = transform.localScale.x;
        float multiply = 0.5f * snapRadiusMultiply; // Multiplier for calculating snap directions
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
                if (!isSnapped)
                {
                    TryParent(currentSide, CheckCollidersInDirection(direction, defaultOverlapRadius)); // Try to snap to a nearby collider
                    currentSide++;
                }
                else break;
            }
            SetTriggersActive(true);
        }
    }

    // Check colliders in a direction for snapping
    private Collider CheckCollidersInDirection(Vector3 direction, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + direction, radius * snapRadiusMultiply, layerMask, QueryTriggerInteraction.Collide); // Check colliders in the specified direction
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

    private void UpdateNeighbourBlocks()
    {
        foreach (Vector3 direction in directions)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + direction, closerOverlapRadius * snapRadiusMultiply, layerMask, QueryTriggerInteraction.Collide);
            if (colliders != null)
            {
                Debug.Log("Count of colliders: " + colliders.Length, gameObject);
                foreach (Collider collider in colliders)
                {
                    collider.GetComponent<AttachmentPoint>().isConnected = false;
                    if (colliders.Length > 1)
                    {
                        collider.GetComponent<AttachmentPoint>().isConnected = true;
                    }
                }
            }
        }
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

            // Проверяем, есть ли свободные точки крепления у обоих блоков
            if (attachmentPoint1 != null && attachmentPoint2 != null &&
            !attachmentPoint1.isConnected && !attachmentPoint2.isConnected)
            {
                transform.position = outTrigger.transform.position;
                transform.rotation = outTrigger.transform.rotation;

                transform.SetParent(outTrigger.GetComponentInParent<Transform>().parent); // Set the parent to the trigger's

                var otherSnappingSystem = outTrigger.gameObject.GetComponentInParent<SnappingSystem>();

                attachmentPoint1.isConnected = true;
                attachmentPoint2.isConnected = true;
                
                AddPointToList(attachmentPoint1, attachmentPoint2, otherSnappingSystem);

                isSnapped = true;
                TurnOnGravity(false);
            }
        }
    }

    private void AddPointToList(AttachmentPoint attachmentPoint1, AttachmentPoint attachmentPoint2, SnappingSystem otherSnappingSystem)
    {
        listOfAttachedPoints.Add(attachmentPoint1);
        listOfAttachedPoints.Add(attachmentPoint2);

        // Pass AttachmentPoint information to another class
        // Assuming you have a reference to the other SnappingSystem class
        if (otherSnappingSystem != null)
        {
            otherSnappingSystem.ReceiveAttachmentPointInfo(attachmentPoint1, attachmentPoint2);
        }
    }

    public void ReceiveAttachmentPointInfo(AttachmentPoint attachmentPoint1, AttachmentPoint attachmentPoint2)
    {
        AddPointToList(attachmentPoint1, attachmentPoint2, null);
    }

    // Unsnap the object from its parent
    public void DisconnectBlocks()
    {
        // Store current position and rotation
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        transform.SetParent(null, true);
        transform.SetParent(null, true);

        // Restore current position and rotation
        transform.position = currentPosition;
        transform.rotation = currentRotation;
        ClearAttachedPoints();

        // Remove points from the other SnappingSystem class if needed
        // Assuming you have a reference to the other SnappingSystem class
        List<SnappingSystem> otherSnappingSystems = GetComponentsInChildren<SnappingSystem>().ToList();
        otherSnappingSystems.Add(GetComponentInParent<SnappingSystem>());

        foreach (SnappingSystem snappingSys  in otherSnappingSystems) 
        {
            snappingSys.ClearAttachedPoints();    
        }


        TurnOnGravity(true); // Turn gravity back on
        isSnapped = false;
    }

    private void ClearAttachedPoints()
    {
        if (listOfAttachedPoints.Count > 0)
        {
            // Create a temporary list to store objects that should not be removed
            List<AttachmentPoint> remainingPoints = new List<AttachmentPoint>();

            foreach (var item in listOfAttachedPoints)
            {
                if (item != null && item.GetType() == typeof(AttachmentPoint))
                {
                    item.isConnected = false;
                }
                else
                {
                    // If it's not an AttachmentPoint, add it to the temporary list
                    remainingPoints.Add(item);
                }
            }

            // Update the original list to only contain the remaining objects
            listOfAttachedPoints = remainingPoints;
        }
    }


    // Turn gravity on or off for the object
    private void TurnOnGravity(bool state)
    {
        rb.isKinematic = !state; // Set kinematic state to control gravity
    }
}