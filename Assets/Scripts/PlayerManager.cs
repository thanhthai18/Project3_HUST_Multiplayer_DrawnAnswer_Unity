using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using unitycoder_MobilePaint;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public int id;

    [Header("Info")]
    public bool isMyTurn;
    public int score;
    public string myAnswer;



    [Header("Components")]
    public Player photonPlayer;
    public MobilePaint drawingPlanceCanvas;


    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        drawingPlanceCanvas = GameObject.Find("DrawingPlaneCanvas").GetComponent<MobilePaint>();
        GameManager.instance.players[id - 1] = this;

        if (id == 1)
        {
            isMyTurn = true;
            GameManager.instance.GiveTurn(id, true);
        }
        else
        {
            isMyTurn = false;
        }
    }

    public void OnClickCheck(TMP_InputField answerInput)
    {

        if (photonView.IsMine)
        {
            myAnswer = answerInput.text;


            if (myAnswer != GameManager.instance.currentGameAnswer)
            {
                UIManager.instance.SetSad(id);
                //photonView.RPC("SetSad", RpcTarget.All, id);
            }
            else
            {
                UIManager.instance.SetHappy(id);
                //photonView.RPC("SetHappy", RpcTarget.All, id);
            }
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
                drawingPlanceCanvas.ClearImage();
                drawingPlanceCanvas.enabled = true;
            }
            else
            {
                GameManager.instance.panelAnswer.gameObject.SetActive(true);
                GameManager.instance.panelDraw.gameObject.SetActive(false);
                drawingPlanceCanvas.ClearImage();
                drawingPlanceCanvas.enabled = false;
            }
        }

    }

}
