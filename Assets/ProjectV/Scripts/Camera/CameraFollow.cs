using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;
public class CameraFollow : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private CinemachineVirtualCamera virtualcam;
    void Start()
    {
        if (!isLocalPlayer)
            return;

        virtualcam = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineVirtualCamera>();
        if (virtualcam == null)
        {
            print("Player encountered error when finding Camera");
        }
        else 
        {
            virtualcam.Follow = this.transform;
            virtualcam.LookAt = this.transform;
        }
    }
}
