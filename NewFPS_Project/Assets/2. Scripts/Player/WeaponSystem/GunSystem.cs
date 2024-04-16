using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GunSystem : MonoBehaviour
{
   public GameObject bullet;                                                  // 총알 프리펩
   public GameObject bulletCasing;                                            // 탄피 프피펩
   // 총기 스탯
   [Header("Gun Stats")]
   public float shootForce, upwardForce;                                      // 총알 발사 힘, 상향 힘
   public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;    // 발사 간격, 탄퍼짐, 사정거리, 재장전 시간, 연속 발사 간의 시간
   public int magazineSize, bulletsPerTap;                                    // 탄창 크기, 탭당 발사하는 총알 수
   public bool allowButtonHold;                                               // 연속 발사 허용 여부
   public float recoilForce;                                                  // 반동 힘
   private int bulletsLeft, bulletShot;                                       // 남은 총알 수, 발사된 총알 수
   
   // 상태 변수
   private bool shooting, readyToShoot, reloading;                            // 발사 중, 발사 준비 완료, 재장전 중

   [Space(10f), Header("Reference")] 
   public Rigidbody playerRb;
   public Camera cam;
   public Transform attackPoint;
   public Transform bulletCasingPoint;
   
   // 그래픽
   [Space(10f),Header("Graphic Reference")]
   public GameObject muzzleFlashPrefab, bulletHoleGraphicPrefab;
   public Text ammoText;

   public CameraShaker camShaker;
   public float camShakeMagnitude, camShakeDuration;
   
   // 정조준 기능
   public Transform gunHolder;
   public Vector3 aimPosition;
   public Vector3 originalPosition;
   
   // 키 입력
   public KeyCode reloadKey = KeyCode.R;
   public KeyCode shootingKey = KeyCode.Mouse0;
   public KeyCode trueAimKey = KeyCode.Mouse1;
   
   public bool isTrueAim = false;

   private void Awake()
   {
      bulletsLeft = magazineSize;
      readyToShoot = true;

      originalPosition = gunHolder.localPosition;
   }

   private void Update()
   {
      PlayerInput();
      HandleAiming();
   }

   private void HandleAiming()
   {
      if (Input.GetKeyDown(trueAimKey))
      {
         StartAiming();
      }

      if (Input.GetKeyUp(trueAimKey))
      {
         StopAiming();
      }
   }

   private void StartAiming()
   {
      isTrueAim = true;
      gunHolder.localPosition = aimPosition;
   }

   private void StopAiming()
   {
      isTrueAim = false;
      gunHolder.localPosition = originalPosition;
   }

   private void PlayerInput()
   {
      shooting = allowButtonHold ? Input.GetKey(shootingKey) : Input.GetKeyDown(shootingKey);
      
      if (Input.GetKeyDown(reloadKey) && bulletsLeft < magazineSize && !reloading)
      {
         StartCoroutine(Reload());
      }

      if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
      {
         Shoot();
      }
   }
   
   private void Shoot()
   {
      readyToShoot = false;
      bulletShot = 0;

      Vector3 directionWithSpread;
      
      if (isTrueAim)
      {
         directionWithSpread = CalculateDirectionWithSpread(0);
      }
      else
      {
         directionWithSpread = CalculateDirectionWithSpread(spread);
      }

      // 오브젝트 풀에서 총알 인스턴스 가져옴
      GameObject currentBullet = ObjectPool.Spawn(bullet, attackPoint.position, Quaternion.identity);
      currentBullet.transform.forward = directionWithSpread.normalized;
      
      // 총알에 힘 적용
      currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
      currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

      // 탄피 생성
      GameObject currentCasing = ObjectPool.Spawn(bulletCasing, bulletCasingPoint.position, quaternion.identity);
      currentCasing.GetComponent<BulletCasing>().SetUpBulletCasing(bulletCasingPoint.transform.right);
      
      // 카메라 흔들림
      camShaker.ShakeOnce(camShakeMagnitude, .5f, camShakeDuration, camShakeDuration);
      
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

   // 랜덤 탄퍼짐 구현 
   private Vector3 CalculateDirectionWithSpread(float spread)
   {
      Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
      RaycastHit hit;

      // 레이캐스트가 hit 되면 그 위치 or 지정한 거리만큼 멀리 있는 지점 좌표
      Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(75);
      
      // 총알 발사 지점에서 타겟 지점까지의 방향 
      Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
      
      // 탄퍼짐
      float x = Random.Range(-spread, spread);
      float y = Random.Range(-spread, spread);

      // 기존 방향에 탄퍼짐을 추가한 최종 방향 리턴
      return directionWithoutSpread + new Vector3(x, y, 0);
   }

}
