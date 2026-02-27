using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

public class RoomSceneScript : NetworkBehaviour
{
    // Start is called before the first frame update
    public MyRoomManager networkManager;

    public GameObject rulesTable;

    public GameObject compsTable;
    void Start()
    {
        // È·±£ NetworkManager ´æÔÚ
        if (networkManager == null)
            networkManager = FindObjectOfType<MyRoomManager>();
        if (!isServer)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OutToRoomButtonClick()
    {
        networkManager.StopHost();
        //networkManager.ServerChangeScene(networkManager.offlineScene);
    }

    public void SettingButtonClick()
    {
        if (!isServer)
        {
            return;
        }
        if (rulesTable != null)
        {
            if (!rulesTable.activeSelf)
            {
                rulesTable.SetActive(true);
                compsTable.SetActive(false);
            }
            else
            {
                rulesTable.SetActive(false);
            }
        }
    }

    public void SettingCompaniesButtonClick()
    {
        if (!isServer)
        {
            return;
        }
        if (compsTable != null)
        {
            if (!compsTable.activeSelf)
            {
                compsTable.SetActive(true);
                rulesTable.SetActive(false);
            }
            else
            {
                compsTable.SetActive(false);
            }
        }
    }
}
