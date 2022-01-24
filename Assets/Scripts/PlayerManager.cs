using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public int id;

    [Header("Info")]
    public bool isMyTurn;
    public int score;
    public string myAnswer;
    public TMP_InputField inputAnswer;
    public Button btnCheck;
    public GameObject VFXMe;



    [Header("Components")]
    public Player photonPlayer;

    private void Start()
    {
        btnCheck = UIManager.instance.btnCheck;
        inputAnswer = UIManager.instance.inputAnswer;
        btnCheck.onClick.AddListener(OnClickCheck);

    }

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        score = 0;

        GameManager.instance.players[id - 1] = this;

        if (photonView.IsMine)
        {
            VFXMe = GameManager.instance.canvas.transform.GetChild(0).GetChild(3).GetChild(id - 1).GetChild(0).gameObject;
            ItsMeFX();
        }

        if (id == 1)
        {
            isMyTurn = true;
            Invoke(nameof(DelayFirstGiveTurn), 0.1f);

        }
        else
        {
            isMyTurn = false;

        }

    }

    public void DelayFirstGiveTurn()
    {
        GameManager.instance.GiveTurn(1, true);

    }

    public void ItsMeFX()
    {
        VFXMe.GetComponent<Image>().DOFade(0.5f, 1).OnComplete(() =>
         {
             VFXMe.GetComponent<Image>().DOFade(0, 1).OnComplete(() =>
             {
                 if (gameObject != null)
                 {
                     ItsMeFX();
                 }
             });
         });
    }

    public void OnClickCheck()
    {
        if (photonView.IsMine)
        {
            myAnswer = inputAnswer.text;

            if (myAnswer != GameManager.instance.currentGameAnswer)
            {
                GameManager.instance.photonView.RPC("SetSad", RpcTarget.All, id);
            }
            else
            {
                btnCheck.interactable = false;
                inputAnswer.interactable = false;
                GameManager.instance.photonView.RPC("SetHappy", RpcTarget.All, id);
                UIManager.instance.txtAnswer.gameObject.SetActive(true);

                photonView.RPC(nameof(UpdateScore), RpcTarget.All);
            }
        }

    }

    [PunRPC]
    void UpdateScore()
    {
        if (GameManager.instance.timeCouting > 25)
        {
            score += 10;
        }
        else if (GameManager.instance.timeCouting > 10)
        {
            score += 5;
        }
        else if (GameManager.instance.timeCouting > 0)
        {
            score += 1;
        }
    }

    public void SetPlayScreen(bool isTurn)
    {
        if (photonView.IsMine)
        {
            if (isTurn)
            {
                GameManager.instance.panelDraw.gameObject.SetActive(true);
                GameManager.instance.panelAnswer.gameObject.SetActive(false);
                UIManager.instance.txtAnswer.gameObject.SetActive(true);
                UIManager.instance.txtGoiY.gameObject.SetActive(true);
                GetComponent<DrawLine>().Pun_BtnBlack_ResetColor();
                GetComponent<DrawLine>().currentSortOder = 0;
                GetComponent<DrawLine>().ButtonClearImage();
                if (inputAnswer != null)
                {
                    if (inputAnswer.gameObject.activeSelf)
                    {
                        inputAnswer.text = "";
                    }
                }
                

                int ran = Random.Range(0, GameManager.instance.data.listGoiY.Count);
                int ranAnswer = Random.Range(0, GameManager.instance.data.listAnswer_0.Count);
                GameManager.instance.photonView.RPC("SetQuestion", RpcTarget.All, ran, ranAnswer);
            }
            if (!isTurn)
            {
                btnCheck.interactable = true;
                inputAnswer.interactable = true;
                GetComponent<DrawLine>().Pun_BtnBlack_ResetColor();
                GetComponent<DrawLine>().currentSortOder = 0;
                GetComponent<DrawLine>().ButtonClearImage();
                if (inputAnswer != null)
                {
                    if (inputAnswer.gameObject.activeSelf)
                    {
                        inputAnswer.text = "";
                    }
                }
                GameManager.instance.panelAnswer.gameObject.SetActive(true);
                GameManager.instance.panelDraw.gameObject.SetActive(false);
                UIManager.instance.txtAnswer.gameObject.SetActive(false);
                UIManager.instance.txtGoiY.gameObject.SetActive(true);
            }
        }
    }




}
