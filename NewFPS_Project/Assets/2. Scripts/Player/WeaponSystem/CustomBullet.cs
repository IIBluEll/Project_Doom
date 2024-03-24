using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask enemy;
    
    // 총알 물리 
    [Range(0f, 1f)] 
    public float bounciness;
    public bool userGravity;
    
    // 폭발 
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;
    
    // 도탄
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;
    
    int collisions;
    PhysicMaterial physics_mat;

    private void SetUp()
    {
        physics_mat = new PhysicMaterial()
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        
        GetComponent<SphereCollider>().material = physics_mat;
        rb.useGravity = userGravity;
    }

    private void Start()
    {
        SetUp();
    }

    private void Update()
    {
        if(collisions > maxCollisions || maxLifeTime <= 0)
        {
            Explode();
        }
        
        maxLifeTime -= Time.deltaTime;
    }

    private void Explode()
    {
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
    }
}
