using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public float moveSpeed = 2f;
    public float turnSpeed = 5f;

    [Header("Turning Settings")]
    [Range(-45f, 45f)] public float turnAngle = 0f;

    [Header("References")]
    [SerializeField] private AttachmentPoint attachment;
    [SerializeField] private Collider col;
    private Rigidbody rb;

    private Vector3 initialLocalRotation; 

    private void Start()
    {
        initialLocalRotation = transform.localEulerAngles;
        rb = GetComponent<Rigidbody>();
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

    public void ApplyForce()
    {
        // if (attachment.isConnected)
        {
            Vector3 direction = Quaternion.Euler(0, turnAngle, 0) * transform.forward;
            //Vector3 force = transform.forward * moveSpeed;
            //rb.AddForceAtPosition(force, transform.position);
            rb.AddForceAtPosition(direction * moveSpeed, transform.position);
        }
    }
}
