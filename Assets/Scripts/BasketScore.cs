using TMPro;
using UnityEngine;

public class BasketScore : MonoBehaviour
{
    public string ballTag = "Ball";
    [SerializeField] private TextMeshProUGUI scoreTxt;
    private int _score;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Vector3 velocityDirection = rb.linearVelocity.normalized;
        float dotDown = Vector3.Dot(velocityDirection, Vector3.down);
        if (dotDown > 0.7f && rb.linearVelocity.magnitude > 1f)
            if (!other.CompareTag(ballTag)) return;
        _score++;
        OnScore();
    }

    private void OnScore()
    {
        scoreTxt.text = _score.ToString();
    }
}