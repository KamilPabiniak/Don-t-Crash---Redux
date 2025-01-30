using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    [SerializeField] private AudioSource ballAudio;
    [SerializeField] private AudioClip[] soundsBall;

    private void OnCollisionEnter(Collision other)
    {
        int randomNum = Random.Range(0, soundsBall.Length);
        ballAudio.clip = soundsBall[randomNum];
        ballAudio.Play();
    }
}
