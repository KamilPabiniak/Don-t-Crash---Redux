using UnityEngine;
using UnityEngine.Serialization;

public class AudioEventPlayer : MonoBehaviour
{
    public AudioSource leverKronkSource; 
    public AudioSource destructionSource; 
    public AudioSource beltSource; 
    public AudioClip firstClip;
    public AudioClip secondClip;
    public GameObject boom;
    
    public void PlayFirstSound()
    {
        if(leverKronkSource != null && firstClip != null)
        {
            leverKronkSource.clip = firstClip;
            leverKronkSource.Play();
        }
    }
    
    public void PlaySecondSound()
    {
        if(leverKronkSource != null && secondClip != null)
        {
            leverKronkSource.clip = secondClip;
            leverKronkSource.Play();
        }
    }

    public void PlayBoom()
    {
        if (boom != null)
        {
            boom.SetActive(true);
            destructionSource.Play();
        }
    }

    public void PlayBelt()
    {
        beltSource.Play();
    }
    
    public void StopBelt()
    {
        beltSource.Stop();
    }
}