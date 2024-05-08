using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GrazeBullet_Sound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] grazeClips;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyBullet"))
        {
            return;
        }
        
        int ranNum = Random.Range(0, grazeClips.Length);
        audioSource.clip = grazeClips[ranNum];
        
        audioSource.Play();
    }
}
