using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    private PlayerMove playerMove;
    private GunSystem gunSystem;
    private Animator animator;

    private void Awake()
    {
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        gunSystem = GetComponent<GunSystem>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("MoveSpeed",playerMove.moveSpeed);
    }
}
