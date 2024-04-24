using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : CharacterStatus
{
    [SerializeField, Tooltip("위기 상태 체력 비율"), Range(0.01f, 1.0f)]
    private float criticalHpRate = 0.3f;

    private bool isCritical;

    public event Action onEnterCritical;
    public event Action onExitCritical;

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (!isCritical && (float)currentHP / maxHP <= criticalHpRate)
        {
            isCritical = true;
            onEnterCritical?.Invoke();
        }
    }

    public override void TakeHeal(int amount)
    {
        base.TakeHeal(amount);

        if (isCritical && (float)currentHP / maxHP > criticalHpRate)
        {
            isCritical = false;
            onExitCritical?.Invoke();
        }
    }
}
