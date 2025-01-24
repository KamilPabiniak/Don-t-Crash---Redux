using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnapSystem : MonoBehaviour
{
    [SerializeField] private bool isRoot = false;
    [SerializeField] private LayerMask snapLayerMask;
    [SerializeField] private float snapRange = 0.2f;

    [Header("Attachment Points")]
    [SerializeField] private List<AttachmentPoint> snapPoints;

    private Rigidbody rb;
    private XRGrabInteractable xrGrab;

    private AttachmentPoint closestPoint; // Najbliższy punkt docelowy

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        xrGrab = GetComponent<XRGrabInteractable>();

        foreach (var point in snapPoints)
        {
            point.Initialize(this);
        }

        if (isRoot)
        {
            SetAsRoot();
        }
        else if (xrGrab != null)
        {
            xrGrab.selectExited.AddListener(OnRelease);
            xrGrab.selectEntered.AddListener(OnGrab);
        }
    }

    private void OnDestroy()
    {
        if (xrGrab != null)
        {
            xrGrab.selectExited.RemoveListener(OnRelease);
            xrGrab.selectEntered.RemoveListener(OnGrab);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        closestPoint = null; // Resetuj punkt, gdy zaczynasz trzymać obiekt
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        TrySnap();
        closestPoint = null; // Reset po puszczeniu obiektu
    }

    private void Update()
    {
        if (xrGrab != null && xrGrab.isSelected)
        {
            var (myPoint, targetPoint) = FindClosestSnapPoint();
            if (myPoint != null && targetPoint != null)
            {
                Debug.DrawLine(myPoint.transform.position, targetPoint.transform.position, Color.green);
            }
        }
    }


    private void TrySnap()
    {
        var (myPoint, targetPoint) = FindClosestSnapPoint();

        if (myPoint != null && targetPoint != null)
        {
            SnapToPoint(myPoint, targetPoint);
        }
        else
        {
            Detach();
        }
    }


    private (AttachmentPoint myPoint, AttachmentPoint targetPoint) FindClosestSnapPoint()
    {
        AttachmentPoint closestMyPoint = null;
        AttachmentPoint closestTargetPoint = null;
        float closestDistance = float.MaxValue;

        foreach (var myPoint in snapPoints)
        {
            var nearbyPoints = GetNearbyPoints(myPoint);

            foreach (var targetPoint in nearbyPoints)
            {
                float distance = Vector3.Distance(myPoint.transform.position, targetPoint.transform.position);
                if (distance < closestDistance)
                {
                    closestMyPoint = myPoint;
                    closestTargetPoint = targetPoint;
                    closestDistance = distance;
                }
            }
        }

        return (closestMyPoint, closestTargetPoint);
    }


    private List<AttachmentPoint> GetNearbyPoints(AttachmentPoint myPoint)
    {
        List<AttachmentPoint> points = new List<AttachmentPoint>();
        Collider[] nearbyColliders = Physics.OverlapSphere(myPoint.transform.position, snapRange, snapLayerMask);

        foreach (var collider in nearbyColliders)
        {
            var point = collider.GetComponent<AttachmentPoint>();
            if (point != null && point != myPoint && !point.isConnected && CanSnap(myPoint, point))
            {
                points.Add(point);
            }
        }

        return points;
    }

    private bool CanSnap(AttachmentPoint myPoint, AttachmentPoint targetPoint)
    {
        return myPoint.GetRootParent() != targetPoint.GetRootParent();
    }

    private AttachmentPoint GetClosestAttachmentPoint()
    {
        return snapPoints[0];
    }

    private void SnapToPoint(AttachmentPoint myPoint, AttachmentPoint targetPoint)
    {
        transform.position = targetPoint.transform.position;
        Quaternion targetRotation = targetPoint.transform.rotation; 
        Quaternion myPointRotation = myPoint.transform.rotation;   
        Quaternion rotationOffset = Quaternion.Inverse(myPointRotation) * transform.rotation;

     
        transform.rotation = targetRotation * rotationOffset;
        
        transform.rotation = SnapToNearest90Degrees(transform.rotation);
        transform.SetParent(targetPoint.transform);

        rb.isKinematic = true;
        
        myPoint.SetConnected(true);
        myPoint.ConnectedTo = targetPoint;
        targetPoint.SetConnected(true);
        targetPoint.ConnectedTo = myPoint;

        Debug.Log($"{myPoint.name} snapped to {targetPoint.name}");
    }
    
    private Quaternion SnapToNearest90Degrees(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        
        euler.x = Mathf.Round(euler.x / 90f) * 90f;
        euler.y = Mathf.Round(euler.y / 90f) * 90f;
        euler.z = Mathf.Round(euler.z / 90f) * 90f;

        return Quaternion.Euler(euler);
    }


    public void Detach()
    {
        transform.SetParent(null);
        rb.isKinematic = false;

        foreach (var point in snapPoints)
        {
            if (point.ConnectedTo != null) 
            {
                point.ConnectedTo.SetConnected(false);
                point.ConnectedTo = null; 
            }

            point.SetConnected(false); 
        }

        Debug.Log("Detached from any snap point.");
    }


    private void SetAsRoot()
    {
        rb.isKinematic = true;

        if (xrGrab != null)
        {
            xrGrab.enabled = false;
        }
    }
    
    public bool GetRoot()
    {
        return isRoot; 
    }
}
