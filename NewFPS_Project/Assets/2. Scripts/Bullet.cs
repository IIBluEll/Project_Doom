using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Assign Field
    [Header("Assign Field")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask enemy;

    [Space(5f), Header("총알 물리")] 
    public float bounciness;
    public bool userGravity;
    
    // 폭발 
    [Space(5f), Header("폭팔 속성")]
    public bool isExplodeBullet;
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;
    
    // 도탄
    protected int maxCollisions;
    protected float maxLifeTime;
    
    // 리셋을 위함
    [Space(5f), Header("도탄 속성")]
    public int setMaxCollisions;
    public float setMaxLifeTime;
    
    protected int collisions;
    private bool isDestroying = false;
    private PhysicMaterial physics_mat;

    protected virtual void OnEnable()
    {
        maxCollisions = setMaxCollisions;
        maxLifeTime = setMaxLifeTime;
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = userGravity;
        
        physics_mat = new PhysicMaterial()
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        
        GetComponent<CapsuleCollider>().material = physics_mat;
    }

    protected void Update()
    {
        if ((collisions > maxCollisions || maxLifeTime <= 0) && !isDestroying)
        {
            StartCoroutine(DestroyBullets(1));
        }

        maxLifeTime -= Time.deltaTime;
    }

    protected IEnumerator DestroyBullets(float time)
    {
        isDestroying = true;

        if (isExplodeBullet)
        {
            Explode();
        }

        yield return new WaitForSeconds(time);
        ObjectPool.Unspawn(this.gameObject);
    }

    protected virtual void Explode()
    {
        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }
        
        // 적 확인
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, enemy);

        for (int i = 0; i < enemies.Length; i++)
        {
            // 상대 피해 함수

            Rigidbody enemyRb = enemies[i].GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }
    }

    protected virtual void BulletHit()
    {
        
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,explosionRange);
    }
}
