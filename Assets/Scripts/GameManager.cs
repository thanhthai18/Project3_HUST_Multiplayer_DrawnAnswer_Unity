using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public Camera mainCamera;

    [Header("Stats")]
    public bool gameEnded = false;
    public float maxTurn;
    public string currentGameAnswer;
    public int timeCouting;
    public int maxTimeTurn;
    public Coroutine timeCoroutine;
    public int index;
    public Dictionary<string, int> listScore = new Dictionary<string, int>();

    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerManager[] players;
    public int indexPlayerTurn;
    public int playersInGame;

    [Header("SrceenPlay")]
    public GameObject panelDraw;
    public GameObject panelAnswer;


    [Header("Data")]
    public ScriptableObj_Data data;

    [Space]
    public RectTransform panelPaper;
    public Canvas canvas;
    public Vector2 VectoClamMouse;
    public string[,] arrayGoiY_Answer = new string[3, 4];
    public bool isFirst;



    private void Awake()
    {
        instance = this;
        isFirst = true;
    }

    private void Start()
    {
        SetSizeCamera();
        SetVectoClamMouse();
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


    void SetVectoClamMouse()
    {
        Vector2 tmpCanvasSizeHalf = new Vector2(canvas.GetComponent<RectTransform>().rect.width * 1.0f / 2, canvas.GetComponent<RectTransform>().rect.height * 1.0f / 2);
        Vector2 tmpPaperSizeHalf = new Vector2(panelPaper.rect.width * 1.0f / 2, panelPaper.rect.height * 1.0f / 2);
        Vector2 tmpRatioSize = new Vector2(tmpPaperSizeHalf.x / tmpCanvasSizeHalf.x, tmpPaperSizeHalf.y / tmpCanvasSizeHalf.y);
        VectoClamMouse = new Vector2(mainCamera.orthographicSize * (Screen.width * 1.0f / Screen.height) * tmpRatioSize.x, mainCamera.orthographicSize * tmpRatioSize.y);
        Debug.Log(tmpCanvasSizeHalf);
        Debug.Log(tmpPaperSizeHalf);
        Debug.Log(mainCamera.orthographicSize * (Screen.width * 1.0f / Screen.height));
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
            //UIManager.instance.SetTextTime(timeCouting);
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
            GetPlayer(indexPlayerTurn).isMyTurn = false;
            photonView.RPC("SetIconDefault", RpcTarget.All);
            StopCoroutine(timeCoroutine);
            //photonView.RPC("StopTime", RpcTarget.All);

        }

        indexPlayerTurn = playerId;
        GetPlayer(indexPlayerTurn).SetPlayScreen(true);
        GetPlayer(indexPlayerTurn).isMyTurn = true;
        //if()

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                players[i].GetComponent<DrawLine>().Pun_BtnBlack_ResetColor();
                players[i].GetComponent<DrawLine>().currentSortOder = 0;
                players[i].GetComponent<DrawLine>().ButtonClearImage();
                if (players[i].inputAnswer != null)
                {
                    if (players[i].inputAnswer.gameObject.activeSelf)
                    {
                        players[i].inputAnswer.text = "";
                    }
                }
                if (players[i].id != indexPlayerTurn)
                {
                    Debug.Log(players[i].id);

                    players[i].SetPlayScreen(false);
                    players[i].isMyTurn = false;
                } // xu ly sau
            }
        }

        photonView.RPC("SetDraw", RpcTarget.All, playerId);
        timeCoroutine = StartCoroutine(countTime());
        //photonView.RPC("StartTime", RpcTarget.All);
    }

    //[PunRPC]
    //public void StartTime()
    //{
    //    timeCoroutine = StartCoroutine(countTime());
    //}
    //[PunRPC]
    //public void StopTime()
    //{
    //    StopCoroutine(timeCoroutine);
    //}

    [PunRPC]
    public void SetQuestion(int ran, int ranAswer)
    {
        photonView.RPC("SetTextGoiY", RpcTarget.All, data.listGoiY[ran]);
        if (ran == 0)
        {
            photonView.RPC("SetTextAnswer", RpcTarget.All, data.listAnswer_0[ranAswer]);
        }
        else if (ran == 1)
        {
            photonView.RPC("SetTextAnswer", RpcTarget.All, data.listAnswer_1[ranAswer]);
        }
        else if (ran == 2)
        {
            photonView.RPC("SetTextAnswer", RpcTarget.All, data.listAnswer_2[ranAswer]);
        }


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
