using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(AudioSource))]
public class HeldTriggerSound : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;
    
    [SerializeField]
    private float soundCooldown = 0.2f;
    
    private float lastSoundTime = 0f;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();

        if (grabInteractable == null)
            Debug.LogError("Brakuje komponentu XRGrabInteractable!");
        if (audioSource == null)
            Debug.LogError("Brakuje komponentu AudioSource!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!grabInteractable.isSelected)
            return;

        if (Time.time - lastSoundTime < soundCooldown)
            return;
        
        audioSource.Play();
        lastSoundTime = Time.time;
    }
}