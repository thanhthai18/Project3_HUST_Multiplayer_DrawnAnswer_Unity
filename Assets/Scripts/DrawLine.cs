using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DrawLine : MonoBehaviourPunCallbacks
{
    public GameObject linePrefab;
    public GameObject currentLine;

    public LineRenderer lineRender;
    public List<Vector2> fingerPositions;
    public Camera mainCamera;
    public Vector2 mousePos;
    public List<GameObject> listForClear = new List<GameObject>();
    public EdgeCollider2D edgeCollider;
    public Button btnClear, btnBlack, btnGreen, btnRed, btnYellow, btnBlue, btnErase;
    public RaycastHit2D[] hit;
    public Color currentColor;
    public int currentSortOder;


    private void Start()
    {
        mainCamera = GameManager.instance.mainCamera;
        btnClear = UIManager.instance.btnClear;
        btnBlack = UIManager.instance.btnBlack;
        btnGreen = UIManager.instance.btnGreen;
        btnRed = UIManager.instance.btnRed;
        btnYellow = UIManager.instance.btnYellow;
        btnBlue = UIManager.instance.btnBlue;
        btnErase = UIManager.instance.btnErase;
        btnClear.onClick.AddListener(ButtonClearImage);
        btnBlack.onClick.AddListener(Pun_BtnBlack_ResetColor);
        btnGreen.onClick.AddListener(Pun_BtnGreen);
        btnRed.onClick.AddListener(Pun_BtnRed);
        btnYellow.onClick.AddListener(Pun_BtnYellow);
        btnBlue.onClick.AddListener(Pun_BtnBlue);
        btnErase.onClick.AddListener(Pun_BtnErase);
        currentColor = Color.black;
        currentSortOder = 0;
    }

    [PunRPC]
    public void CreateLine(Vector2 mousePos)
    {
        currentLine = Instantiate(linePrefab, mousePos, Quaternion.identity);
        listForClear.Add(currentLine);
        lineRender = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        fingerPositions.Clear();
        fingerPositions.Add(mousePos);
        fingerPositions.Add(mousePos);
        lineRender.startColor = currentColor;
        lineRender.endColor = currentColor;
        currentSortOder++;
        lineRender.sortingOrder = currentSortOder;
        lineRender.SetPosition(0, mousePos);
        lineRender.SetPosition(1, mousePos);
    }

    

    [PunRPC]
    public void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        lineRender.positionCount++;
        lineRender.SetPosition(lineRender.positionCount - 1, newFingerPos);
        edgeCollider.points = fingerPositions.ToArray();
    }

    [PunRPC]
    public void SetMousePos()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector2(Mathf.Clamp(mousePos.x, -GameManager.instance.VectoClamMouse.x + 0.1f, GameManager.instance.VectoClamMouse.x - 0.1f), Mathf.Clamp(mousePos.y, -GameManager.instance.VectoClamMouse.y + 0.1f, GameManager.instance.VectoClamMouse.y - 0.3f));
    }

    [PunRPC]
    public void ClearImage()
    {
        fingerPositions.Clear();
        if (listForClear.Count > 0)
        {
            listForClear.ForEach(s => Destroy(s));
            listForClear.Clear();
        }
    }
    public void ButtonClearImage()
    {
        photonView.RPC("ClearImage", RpcTarget.All);
    }


    public void Pun_BtnBlack_ResetColor()
    {
        photonView.RPC("BtnBlack_ResetColor", RpcTarget.All);
    }
    [PunRPC]
    public void BtnBlack_ResetColor()
    {
        currentColor = Color.black;
    }

    public void Pun_BtnGreen()
    {
        photonView.RPC("BtnGreen", RpcTarget.All);
    }
    [PunRPC]
    public void BtnGreen()
    {
        currentColor = Color.green;
    }

    public void Pun_BtnRed()
    {
        photonView.RPC("BtnRed", RpcTarget.All);
    }
    [PunRPC]
    public void BtnRed()
    {
        currentColor = Color.red;
    }

    public void Pun_BtnYellow()
    {
        photonView.RPC("BtnYellow", RpcTarget.All);
    }
    [PunRPC]
    public void BtnYellow()
    {
        currentColor = Color.yellow;
    }

    public void Pun_BtnBlue()
    {
        photonView.RPC("BtnBlue", RpcTarget.All);
    }
    [PunRPC]
    public void BtnBlue()
    {
        currentColor = new Color(0,1,1,1);
    }

    public void Pun_BtnErase()
    {
        photonView.RPC("BtnErase", RpcTarget.All);
    }
    [PunRPC]
    public void BtnErase()
    {
        currentColor = Color.white;
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            if (GetComponentInParent<PlayerManager>().isMyTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    photonView.RPC("SetMousePos", RpcTarget.All);
                    if (mousePos.x <= GameManager.instance.VectoClamMouse.x - 0.1f && mousePos.x >= -GameManager.instance.VectoClamMouse.x + 0.1f && mousePos.y <= GameManager.instance.VectoClamMouse.y - 0.3f && mousePos.y >= -GameManager.instance.VectoClamMouse.y + 0.1f)
                    {
                        photonView.RPC("CreateLine", RpcTarget.All, mousePos);
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    photonView.RPC("SetMousePos", RpcTarget.All);
                    if (mousePos.x <= GameManager.instance.VectoClamMouse.x - 0.1f && mousePos.x >= -GameManager.instance.VectoClamMouse.x + 0.1f && mousePos.y <= GameManager.instance.VectoClamMouse.y - 0.3f && mousePos.y >= -GameManager.instance.VectoClamMouse.y + 0.1f)
                    {
                        if (Vector2.Distance(mousePos, fingerPositions[fingerPositions.Count - 1]) > 0.1f)
                        {
                            photonView.RPC("UpdateLine", RpcTarget.All, mousePos);
                        }
                    }
                }
            }
        }
    }
}

