using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CustomBullet : Bullet
{
    public int BulletDamage;

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player HIT!!!");
            if (isExplodeBullet)
            {
                Explode();
            }
            
            other.GetComponent<PlayerStatus>().TakeDamage(BulletDamage);
            StartCoroutine(DestroyBullets(0));
        }
        else
        {
            Debug.Log("EnemyBullet Hit [ " + other.name + " ]");
            collisions++;
        }
    }

}
