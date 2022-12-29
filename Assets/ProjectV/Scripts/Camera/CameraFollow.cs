using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Mirror;
public class CameraFollow : NetworkBehaviour
{
    CinemachineVirtualCamera mainCam;

    void Awake()
    {
        mainCam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineVirtualCamera>();
    }
    public override void OnStartLocalPlayer()
    {
        if (mainCam != null)
        {
            SceneManager.MoveGameObjectToScene(mainCam.gameObject, this.gameObject.scene);
            mainCam.Follow = this.transform;
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
            mainCam.Follow = null;
            SceneManager.MoveGameObjectToScene(mainCam.gameObject, SceneManager.GetActiveScene());
        }
    }

}
