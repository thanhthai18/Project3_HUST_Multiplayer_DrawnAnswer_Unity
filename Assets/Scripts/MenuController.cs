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

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC(nameof(UpdateLobbyUI), RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        txtPlayerList.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            txtPlayerList.text += player.NickName + "\n";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            btnStartGame.interactable = true;
        }
        else
        {
            btnStartGame.interactable = false;
        }
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        if (PhotonNetwork.PlayerList.Length > 1 && PhotonNetwork.PlayerList.Length < 5)
        {
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "MainGame");

        }
    }
}
