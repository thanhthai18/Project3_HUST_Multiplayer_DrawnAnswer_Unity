using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MenuController : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button btnCreateRoom;
    public Button btnJoinRoom;

    [Header("Lobby Screen")]
    public TextMeshProUGUI txtPlayerList;
    public Button btnStartGame;

    private void Start()
    {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;


    }

    void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        screen.SetActive(true);
    }

    public void OnClickCreateRoom(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public void OnClickJoinRoom(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }
}
