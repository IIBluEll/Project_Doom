using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletPassAroundSound : MonoBehaviour
{
    public AudioClip[] passSounds;
    public AudioSource audioSource;

    public bool alreadySoundPlay = false;
    
    private void Awake()
    {
        audioSource.clip = passSounds[Random.Range(0, passSounds.Length)];
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("탄약스침 소리!");
        
        if (other.gameObject.CompareTag("Player") && alreadySoundPlay == false)
        {
            audioSource.Play();
            alreadySoundPlay = true;
        }
    }
}
