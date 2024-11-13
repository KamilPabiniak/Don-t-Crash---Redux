using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Rigidbody))]
public class RailRoad : MonoBehaviour
{
    [SerializeField] private SplineContainer rail;
    [SerializeField] private float maxDistance = 0.2f;
    [SerializeField] private Vector3 cubeOffset = Vector3.down;
    [SerializeField] private Vector3 cubeSize = Vector3.one;
    [SerializeField] private float coroutineTimer = 3f;
    [SerializeField] private LayerMask layerMask;

    public float currentSpeed;

    private bool onTors = true;
    private bool isUnrailed = false;
    private Rigidbody rb;
    private Spline currentSpline;
    private Vector3 previousPosition;
    private float previousTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpline = rail.Splines[0];
        previousPosition = transform.position;
        previousTime = Time.time;
    }

    private void FixedUpdate()
    {
        MeasureSpeed();
        CheckRailCollision();
        CheckTorsCollision();
    }

    private void MeasureSpeed()
    {
        float currentTime = Time.time;
        float deltaTime = currentTime - previousTime;
        float distance = Vector3.Distance(transform.position, previousPosition);
        currentSpeed = distance / deltaTime;
        previousPosition = transform.position;
        previousTime = currentTime;
    }

    private void CheckRailCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            RunOnRail();
        }
        else
        {
            if (!isUnrailed && onTors)
            {
                MoveTrain();
            }
        }
    }

    private void CheckTorsCollision()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + cubeOffset, cubeSize, Quaternion.identity, layerMask);
        onTors = colliders.Length > 0;
    }

    private void MoveTrain()
    {
        NativeSpline native = new NativeSpline(currentSpline);
        float distance = SplineUtility.GetNearestPoint(native, transform.position, out float3 nearest, out float t);

        transform.position = nearest;

        Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
        Vector3 up = native.EvaluateUpVector(t);

        Vector3 remappedForward = Vector3.forward;
        Vector3 remappedUp = Vector3.up;
        Quaternion axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

        transform.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

        Vector3 engineForward = transform.forward;

        if (Vector3.Dot(rb.linearVelocity, transform.forward) < 0)
        {
            engineForward *= -1;
        }

        rb.linearVelocity = rb.linearVelocity.magnitude * engineForward;
    }

    private void RunOnRail()
    {
        StartCoroutine(RunOnRailCoroutine());
        isUnrailed = true;
    }

    private IEnumerator RunOnRailCoroutine()
    {
        yield return new WaitForSeconds(coroutineTimer);
        isUnrailed = false;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.localPosition, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + cubeOffset, cubeSize);
    }
}