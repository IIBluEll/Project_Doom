using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunSystem : MonoBehaviour
{
   public GameObject bullet;                                                  // 총알 프리펩
   
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
   
   // 그래픽
   [Space(10f),Header("Graphic Reference")]
   public GameObject muzzleFlashPrefab, bulletHoleGraphicPrefab;
   public Text ammoText;
   
   // 키 입력
   public KeyCode reloadKey = KeyCode.R;
   public KeyCode shootingKey = KeyCode.Mouse0;

   private void Awake()
   {
      bulletsLeft = magazineSize;
      readyToShoot = true;
   }

   private void Update()
   {
      
   }

   private void CheckInput()
   {
      shooting = allowButtonHold ? Input.GetKey(shootingKey) : Input.GetKeyDown(shootingKey);

      if (Input.GetKeyDown(reloadKey) && bulletsLeft < magazineSize && !reloading)
      {
         // 재장전 메서드
      }

      if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
      {
         // 사격 메서드
      }
   }

   private void Shoot()
   {
      readyToShoot = false;
      bulletShot = 0;
      
      
   }
}
