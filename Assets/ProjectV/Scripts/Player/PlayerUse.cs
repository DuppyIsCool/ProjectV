using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
public class PlayerUse : NetworkBehaviour
{
    //[SyncVar]
    private float nextUseTime;
    [SerializeField]
    private float cooldownTime;

    [SerializeField]
    private Rigidbody2D rb;

    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1"))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Use(mousePosition);
            }
        }
    }

    //This is a call used to check if the player can Use
    bool CanUse()
    {
        InventoryItem equippedItem = this.GetComponent<Inventory>().equippedItem;

        if (Time.time > nextUseTime && equippedItem.item != null && equippedItem.item.isUsable)
            return true;
        else
            return false;
    }

    //This is called on the client when they Use
    [ClientCallback]
    void Use(Vector2 direction)
    {
        if (CanUse())
        {
            CmdUse(direction);
            DoUse(direction);
        }
    }

    //This is called on the server to determine if the server and other clients should register the Use
    [Command]
    void CmdUse(Vector2 direction)
    {
        if (CanUse())
        {
            RpcUse(direction);
            DoUse(direction);
        }
    }

    //This is called on every client besides the owner to Use, as owner already processed Use
    [ClientRpc(includeOwner = false)]
    void RpcUse(Vector2 direction) 
    {
        DoUse(direction);
    }

    //This is called by both server and clients to process the Use
    void DoUse(Vector2 direction) 
    {
        if (isServerOnly) 
        {
            InventoryItem equippedItem = this.GetComponent<Inventory>().equippedItem;

            direction -= this.GetComponent<Rigidbody2D>().position;
            direction.Normalize();

            if (equippedItem.item.GetType() == typeof(BowItem))
            {
                BowItem bowItem = (BowItem)equippedItem.item;

                //Spawn for the server
                GameObject projectile = Instantiate(bowItem.spawnedPrefab, rb.position + direction * 1f, this.gameObject.transform.rotation);
                projectile.GetComponent<Projectile>().damage = bowItem.damage;
                projectile.GetComponent<Projectile>().ApplyForce(direction,bowItem.speed);
            }

            else if (equippedItem.item.GetType() == typeof(SwordItem))
            {
                print("I am a sword");
            }

            else if (equippedItem.item.GetType() == typeof(Item))
            {
                print("I am a normal item");
            }

            nextUseTime = Time.time + equippedItem.item.useCooldownTime;
        }
        
        //Do visual effects
        if (isClient) 
        {
            InventoryItem equippedItem = this.GetComponent<Inventory>().equippedItem;

            direction -= this.GetComponent<Rigidbody2D>().position;
            direction.Normalize();

            if (equippedItem.item.GetType() == typeof(BowItem))
            {
                BowItem bowItem = (BowItem)equippedItem.item;
                //Spawn for the server

                GameObject projectile = Instantiate(bowItem.spawnedPrefab, rb.position + direction * 1f, this.gameObject.transform.rotation);
                projectile.GetComponent<Projectile>().damage = bowItem.damage;
                projectile.GetComponent<Projectile>().ApplyForce(direction, bowItem.speed);
            }

            else if (equippedItem.item.GetType() == typeof(SwordItem))
            {
                print("I am a sword");
            }

            else if (equippedItem.item.GetType() == typeof(Item))
            {
                print("I am a normal item");
            }
        }
    }
}
