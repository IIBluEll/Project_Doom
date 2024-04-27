using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HP_Bar : MonoBehaviour
{
   public CharacterStatus targetStatus;

   [SerializeField] protected Image fillHP;
   [SerializeField] protected Image changedHP;

   [SerializeField] protected float animationTime = 0.7f;

   private void OnEnable()
   {
      targetStatus.OnHpChanged += Draw;
   }

   public void Draw(int current, int max)
   {
      Draw((float) current / max);
   }

   public void Draw(float percent)
   {
      fillHP.fillAmount = percent;
      changedHP.DOFillAmount(percent, animationTime).SetEase(Ease.Linear);
   }

   public void End()
   {
      targetStatus.OnHpChanged -= Draw;
   }
}
