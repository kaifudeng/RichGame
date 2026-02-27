//using Mirror;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class CardUI : MonoBehaviour
//{

//    //public string[] cards;

//    [Header("Child Text Objects")]
//    public Text val;

//    public MyButton c1;
//    public MyButton c2; public MyButton c3;
//    public MyButton c4;
//    public MyButton c5; public MyButton c6;

//    public List<MyButton> buttons;

//    public PlayerController playerController;
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }
//    private void Awake()
//    {
//        buttons = new List<MyButton>
//        {
//            c1,
//            c2,
//            c3,
//            c4,
//            c5,
//            c6
//        };
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    [Client]
//    public void OnCardsChange(string[] cards)
//    {
//        int index = 0;
//        foreach (MyButton button in buttons) 
//        {
//            TextMeshProUGUI btntext =button.GetComponentInChildren<TextMeshProUGUI>();
//            if (btntext != null&&index<cards.Length) 
//            {
//                GameObject btn = button.gameObject;
//                btn.SetActive(true);
//                btntext.text = cards[index];
//                index++;
//            }
//        }
//    }
//    [Client]
//    public void OnStateChange(bool state)
//    {
//        if (state == true)
//        {
//            foreach (MyButton button in buttons)
//            {
//                button.enabled = true;
//            }
//        }
//        else
//        {
//            foreach (MyButton button in buttons)
//            {
//                button.enabled = false;
//            }
//        }
//    }

//    public void CardOnclick(GameObject card)
//    {
//        foreach (MyButton button in buttons) 
//        {
//            if (button.name == card.name) 
//            {
//                GameObject btn = button.gameObject;
//                TextMeshProUGUI btntext = button.GetComponentInChildren<TextMeshProUGUI>();
//                playerController.AttackOneCard(btntext.text);
                
//                playerController.CmdSendPLayerMessage(playerController.PlayerName + "attack card " + btntext.text + "\n");
//                playerController.GetPlayerTotalAssets();
//                //buttons.Remove(button);
//                btn.SetActive(false);
//                break;
//            }
//        }
//        //RectTransform ui = GameInfoUI.GetPlayersInfoPanel();
//        //GameInfoUI gameInfoUI=ui.GetComponentInChildren<GameInfoUI>();
//        //if (gameInfoUI != null) 
//        //{
//        //    AddRealTimeInfo=gameInfoUI.AddRealTimeInfo;
//        //    AddRealTimeInfo?.Invoke(Time.time.ToString()+" "+ );
//        //}
//    }

//}
