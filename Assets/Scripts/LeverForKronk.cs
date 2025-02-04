using UnityEngine;

public class LeverForKronk : MonoBehaviour
{
    public HingeJoint hingeJoint;
    public Animator animVehicle;
    [SerializeField] private string boolParameterName = "WrongLever";
    private float forwardLimit;   

    void Start()
    {
        forwardLimit = hingeJoint.limits.max;
    }
    
    void Update()
    {
        float angle = hingeJoint.angle;
        if (angle >= forwardLimit / 2)
        {
            animVehicle.SetBool(boolParameterName, true);
        }
    }
}
