using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Mirror;
public class CameraFollow : NetworkBehaviour
{
    Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }
    public override void OnStartLocalPlayer()
    {
        if (mainCam != null)
        {
            mainCam.GetComponent<CameraTarget>().target = this.transform;
        }
        else 
        {
            print("Error setting camera target for player");
        }

        base.OnStartLocalPlayer();
    }
    public override void OnStopLocalPlayer()
    {
        if (mainCam != null)
        {
            mainCam.GetComponent<CameraTarget>().target = null;
            SceneManager.MoveGameObjectToScene(mainCam.gameObject, SceneManager.GetActiveScene());
        }
    }

}
