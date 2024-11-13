using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class TriggerSnappingSystem : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [SerializeField] private int pointCount;
    public SnapPoint[] snapPoints;

    public void Start()
    {
        snapPoints = new SnapPoint[pointCount];
        for (int i = 0; i < pointCount; i++) 
        {
            snapPoints[i] = transform.GetChild(i).GetComponent<SnapPoint>();
        }
    }
    public void UpdateState()
    {
        foreach (SnapPoint trigger in snapPoints)
        {

        }
    }

    public void UnSnap()
    {
        // Store current position and rotation
        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        transform.SetParent(null, true);

        // Restore current position and rotation
        transform.position = currentPosition;
        transform.rotation = currentRotation;

        SetTriggersActive(true); // Reactivate snapping triggers
    }
    private void SetTriggersActive(bool isActive)
    {
        foreach (var trigger in snapPoints)
        {
            trigger.GetComponent<Collider>().enabled = true;
        }
    }


    public void GetSides()
    {
        List<GameObject> sides = new List<GameObject>();
    }

    private void GetSide(SnapPoint snapPoint)
    {
        if (!snapPoint && snapPoint.isLinked)
        {
            
        }
    }
}
