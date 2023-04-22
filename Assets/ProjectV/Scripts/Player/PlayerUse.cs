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
    public LayerMask enemyLayer;
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
            if (!EventSystem.current.IsPointerOverGameObject() && CanUse())
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
        }
        
        //Do visual effects only for clients
        if (isClient) 
        {
            direction -= this.GetComponent<Rigidbody2D>().position;
            direction.Normalize();

            if (equippedItem.item.GetType() == typeof(BowItem))
            {
                BowItem bowItem = (BowItem)equippedItem.item;

                //Spawn the client projectile
                GameObject projectile = Instantiate(bowItem.clientPrefab, rb.position + direction * 1f, this.gameObject.transform.rotation);
                SceneManager.MoveGameObjectToScene(projectile, this.gameObject.scene);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), projectile.GetComponent<Collider2D>()); 
                projectile.GetComponent<ClientProjectile>().ApplyForce(direction, bowItem.speed);
            }

            else if (equippedItem.item.GetType() == typeof(SwordItem))
            {
                SwordItem sword = (SwordItem)equippedItem.item;

                //Create the attack box
                Vector2 boxSize = new Vector2(sword.range, sword.width);
                float offset = 1.2f;
                Vector2 boxCenter = (Vector2)transform.position + direction * (sword.range * 0.5f + offset);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle);

                // Visualize the attack in the editor (for debugging purposes).
                #if UNITY_EDITOR
                DebugDrawAttackBox(boxCenter, boxSize, angle);
                #endif
                foreach (Collider2D hitCollider in hitColliders)
                {
                    if (hitCollider.GetComponent<Health>() != null)
                    {
                        HitEnemyCMD(hitCollider.gameObject);
                    }
                }
            }

            else if (equippedItem.item.GetType() == typeof(Item))
            {
                print("I am a normal item");
            }
        }
    }
    #if UNITY_EDITOR
    private void DebugDrawAttackBox(Vector2 center, Vector2 size, float angle)
    {

        Vector2[] corners = new Vector2[4];
        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;

        corners[0] = new Vector2(-halfWidth, -halfHeight);
        corners[1] = new Vector2(halfWidth, -halfHeight);
        corners[2] = new Vector2(halfWidth, halfHeight);
        corners[3] = new Vector2(-halfWidth, halfHeight);

        float rotation = angle * Mathf.Deg2Rad;
        for (int i = 0; i < 4; i++)
        {
            Vector2 rotatedCorner = new Vector2(
                corners[i].x * Mathf.Cos(rotation) - corners[i].y * Mathf.Sin(rotation),
                corners[i].x * Mathf.Sin(rotation) + corners[i].y * Mathf.Cos(rotation)
            );
            corners[i] = center + rotatedCorner;
        }
        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(corners[i], corners[(i + 1) % 4], Color.red, 3.0f);
        }
    }
    #endif

    [Command]
    private void HitEnemyCMD(GameObject enemy) 
    {
        //If the player has a sword equipped
        if (GetComponent<Inventory>().equippedItem.item.GetType() == typeof(SwordItem)) 
        {
            //Get the sword
            SwordItem sword = (SwordItem)GetComponent<Inventory>().equippedItem.item;

            //If the sword range or width is greater than the distance between us and the enemy
            if (Vector2.Distance(transform.position, enemy.transform.position) <= sword.range || Vector2.Distance(transform.position, enemy.transform.position) <= sword.width) 
            {
                //If the enemy has a health component
                if (enemy.GetComponent<Health>() != null) 
                {
                    //Apply damage to the enemy
                    enemy.GetComponent<Health>().ApplyDamage(sword.damage);
                }
            }
        }
    }
}