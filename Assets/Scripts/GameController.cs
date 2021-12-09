using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance);
    }

    private void Start()
    {
        SetSizeCamera();
    }

    void SetSizeCamera()
    {
        float f1, f2;
        f1 = 9.0f / 16;
        f2 = Screen.width * 1.0f / Screen.height;
        mainCamera.orthographicSize *= f1 / f2;
    }
}
