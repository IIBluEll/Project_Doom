using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_Bullet_GrazeSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] grazeClips;

    private void OnEnable()
    {
        int ranNum = Random.Range(0, grazeClips.Length);
        audioSource.clip = grazeClips[ranNum];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        audioSource.Play();
        this.gameObject.SetActive(false);
    }
}
