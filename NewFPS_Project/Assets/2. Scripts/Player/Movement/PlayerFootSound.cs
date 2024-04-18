using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerFootSound : MonoBehaviour
{
    private PlayerMove playerMove;
    private AudioSource audioSource;
    // 플레이어 발소리
    public AudioClip[] grassFootSound;  // 풀 발소리
    public AudioClip[] concreteFootSound; // 콘크리트 발소리
    public AudioClip[] woodFootSound; // 콘크리트 발소리
    public AudioClip[] sandFootSound; // 콘크리트 발소리

    public float checkDistance = 1f;
    public LayerMask groundLayer;

    public const string Grass = "Grass";
    public const string Concrete = "Concrete";
    public const string Wood = "Wood";
    public const string Sand = "Sand";
    
    private RaycastHit hit;
    
    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(PlayerFootSoundCo());
    }

    private IEnumerator PlayerFootSoundCo()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance))
        {
            CheckWhatIsGround(hit.collider.tag);   
        }
        
        if (playerMove.moveSpeed >= 3.9 && playerMove.moveSpeed < 5.9 && playerMove.isGrounded)
        {
            audioSource.volume = .7f;
            audioSource.Play();
            yield return new WaitForSeconds(.75f);
        }
        else if (playerMove.moveSpeed >= 5.9 && playerMove.isGrounded)
        {
            audioSource.volume = 1;
            audioSource.Play();
            yield return new WaitForSeconds(.25f);
        }
        else
        {
            audioSource.Stop();
            yield return new WaitForSeconds(1f);
        }

        if (!playerMove.isPlayerDie)
        {
            StartCoroutine(PlayerFootSoundCo());
        }
        
    }

    private void CheckWhatIsGround(string tagName)
    {
        switch (tagName)
        {
            case Grass :
                audioSource.clip = grassFootSound[Random.Range(0, grassFootSound.Length)];
                break;
            
            case Concrete :
                audioSource.clip = concreteFootSound[Random.Range(0, concreteFootSound.Length)];
                break;
            
            case Wood :
                audioSource.clip = woodFootSound[Random.Range(0, woodFootSound.Length)];
                break;
            
            case Sand :
                audioSource.clip = sandFootSound[Random.Range(0, sandFootSound.Length)];
                break;
        }
    }
}
