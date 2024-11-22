using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public float moveSpeed = 2f;

    [Header("Turning Settings")]
    [Range(-45f, 45f)] public float turnAngle = 0f;
    [Header("References")]
    [SerializeField] private AttachmentPoint attachment;
    [SerializeField] private Collider col;

    private Vector3 initialLocalRotation; 

    private void Start()
    {
        initialLocalRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        if (!attachment.isConnected)
        {
            col.enabled = true;
            return;
        }

        col.enabled = false;
        transform.localRotation = Quaternion.Euler(initialLocalRotation.x, initialLocalRotation.y + turnAngle, initialLocalRotation.z);
    }

    public void Move()
    {
        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }
}
