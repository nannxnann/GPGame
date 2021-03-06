﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using System.Collections;

using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;
    public static GameManager Instance;
    #endregion
    public bool gameStart = false;
    private bool dropping = false;
    [Tooltip("UI Text to display Player's ranking")]
    [SerializeField]
    private Text rankingText;
    [Tooltip("start Button for the UI")]
    [SerializeField]
    private GameObject startButton;
    public float gravity = 0.5F;
    private GameObject[] Players;
    private List<Text> Rankings = new List<Text>();
    private List<GameObject> playerList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

        StartGenerator();
        startButton.SetActive(false);

        Instance = this;

        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            if (SimpleTeleport_NetworkVersion.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                playerList.Add(PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector2(Random.Range(-6.0f, 6.0f), 1f), Quaternion.identity, 0));
                

            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameStart && dropping == false)
        {

            dropping = true;
            StartGenerator();
            startButton.SetActive(false);

        }    
    }

    private IEnumerator EnemyGenerator()
    {
        while (true)
        {
            Vector2 randPosition = new Vector2(Random.Range(-8.0f, 8.0f), 6.0f);
            PhotonNetwork.Instantiate("Drop", randPosition, Quaternion.identity);
            yield return new WaitForSeconds(Mathf.Lerp(0.1f, 1.0f, Random.value));
        }
    }

    public void StartGenerator()
    {
        StartCoroutine(EnemyGenerator());
    }

    public void StartGame()
    {
        gameStart = true;
        Players = GameObject.FindGameObjectsWithTag("player");
        // now all your game objects are in GOs,
        // all that remains is to getComponent of each and every script and you are good to go.
        // to disable a components
        for (int i = 0; i < Players.Length; i++)
        {
            // to access component - GOs[i].GetComponent.<BoxCollider>()
            // but I do it everything in 1 line.
            Players[i].GetComponent<Rigidbody2D>().gravityScale = gravity;
            Rankings.Add(Instantiate(rankingText));
            Rankings[i].transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }
    }
    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Launcher");
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }

    #endregion


    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion
    
    #region Private Methods


    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        //PhotonNetwork.LoadLevel("RoomForNetwork");
    }


    #endregion





}
