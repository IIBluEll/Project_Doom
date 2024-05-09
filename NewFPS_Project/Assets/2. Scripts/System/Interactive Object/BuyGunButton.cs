using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyGunButton : MonoBehaviour
{
    [SerializeField] private GameObject prefabGun;
    
    [SerializeField] private string message = "구매하시려면 F 버튼을 눌러주세요";
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("InteractiveRay"))
            return;
        
        UI_Manager.Instance.ShowShortMessageToPlayer(message);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("InteractiveRay"))
            return;
        
        UI_Manager.Instance.ExitShortMessageToPlayer();
    }
}
