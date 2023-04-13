using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using UnityEngine.SceneManagement;
public class PlayerUse : NetworkBehaviour
{
    [SyncVar]
    private double nextUseTime;
    [SerializeField]
    private float cooldownTime;

    [SerializeField]
    private Rigidbody2D rb;

    private Camera cam;

    Vector2 mousePosition;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        //Only take updates from local players
        if (!isLocalPlayer)
            return;

        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1"))
        {
            //If the mouse is not over UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Use(mousePosition);
            }
        }
    }


    //This is called on the client when they Use
    [ClientCallback]
    void Use(Vector2 direction)
    {
        if (CanUse())
        {
            InventoryItem equippedItem = this.GetComponent<Inventory>().equippedItem;
            //Request from the server to use the item
            CmdUse(direction);

            //Use the item on the client
            if(isClientOnly)
                DoUse(direction,equippedItem);
        }
    }


    //This is a call used on the client to check if the player can Use
    bool CanUse()
    {
        InventoryItem equippedItem = this.GetComponent<Inventory>().equippedItem;

        if (NetworkTime.time > nextUseTime && equippedItem.item != null && equippedItem.item.isUsable)
            return true;
        else
            return false;
    }

    //This is called on the server to determine if the server and other clients should register the Use
    [Command]
    void CmdUse(Vector2 direction)
    {
        //If the player can ACTUALLY use
        if (CanUse())
        {
            InventoryItem item = this.GetComponent<Inventory>().equippedItem;
            nextUseTime = NetworkTime.time + item.item.useCooldownTime;
            //Tell all clients to register the use
            RpcUse(direction,item);
            //Run the use on the servver
            DoUse(direction,item);
        }
    }

    //This is called on every client besides the owner to Use, as owner already processed Use
    [ClientRpc(includeOwner = false)]
    void RpcUse(Vector2 direction,InventoryItem item) 
    {
        if(isClientOnly)
            DoUse(direction,item);
    }

    
    //This is called by both server and clients to process the Use
    void DoUse(Vector2 direction, InventoryItem equippedItem) 
    {
        if (isServer) 
        {
            Vector2 tempdirection = direction;
            tempdirection -= this.GetComponent<Rigidbody2D>().position;
            tempdirection.Normalize();

            if (equippedItem.item.GetType() == typeof(BowItem))
            {
                BowItem bowItem = (BowItem)equippedItem.item;

                //Spawn for the server
                GameObject projectile = Instantiate(bowItem.serverPrefab, rb.position + tempdirection *1f, this.gameObject.transform.rotation);
                SceneManager.MoveGameObjectToScene(projectile, this.gameObject.scene);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>());
                projectile.GetComponent<ServerProjectile>().SetDamage(bowItem.damage);
                projectile.GetComponent<ServerProjectile>().ApplyForce(tempdirection, bowItem.speed);
            }

            else if (equippedItem.item.GetType() == typeof(SwordItem))
            {
                SwordItem sword = (SwordItem)equippedItem.item;

                LayerMask enemy = LayerMask.GetMask("Player");

                Collider2D[] attackCollisions = Physics2D.OverlapCircleAll(rb.position + tempdirection, 2);

                Debug.Log(attackCollisions.Length);

              foreach(Collider2D hit in attackCollisions)
                {
                    Debug.Log("Hello World");
                    if(hit.gameObject.GetComponent<Health>() != null) // Check for Enemy health
                    {
                        hit.gameObject.GetComponent<Health>().ApplyDamage(5); // Apply damage here
                    }
                }

                print("I am a sword");


            }

            else if (equippedItem.item.GetType() == typeof(Item))
            {
                print("I am a normal item");
            }

            
        }
        
        //Do visual effects only for clients
        if (isClient) 
        {
            direction -= this.GetComponent<Rigidbody2D>().position;
            direction.Normalize();

            if (equippedItem.item.GetType() == typeof(BowItem))
            {
                BowItem bowItem = (BowItem)equippedItem.item;
                //Spawn for the server

                GameObject projectile = Instantiate(bowItem.clientPrefab, rb.position + direction * 1f, this.gameObject.transform.rotation);
                SceneManager.MoveGameObjectToScene(projectile, this.gameObject.scene);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>()); 
                projectile.GetComponent<ClientProjectile>().ApplyForce(direction, bowItem.speed);
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

    private void OnDrawGizmos()
    {
        Vector2 tempvec = (mousePosition - this.GetComponent<Rigidbody2D>().position);
        Vector3 myVec = new Vector3(tempvec.x, tempvec.y, 1);
        myVec.Normalize();
        
        Gizmos.DrawWireSphere(myVec + (Vector3)(rb.position), 2);
    }

}