using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WeaponManager : MonoBehaviour
{
   [Header("PickUp Value")]
   public float pickUpRange;
   public float pickUpRadius;

   [Header("Sway System")] 
   public int weaponLayer;
   public float swaySize;
   public float swaySmooth;

   [Header("Fov Value")] 
   public float defaultFov;
   public float scopedFov;
   public float fovSmooth;

   [Header("Transform")] 
   public Transform weaponHolder;
   public Transform playerCamera;
   public Transform swayHolder;

   public Camera[] playerCams;
   
   [SerializeField]
   private bool isWeaponHold;
   private Weapon holdWeapon;

   private void Update()
   {
      foreach (Camera cam in playerCams)
      {
         cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 
            isWeaponHold && holdWeapon.IsScoping ? scopedFov : defaultFov, fovSmooth * Time.deltaTime);
      }
      
      if (isWeaponHold)
      {
         var mouseDelta = -new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

         swayHolder.localPosition = Vector3.Lerp(swayHolder.localPosition, Vector3.zero, swaySmooth * Time.deltaTime);
         swayHolder.localPosition += (Vector3)mouseDelta * swaySize;

         if (Input.GetKeyDown(KeyCode.Q))
         {
            holdWeapon.Drop(playerCamera);
            holdWeapon = null;
            isWeaponHold = false;
         }
      }
      else if (Input.GetKeyDown(KeyCode.E))
      {
         var hitList = new RaycastHit[256];

         var realList = new List<RaycastHit>();
         
         int hitnumber = Physics.CapsuleCastNonAlloc(playerCamera.position,
            playerCamera.position + playerCamera.forward * pickUpRange, pickUpRadius, 
            playerCamera.forward, hitList);

         for (int i = 0; i < hitnumber; i++)
         {
            var hit = hitList[i];
            
            if (hit.transform.gameObject.layer != weaponLayer)
            {
               continue;
            }

            if (hit.point == Vector3.zero)
            {
               realList.Add(hit);
            }
            else if (Physics.Raycast(playerCamera.position, hit.point - playerCamera.position, out var hitInfo,
                        hit.distance + 0.1f) && hitInfo.transform == hit.transform)
            {
               realList.Add(hit);
            }
         }

         if (realList.Count == 0)
         {
            return;
         }
         
         realList.Sort((hit1, hit2) =>
         {
            var dist1 = GetDistanceTo(hit1);
            var dist2 = GetDistanceTo(hit2);
            return Mathf.Abs(dist1 - dist2) < 0.001f ? 0 : dist1 < dist2 ? -1 : 1;
         });


         isWeaponHold = true;
         holdWeapon = realList[0].transform.GetComponent<Weapon>();
         holdWeapon.PickUp(weaponHolder,playerCamera);
      }
   }

   private float GetDistanceTo(RaycastHit hit)
   {
      return Vector3.Distance(playerCamera.position, 
         hit.point == Vector3.zero ? hit.transform.position : hit.point);
   }
}
