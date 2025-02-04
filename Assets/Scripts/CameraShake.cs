using UnityEngine;
using System.Collections;

public class CameraShakeVR : MonoBehaviour
{
    [Header("Parametry trzêsienia")]
    [Tooltip("Czas trwania trzêsienia obiektu")]
    public float shakeDuration = 0.5f;
    [Tooltip("Maksymalne wychylenie obiektu (w jednostkach Unity)")]
    public float shakeAmount = 0.2f;

    // Oryginalna pozycja obiektu (ShakeContainer)
    private Vector3 initialLocalPosition;

    private void Awake()
    {
        // Zapamiêtujemy pocz¹tkow¹ lokaln¹ pozycjê ShakeContainer
        initialLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Metoda wywo³ywana eventem (np. po wybuchu) aby rozpocz¹æ efekt trzêsienia
    /// </summary>
    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Losowa wartoœæ w sferze jednostkowej mno¿ona przez shakeAmount
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            transform.localPosition = initialLocalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Przywracamy oryginaln¹ pozycjê
        transform.localPosition = initialLocalPosition;
    }
}