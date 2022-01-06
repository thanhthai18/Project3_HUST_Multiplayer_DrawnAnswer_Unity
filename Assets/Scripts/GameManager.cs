using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public Camera mainCamera;

    [Header("Stats")]
    public bool gameEnded = false;
    public float maxTurn;
    public string[] gameAnswer;
    public string currentGameAnswer;
    public int timeCouting;
    public int maxTimeTurn;
    public Coroutine timeCoroutine;
    public int index;
    public Dictionary<string ,int> listScore = new Dictionary<string, int>();




    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerManager[] players;
    public int indexPlayerTurn;
    public int playersInGame;

    [Header("SrceenPlay")]
    public GameObject panelDraw;
    public GameObject panelAnswer;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetSizeCamera();
        players = new PlayerManager[PhotonNetwork.PlayerList.Length];
        photonView.RPC(nameof(ImInGame), RpcTarget.AllBuffered);
        maxTimeTurn = 30;
        timeCouting = maxTimeTurn;
    }

    void SetSizeCamera()
    {
        float f1, f2;
        f1 = 9.0f / 16;
        f2 = Screen.width * 1.0f / Screen.height;
        mainCamera.orthographicSize *= f1 / f2;
    }

    [PunRPC]
    public IEnumerator countTime()
    {
        timeCouting = maxTimeTurn;
        while (timeCouting > 0)
        {
            yield return new WaitForSeconds(1);
            timeCouting--;
            photonView.RPC("SetTextTime", RpcTarget.All, timeCouting);
            if (timeCouting == 0)
            {
                if (index == playersInGame)
                {
                    Debug.Log("End game");
                    photonView.RPC(nameof(ShowRank), RpcTarget.All);
                }
                else
                {
                    index = indexPlayerTurn + 1;
                    GiveTurn(index, false);
                }


            }

        }
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, Vector3.zero, Quaternion.identity);
        PlayerManager playerScript = playerObj.GetComponent<PlayerManager>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }


    public PlayerManager GetPlayer(int playerId)
    {
        return players.First(s => s.id == playerId);
    }

    public PlayerManager GetPlayer(GameObject playerObj)
    {
        return players.First(s => s.gameObject == playerObj);
    }




    public void GiveTurn(int playerId, bool initalGive)
    {
        if (!initalGive)
        {
            GetPlayer(indexPlayerTurn).SetPlayScreen(false);
            photonView.RPC("SetIconDefault", RpcTarget.All);
            StopCoroutine(timeCoroutine);
        }

        indexPlayerTurn = playerId;
        GetPlayer(indexPlayerTurn).SetPlayScreen(true);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                if (players[i].id != indexPlayerTurn)
                {
                    Debug.Log(indexPlayerTurn);
                    GetPlayer(players[i].id).SetPlayScreen(false);
                }
            }
        }

        photonView.RPC("SetDraw", RpcTarget.All, playerId);

        timeCoroutine = StartCoroutine(countTime());
    }

    //public void CheckScore()
    //{
    //    for (int i = 0; i < players.Length; i++)
    //    {
    //        listScore.Add(players[i].photonPlayer.NickName, players[i].score);
    //    }
    //    listScore.
    //}

    [PunRPC]
    void ShowRank()
    {
        gameEnded = true;
        //PlayerController player = GetPlayer(playerId);
        UIManager.instance.SetPanelRank();
        Invoke(nameof(GoBackToMenu), 5f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
