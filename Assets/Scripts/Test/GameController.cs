using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;



public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController instance;

    public Camera mainCamera;

    [Header("Stats")]
    public bool gameEnded = false;
    public float timeToWin;
    public float invincibleDuration;
    public float hatPickupTime;


    [Header("Players")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    public int playersInGame;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetSizeCamera();
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC(nameof(ImInGame), RpcTarget.AllBuffered);
    }

    void SetSizeCamera()
    {
        float f1, f2;
        f1 = 9.0f / 16;
        f2 = Screen.width * 1.0f / Screen.height;
        mainCamera.orthographicSize *= f1 / f2;
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }



    public PlayerController GetPlayer(int playerId)
    {
        return players.First(s => s.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(s => s.gameObject == playerObj);
    }


    [PunRPC]
    public void GiveHat(int playerId, bool initalGive)
    {
        if (!initalGive)
        {
            GetPlayer(playerWithHat).SetHat(false);
        }

        playerWithHat = playerId;
        GetPlayer(playerWithHat).SetHat(true);
        hatPickupTime = Time.time;
    }

    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibleDuration)
            return true;
        else
            return false;
    }

    [PunRPC]
    void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke(nameof(GoBackToMenu), 3f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
