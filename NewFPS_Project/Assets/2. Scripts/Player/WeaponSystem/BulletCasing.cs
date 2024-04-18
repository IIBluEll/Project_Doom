using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletCasing : MonoBehaviour
{
    public float deactiveTime = 3.0f;     // 탄피 생성 후 비활성화 시간
    public float casingSpin = 1.0f;       // 탄피 회전 속도
    public AudioClip[] audioClips;        // 탄피 충돌 사운드클립

    private Transform CasingPosition;
    private Rigidbody rigidBody;
    private AudioSource audioSource;

    public void SetUpBulletCasing(Vector3 direction)
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        // 탄피의 속도, 회전 
        rigidBody.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigidBody.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin));
        
        // 탄피 사운드 랜덤 재생
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];

        StartCoroutine(DeactivateAfterTime());
    }

    private void OnCollisionEnter(Collision other)
    {
        audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactiveTime);

        // 메모리풀에서 언스폰
        ObjectPool.Unspawn(this.gameObject);
    }
}
