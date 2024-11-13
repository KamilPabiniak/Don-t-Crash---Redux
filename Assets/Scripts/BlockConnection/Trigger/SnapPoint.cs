using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public Transform linkedObj = null;

    public bool isLinked = false;
    public bool isLinkedToParent = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out SnapPoint point))
        {
            isLinked = true;

            other.enabled = false;
            TryParent(other);

            Debug.Log("Linked successfully: " + point.gameObject.name);
        }

    }

    private void TryParent(Collider trigger)
    {
        if (trigger != null && trigger.transform.parent.hierarchyCount > 6) // Check if a valid collider was found and the hierarchy count of its parent is sufficient
        {
            transform.parent.position = trigger.transform.position;
            transform.parent.rotation = trigger.transform.rotation;
            transform.parent.SetParent(trigger.GetComponentInParent<Transform>().parent); // Set the parent to the trigger's parent

            trigger.GetComponentInParent<Rigidbody>().isKinematic = false;
            linkedObj = trigger.GetComponentInParent<Transform>().parent;
        }
    }
}
