using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject PlayerSpawnPoint;
    public GameObject PlayerPrefab;

    protected GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPlayerManager()
    {
        GameObject obj = GameObject.Instantiate(PlayerPrefab);
        obj.transform.SetParent(gameObject.transform);
        obj.transform.position = PlayerSpawnPoint.transform.position;
        obj.GetComponent<PlayerScript>().PlayerInit();
        Player = obj;
    }

    public PlayerScript GetPlayerInfo()
    {
        return Player.GetComponent<PlayerScript>();
    }
}
