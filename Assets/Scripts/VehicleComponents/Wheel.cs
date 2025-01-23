using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Wheel Settings")]
    public float turnSpeed = 5f;
    private float _moveSpeed;

    [Header("Turning Settings")]
    [Range(-15f, 15f)] public float turnAngle = 0f;

    [Header("References")]
    [SerializeField] private AttachmentPoint attachment;
    [SerializeField] private GameObject wheel;
    [SerializeField] private Collider col;
    private Rigidbody rb;
    private Rigidbody _vehicleRb;

    private Vector3 initialLocalRotation; 
    private float turning;

    private void Start()
    {
        initialLocalRotation = wheel.transform.localEulerAngles;
        rb = GetComponent<Rigidbody>();
    }
    
    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    public void ApplyForce(Rigidbody vehicleRb)
    {
        col.enabled = false;
        _vehicleRb = vehicleRb;
        turning += turnAngle * turnSpeed * Time.deltaTime;
        //wheel.transform.rotation = Quaternion.Euler(-initialLocalRotation.x, (initialLocalRotation.y + turning), initialLocalRotation.z);

        //pcha od naszej pozycji do przodu z siłą movespeed pamietaj
        Vector3 forwardForce = Quaternion.Euler(0, transform.rotation.y, 0) * _vehicleRb.transform.right * _moveSpeed;
        Vector3 forcePosition = transform.position;
        Debug.DrawLine(forcePosition, forcePosition + forwardForce, Color.red, 0.1f);
        
        rb.AddForceAtPosition(forwardForce, forcePosition);
    }
    
    private void OnDrawGizmos()
    {
        if (!_vehicleRb) return;
        Vector3 forwardForce = Quaternion.Euler(0, transform.rotation.y, 0) * _vehicleRb.transform.right * _moveSpeed;
        Vector3 forcePosition = transform.position;
        
        Gizmos.color = Color.green;
        
        Gizmos.DrawLine(forcePosition, forcePosition + forwardForce);
        Gizmos.DrawSphere(forcePosition, 0.05f); 
    }


}
