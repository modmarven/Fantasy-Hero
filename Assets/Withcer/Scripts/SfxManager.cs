using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public AudioClip[] slashSound;
    public AudioSource audioSource;

    void Start()
    {

    }

    public void PlaySlashSound()
    {
        int randomIndex = Random.Range(0, slashSound.Length);
        audioSource.PlayOneShot(slashSound[randomIndex]);
    }
}
