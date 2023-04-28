using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Mirror;
using kcp2k;
public class PlayerJoinTest
{
    private NetworkManagerTestWrapper networkManager;
    private GameObject playerPrefab;

    [SetUp]
    public void Setup()
    {
        networkManager = new GameObject().AddComponent<NetworkManagerTestWrapper>();
        networkManager.hideFlags = HideFlags.HideInHierarchy;

        // Replace "YourPlayerPrefabPath" with the path to your player prefab in the Resources folder
        playerPrefab = Resources.Load<GameObject>("Player");
        networkManager.playerPrefab = playerPrefab;

        GameObject transportObj = new GameObject("TestTransport");
        transportObj.hideFlags = HideFlags.HideInHierarchy;
        KcpTransport transport = transportObj.AddComponent<KcpTransport>();
        networkManager.transport = transport;
        Transport.active = transport;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(networkManager.gameObject);
        Object.DestroyImmediate(networkManager.transport.gameObject);
    }

    [UnityTest]
    public IEnumerator TestPlayerObjectCreation()
    {
        // Start server
        networkManager.StartHost();

        yield return new WaitForSeconds(5f);

        // Check if the player object is created
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(playerObject, "Player object was not created.");
    }

    [UnityTest]
    public IEnumerator TestPlayerHealth() 
    {
        if (networkManager.isNetworkActive)
            networkManager.StopHost();

        networkManager.StartHost();

        yield return new WaitForSeconds(5f);

        // Check if the player object is created
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(playerObject, "Player object was not created.");

        int health = playerObject.GetComponent<Health>().currentHealth;
        health -= 10;
        playerObject.GetComponent<Health>().ApplyDamage(10);
        Assert.AreEqual(health, playerObject.GetComponent<Health>().currentHealth);
        health += 10;
        playerObject.GetComponent<Health>().Heal(10);
        Assert.AreEqual(health, playerObject.GetComponent<Health>().currentHealth);
    }
}

public class NetworkManagerTestWrapper : NetworkManager
{
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("Host started.");
    }
}
