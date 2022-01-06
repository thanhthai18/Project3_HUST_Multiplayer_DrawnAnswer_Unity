using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public PlayerUI[] playerContainers;
    public static UIManager instance;
    public Text txtTime;
    public RankUI[] playerContainersRank;
    public GameObject panelRankFrame;


    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        panelRankFrame.SetActive(false);
        InitializePlayerUI();
    }

    void InitializePlayerUI()
    {
        for (int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUI container = playerContainers[x];
            if (x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[x].NickName;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }


    public void SetHappy(int id)
    {
        PlayerUI container = playerContainers[id-1];
        container.happyIcon.gameObject.SetActive(true);
    }


    public void SetSad(int id)
    {
        PlayerUI container = playerContainers[id - 1];
        container.sadIcon.gameObject.SetActive(true);
        container.sadIcon.GetComponent<Image>().DOFade(0, 1).OnComplete(() =>
         {
             container.sadIcon.gameObject.SetActive(false);
         });
    }

    [PunRPC]
    public void SetDraw(int id)
    {
        PlayerUI container = playerContainers[id - 1];
        container.drawIcon.gameObject.SetActive(true);
    }
    [PunRPC]
    public void SetIconDefault()
    {
        for (int x = 0; x < playerContainers.Length; ++x)
        {
            PlayerUI container = playerContainers[x];
            container.happyIcon.gameObject.SetActive(false);
            container.sadIcon.gameObject.SetActive(false);
            container.drawIcon.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void SetTextTime(int time)
    {
        txtTime.text = time.ToString();
    }

    public void SetPanelRank()
    {
        panelRankFrame.SetActive(true);
        for (int x = 0; x < playerContainersRank.Length; ++x)
        {
            RankUI container = playerContainersRank[x];
            if (x < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.namePlayer.text = PhotonNetwork.PlayerList[x].NickName;
                container.txtScore.text = GameManager.instance.players[x].score.ToString();
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }


}

[System.Serializable]
public class PlayerUI
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Image happyIcon;
    public Image sadIcon;
    public Image drawIcon;
}

[System.Serializable]
public class RankUI
{
    public GameObject obj;
    public TextMeshProUGUI namePlayer;
    public TextMeshProUGUI txtScore;
}

