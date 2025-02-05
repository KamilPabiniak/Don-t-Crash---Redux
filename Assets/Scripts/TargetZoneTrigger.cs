using UnityEngine;

public class TargetZoneTrigger : MonoBehaviour
{
    public string targetTag = "Beer";
    public GameObject wand;
    public GameObject particles;
    private int objectsInZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            objectsInZone++;
            Debug.Log("Obiekt umieszczony w strefie!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            objectsInZone--;
            Debug.Log("Obiekt zabrany ze strefy.");
        }
    }
    
    void Update()
    {
        if (!IsComplete()) return;
        wand.SetActive(true);
        particles.SetActive(true);
        gameObject.SetActive(false);
    }


    private bool IsComplete()
    {
        return objectsInZone >= 3; 
    }
}