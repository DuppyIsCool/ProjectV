using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class ProjectVPortal : NetworkBehaviour
{
    [Scene, Tooltip("Which scene to send player from here")]
    public string destinationScene;

    [Tooltip("Where to spawn player in Destination Scene")]
    public Vector2 startPosition;

    // This is approximately the fade time
    WaitForSeconds waitForFade = new WaitForSeconds(2f);

    // Note that I have created layers called Player(8) and Portal(9) and set them
    // up in the Physics collision matrix so only Player collides with Portal.
    void OnTriggerEnter2D(Collider2D other)
    {
        // tag check in case you didn't set up the layers and matrix as noted above
        if (!other.CompareTag("Player")) return;

        //Debug.Log($"{System.DateTime.Now:HH:mm:ss:fff} Portal::OnTriggerEnter {gameObject.name} in {gameObject.scene.name}");

        // applies to host client on server and remote clients
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerController))
        {
            other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerController.enabled = false;
        }

        if (isServer)
            StartCoroutine(SendPlayerToNewScene(other.gameObject));
    }

    [ServerCallback]
    IEnumerator SendPlayerToNewScene(GameObject player)
    {
        if (player.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
        {
            NetworkConnectionToClient conn = identity.connectionToClient;
            if (conn == null) yield break;

            // Tell client to unload previous subscene. No custom handling for this.
            conn.Send(new SceneMessage { sceneName = gameObject.scene.path, sceneOperation = SceneOperation.UnloadAdditive, customHandling = true });

            yield return waitForFade;

            //Debug.Log($"SendPlayerToNewScene RemovePlayerForConnection {conn} netId:{conn.identity.netId}");
            NetworkServer.RemovePlayerForConnection(conn, false);

            // reposition player on server and client
            player.transform.position = startPosition;
            
            // Move player to new subscene.
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByPath(destinationScene));

            // Tell client to load the new subscene with custom handling (see NetworkManager::OnClientChangeScene).
            conn.Send(new SceneMessage { sceneName = destinationScene, sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            //Debug.Log($"SendPlayerToNewScene AddPlayerForConnection {conn} netId:{conn.identity.netId}");
            NetworkServer.AddPlayerForConnection(conn, player);

            // host client would have been disabled by OnTriggerEnter above
            if (NetworkClient.localPlayer != null && NetworkClient.localPlayer.TryGetComponent<PlayerMovement>(out PlayerMovement playerController))
                playerController.enabled = true;
        }
    }
}
