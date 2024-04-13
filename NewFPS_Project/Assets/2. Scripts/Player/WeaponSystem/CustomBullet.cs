using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 커스텀 탄환 스크립트
/// </summary>

public class CustomBullet : MonoBehaviour
{
    // Assign Field
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
    private int maxCollisions;
    private float maxLifeTime;
    
    // 리셋을 위함
    public int setMaxCollisions;
    public float setMaxLifeTime;

    public bool isExplodeBullet;
    
    int collisions;
    PhysicMaterial physics_mat;

    private void SetUp()
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
        
        GetComponent<SphereCollider>().material = physics_mat;
    }

    private void OnEnable()
    {
       SetUp();
    }

    private void Update()
    {
        // 폭발성 탄환일 경우 폭발
        if (isExplodeBullet)
        {
            if(collisions > maxCollisions || maxLifeTime <= 0)
            {
                Explode();
            }
        }
        // 아닐 경우 그냥 삭제
        else
        {
            if (maxLifeTime <= 0)
            {
                StartCoroutine(DestoryBullets(0));
            }
        }
        
        maxLifeTime -= Time.deltaTime;
    }

    private IEnumerator DestoryBullets(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Unspawn(this.gameObject);
    }
  
    private void Explode()
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

        StartCoroutine(DestoryBullets(0));
    }

    private void BulletHit()
    {
        // 상대 피해 함수
        
        StartCoroutine(DestoryBullets(0));
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Bullet"))
        {
            return;
        }
        
        if (other.collider.CompareTag("Enemy"))
        {
            if (isExplodeBullet)
            {
                Explode();
            }
            else
            {
                BulletHit();
            }
        }
        else
        {
            collisions++;
        }
    }

    // 에디터에서 폭발 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,explosionRange);
    }
}
