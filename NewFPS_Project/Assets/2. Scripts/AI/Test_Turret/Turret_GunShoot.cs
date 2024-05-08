using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Turret_GunShoot : MonoBehaviour
{
    public GameObject enemyBullet;
    public Transform attackPoint;
    
    // 그래픽
    [Space(10f), Header("Graphic Reference")]
    public GameObject muzzleFlashPrefab;
    
    public bool shooting, readyToShoot, reloading;    
    public float shootForce, upwardForce; 
    private int bulletsLeft, bulletShot;
    public int magazineSize, bulletsPerTap; 
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    
    private AudioSource audioSource;
    
    //Debug
    public Vector3 debugRay;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        readyToShoot = true;
    }

    private void Update()
    {
        if (readyToShoot  && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }
        else if (!reloading && bulletsLeft <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        bulletShot = 0;

        Vector3 directionWithSpread = CalculateDirectionWithSpread();
        
        GameObject currentBullet = ObjectPool.Spawn(enemyBullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;
        
        // 총알에 힘 적용
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(attackPoint.transform.up * upwardForce, ForceMode.Impulse);
        
        bulletsLeft--;
        bulletShot++;
        
        if (muzzleFlashPrefab != null)
        {
            muzzleFlashPrefab.SetActive(true);

            StartCoroutine(ReturnMuzzleFlash(.05f));
        }

        if (bulletShot < bulletsPerTap && bulletsLeft > 0)
        {
            StartCoroutine(ShootWithDelay(timeBetweenShooting));
        }
        else
        {
            StartCoroutine(ResetShot(timeBetweenShooting));
        }
    }

    private IEnumerator ReturnMuzzleFlash(float delay)
    {
        yield return new WaitForSeconds(delay);
        muzzleFlashPrefab.SetActive(false);
    }

    private IEnumerator ShootWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Shoot();
    }

    // 발사 상태 복구
    private IEnumerator ResetShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToShoot = true; 
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsLeft = magazineSize;   // 탄창 다시 Full
        reloading = false;
    }
    
    private Vector3 CalculateDirectionWithSpread()
    {
        Ray ray = new Ray(attackPoint.position, attackPoint.forward);
        RaycastHit hit;
        
        // 레이캐스트가 hit 되면 그 위치 or 지정한 거리만큼 멀리 있는 지점 좌표
        Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(100);

        // 총알 발사 지점에서 타겟 지점까지의 방향 
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        
        // 탄퍼짐
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
      
        debugRay = directionWithoutSpread + new Vector3(x, y, 0);
        
        // 기존 방향에 탄퍼짐을 추가한 최종 방향 리턴
        return directionWithoutSpread + new Vector3(x, y, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 rayDirection = attackPoint.forward * 50;
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + rayDirection);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + debugRay);
    }
}
