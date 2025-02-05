using System.Collections;
using UnityEngine;

public class AmbientMusicController : MonoBehaviour
{
    [Header("Ustawienia dŸwiêku ambientu")]
    [Tooltip("Komponent AudioSource z przypisanym klipem ambientowym.")]
    public AudioSource audioSource;
    
    [Tooltip("Czas trwania efektu fade in/out (w sekundach).")]
    public float fadeDuration = 2f;

    // Flaga informuj¹ca, czy muzyka aktualnie gra
    private bool isPlaying = false;
    // Odniesienie do aktualnie dzia³aj¹cej korutyny (jeœli jest)
    private Coroutine currentCoroutine = null;

    void Start()
    {
        // Jeœli nie przypisano AudioSource w Inspectorze, pobierz go z tego samego obiektu
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            // Ustaw pêtlê odtwarzania
            audioSource.loop = true;
            // Ustaw g³oœnoœæ na 0, aby rozpocz¹æ od fade in
            audioSource.volume = 0f;
            // Rozpocznij odtwarzanie
            audioSource.Play();
            isPlaying = true;
            // Rozpocznij efekt fade in
            currentCoroutine = StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogError("Brak komponentu AudioSource! Przypisz AudioSource w Inspectorze lub dodaj go do obiektu.");
        }
    }

    /// <summary>
    /// Metoda wy³¹czaj¹ca muzykê z efektem fade out.
    /// </summary>
    public void TurnOff()
    {
        if (audioSource != null && isPlaying)
        {
            // Jeœli jakaœ korutyna jest ju¿ uruchomiona, zatrzymaj j¹
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(FadeOut());
        }
    }

    /// <summary>
    /// Metoda w³¹czaj¹ca muzykê z efektem fade in.
    /// </summary>
    public void TurnOn()
    {
        if (audioSource != null && !isPlaying)
        {
            // Upewnij siê, ¿e pêtla odtwarzania jest aktywna i rozpocznij odtwarzanie
            audioSource.loop = true;
            audioSource.Play();

            // Jeœli jakaœ korutyna jest ju¿ uruchomiona, zatrzymaj j¹
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            currentCoroutine = StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// Korutyna odpowiedzialna za efekt fade in.
    /// </summary>
    private IEnumerator FadeIn()
    {
        float timer = 0f;
        float startVolume = audioSource.volume;
        float targetVolume = 0.25f; // docelowa g³oœnoœæ

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
        isPlaying = true;
        currentCoroutine = null;
    }

    /// <summary>
    /// Korutyna odpowiedzialna za efekt fade out.
    /// </summary>
    private IEnumerator FadeOut()
    {
        float timer = 0f;
        float startVolume = audioSource.volume;
        float targetVolume = 0f; // docelowa g³oœnoœæ

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
        // Po zakoñczeniu fade out zatrzymaj odtwarzanie
        audioSource.Stop();
        isPlaying = false;
        currentCoroutine = null;
    }
}
