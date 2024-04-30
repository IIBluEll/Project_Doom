using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CustomBullet : Bullet
{
    public int BulletDamage;
    protected override void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Bullet"))
        {
            return;
        }

        if (other.collider.CompareTag("Player"))
        {
            Debug.Log("Player HIT!!!");
            if (isExplodeBullet)
            {
                Explode();
            }
            
            other.collider.GetComponent<PlayerStatus>().TakeDamage(BulletDamage);
            StartCoroutine(DestroyBullets(0));
        }
        else
        {
            collisions++;
        }
    }

}
