using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : SingleTon<UI_Manager>
{
    [SerializeField] private Text shortMessage;


    public void ShowShortMessageToPlayer(string message)
    {
        shortMessage.text = message;
        shortMessage.gameObject.SetActive(true);
    }

    public void ExitShortMessageToPlayer()
    {
        shortMessage.gameObject.SetActive(false);
    }
}
