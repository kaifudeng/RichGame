using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameProcessUI : NetworkBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    [ClientRpc]
    public void OnGameProcessInfoChange(string gameProcessInfo)
    {
        textMeshProUGUI.text += gameProcessInfo;
    }
}
