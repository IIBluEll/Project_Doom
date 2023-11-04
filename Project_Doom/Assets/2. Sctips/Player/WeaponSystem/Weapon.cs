using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
using TMPro;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [Header("Throwing")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Pickup")]
    public float animTime;

    [Header("Shooting")]
    public int maxAmmo;
    public int shotsPerSecond;
    public float reloadSpeed;
    public float hitForce;
    public float range;
    public bool tapable;
    public float kickbackForce;
    public float resetSmooth;
    public Vector3 scopePos;

    [Header("Data")]
    public int weaponGfxLayer;
    public GameObject[] weaponGfxs;
    public Collider[] gfxColliders;

    private float rotationTime;
    private float time;
    [SerializeField] private bool isHold;
    private bool isScoping;
    private bool isReloading;
    private bool isShooting;
    private int ammo;
    
    private Rigidbody rb;
    private Transform playerCamera;
    private TMP_Text ammoText;
    
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        ammo = maxAmmo;
    }

    private void Update()
    {
        if (!isHold)
        {
            return;
        }

        if (time < animTime)
        {
            time += Time.deltaTime;
            time = Clamp(time, 0f, animTime);

            float delta = -(Cos(PI * (time / animTime)) - 1f) / 2f;

            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, delta);
            transform.localRotation = Quaternion.Lerp(startRotation, Quaternion.identity, delta);
        }
        else
        {
            isScoping = Input.GetMouseButton(1) && !isReloading;
            
            transform.localRotation = Quaternion.identity;
            transform.localPosition =
                Vector3.Lerp(transform.localPosition, isScoping ? scopePos : Vector3.zero,
                    resetSmooth * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && ammo < maxAmmo)
        {
            StartCoroutine(ReloadCoolDown());
        }

        if ((tapable ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0)) && !isShooting && !isReloading)
        {
            ammo--;
            Shoot();
            StartCoroutine(ammo <= 0 ? ReloadCoolDown() : ShootingCoolDown());
        }
    }

    private void Shoot()
    {
        transform.localPosition -= new Vector3(0, 0, kickbackForce);

        if (!Physics.Raycast(playerCamera.position, playerCamera.forward, out var hitInfo, range))
        {
            return;
        }

        var rb = hitInfo.transform.GetComponent<Rigidbody>();

        if (rb == null)
        {
            return;
        }

        rb.velocity += playerCamera.forward * hitForce;
    }

    private IEnumerator ShootingCoolDown()
    {
        isShooting = true;
        yield return new WaitForSeconds(1f / shotsPerSecond);
        isShooting = false;
    }

    private IEnumerator ReloadCoolDown()
    {
        isReloading = true;
        
        //ToDo : Reload Animation

        yield return new WaitForSeconds(reloadSpeed);
        ammo = maxAmmo;
        isReloading = false;
    }
    
    public bool IsScoping => isScoping;

    public void PickUp(Transform _WeaponHolder, Transform _PlayerCamera)
    {
        if (isHold)
        {
            return;
        }

        Destroy(rb);

        transform.parent = _WeaponHolder;
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        foreach (Collider col in gfxColliders)
        {
            col.enabled = false;
        }
        
        foreach (GameObject gfx in weaponGfxs)
        {
            gfx.layer = weaponGfxLayer;
        }

        isHold = true;
        this.playerCamera = _PlayerCamera;

        isScoping = false;
    }

    public void Drop(Transform _playerCamera)
    {
        if (!isHold)
        {
            return;
        }

        rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 0.1f;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Vector3 forward = _playerCamera.forward;
        forward.y = 0f;

        rb.velocity = forward * throwForce;
        rb.velocity += Vector3.up * throwExtraForce;
        rb.angularVelocity = Random.onUnitSphere * rotationForce;

        foreach (Collider col in gfxColliders)
        {
            col.enabled = true;
        }
        
        foreach (GameObject gfx in weaponGfxs)
        {
            gfx.layer = 0;
        }

        transform.parent = null;
        isHold = false;
    }


}
