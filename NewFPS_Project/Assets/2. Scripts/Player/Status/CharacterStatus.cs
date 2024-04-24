using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  체력 / 스테미나 / 
/// </summary>

public class CharacterStatus : MonoBehaviour
{
    protected int maxHP = 100;
    protected int currentHP;

    protected bool isDead = false;

    public delegate void HpChangeHandler(int current, int max);

    public event HpChangeHandler OnHpChanged;
    public event Action OnDead;
    public event Action<int> OnDamaged;
    public event Action<int> OnHealed;
    
    public int CurrentHP
    {
        get => currentHP;
        set
        {
            currentHP = value;

            if (currentHP <= 0)
            {
                currentHP = 0;

                if (!isDead)
                {
                    isDead = true;
                    OnDead?.Invoke();
                }
            }
            else if(currentHP > maxHP)
            {
                currentHP = maxHP;
            }
            
            OnHpChanged?.Invoke(currentHP,maxHP);            
        }
    }

    public int MaxHP
    {
        get => maxHP;
        set
        {
            int changeAmount = Mathf.Max(value - maxHP, 0);
            maxHP = value;
            currentHP += changeAmount;
            
            OnHpChanged?.Invoke(currentHP,maxHP);
        }
    }

    protected virtual void OnEnable()
    {
        currentHP = maxHP;
        isDead = false;
    }

    private void Start()
    {
        // 초기 UI 갱신
        CurrentHP = currentHP;
    }

    public virtual void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        if (!isDead)
        {
            OnDamaged?.Invoke(amount);
        }
    }

    public virtual void TakeHeal(int amount)
    {
        CurrentHP += amount;

        if (!isDead)
        {
            OnHealed?.Invoke(amount);
        }
    }
}
