using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    public int id;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;


    public float curHatTime;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameController.instance.players[id - 1] = this;

        if(id == 1)
        {
            GameController.instance.GiveHat(id, true);
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float y = Input.GetAxis("Vertical") * moveSpeed;
        rig.velocity = new Vector3(x, y);
    }

    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(curHatTime >= GameController.instance.timeToWin && !GameController.instance.gameEnded)
            {
                GameController.instance.gameEnded = true;
                GameController.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine)
        {
            Move();

            if (hatObject.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if(GameController.instance.GetPlayer(collision.gameObject).id == GameController.instance.playerWithHat)
            {
                if (GameController.instance.CanGetHat())
                {
                    GameController.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }       
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }
}
