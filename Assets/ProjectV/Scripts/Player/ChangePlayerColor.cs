using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
public class ChangePlayerColor : NetworkBehaviour
{ 
    [SyncVar(hook = "ChangeColor")] Color color;

    //This is called when the player object is created
    public override void OnStartLocalPlayer() 
    {
        //If we are the local player
        if (isLocalPlayer)
        {
            //Calculate a new color value
            Color newcolor = new Color(Random.value, Random.value, Random.value);

            //Ask our color value to be changed by the server
            RequestColorChange(newcolor);

            //To prevent delays and a noticable color swap, update the color on the client's side immedietly
            this.GetComponent<SpriteRenderer>().color = newcolor;
        }
        base.OnStartLocalPlayer();
    }

    //This is called on the server by a client
    [Command]
    private void RequestColorChange(Color newcolor)
    {
        //Set the inputed color value (from client) into the SyncVar (which syncs the value between all clients)
        color = newcolor;

        //Update the color of the value on the server side
        this.GetComponent<SpriteRenderer>().color = newcolor;
    }

    //This is called on clients when the SyncVar 'color' is changed
    [Client]
    private void ChangeColor(Color oldcolor, Color newcolor) 
    {

        //Update the color of this sprite when we receive color updates
        this.GetComponent<SpriteRenderer>().color = newcolor;
    }
}
