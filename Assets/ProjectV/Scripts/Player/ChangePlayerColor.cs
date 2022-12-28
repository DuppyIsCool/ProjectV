using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class ChangePlayerColor : NetworkBehaviour
{ 
    [SyncVar(hook = nameof(SetColor))]public Color32 color = Color.black;

    /// Unity clones the material when GetComponent<Renderer>().material is called
    // Cache it here and destroy it in OnDestroy to prevent a memory lea

    void SetColor(Color32 _, Color32 newColor)
    {

        this.GetComponent<SpriteRenderer>().color = newColor;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // This script is on players that are respawned repeatedly
        // so once the color has been set, don't change it.
        if (color == Color.black)
            color = Random.ColorHSV(0f, 1f, 0.1f, 1f, 0.1f, 1f);
    }
}
