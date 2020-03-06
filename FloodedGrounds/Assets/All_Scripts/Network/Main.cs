using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;


public class Main : MonoBehaviour
{
    private SceneController sceneController;

    public enum GameState { INITIAL, LOGIN, MAIN_MENU, LOBBY, GAME }
    public static GameState gameState { get; set; }
    private static GameState lastState = GameState.INITIAL;
    public static User user { get; set; }

    private Dictionary<string, Scene> sceneMap;

    //Determine what state the game is in
    private static string character = null;
    private static ConnectionManager cManager;

    //Blood effects
    public ParticleSystem monsterBlood;
    public ParticleSystem humanBlood;

    public static void setCharacter(string _character)
    {
        character = _character;
    }

    private void setCharacterValues()
    {
        string movementScript = Constants.movementScripts[character];

        GameObject player = GameObject.FindGameObjectWithTag(character);

        //Disble the network movement script
        player.GetComponent<NetworkMovement>().enabled = false;

        //Set the movement script for the weapon select
        if (character != Constants.MONSTER)
            WeaponSelectionAnim.player = GameObject.Find(character).GetComponent<MaxMovement>();

        //Find the first person camera for this character
        GameObject myCamera = null;

        foreach (Transform t in player.transform.GetComponentsInChildren<Transform>(true))
            if (t.gameObject.name == "FPS Camera")
                myCamera = t.gameObject;

        //Enable the first person camera
        myCamera.SetActive(true);

        //Make all of the player tags face the camera
        //Loop through all of the characters
        foreach (string name in Constants.IDtoCharacter.Values)
            //Skip the character being played
            if (name != Main.getCharacter())
                //Loop through all of the children of the object
                foreach (Transform t in GameObject.FindGameObjectWithTag(name).transform.GetComponentsInChildren<Transform>(true))
                    //If the child is the player status, enable the billboard script and set it's camera to this camera
                    if (t.gameObject.name == "PlayerStatus")
                    {
                        t.gameObject.SetActive(true);
                        t.gameObject.GetComponent<Billboard>().cam = myCamera.GetComponent<Camera>();
                    }

        //Enable the movement script and enable gravity for this player
        switch (movementScript)
        {
            case "MonsterMovement":
                player.GetComponent<MonsterMovement2>().enabled = true;
                player.GetComponent<MonsterMovement2>().gravityEnabled = true;
                break;
            case "PlayerMovement2":
                player.GetComponent<MaxMovement>().enabled = true;
                player.GetComponent<MaxMovement>().gravityEnabled = true;
                break;
            case "MaxMovement":
                player.GetComponent<MaxMovement>().enabled = true;
                player.GetComponent<MaxMovement>().gravityEnabled = true;
                break;
        }

        //Set the player object in the RequestPushUpdate script
        ((RequestPushUpdate)NetworkRequestTable.get(Constants.CMSG_PUSHUPDATE)).setPlayer(character);
    }

    public static string getCharacter()
    {
        return character;
    }

    public static ConnectionManager GetConnectionManager()
    {
        return cManager;
    }

    public static bool joinLobby(Lobby lobby)
    {
        // make sure the player can actually join a lobby
        if (gameState != GameState.MAIN_MENU) return false;
        return true;
    }

    void Awake()
    {
        sceneController = FindObjectOfType<SceneController>();
        if (!sceneController) throw new UnityException("Scene Controller could not be found");

        gameState = GameState.LOGIN;

        gameObject.AddComponent<MessageQueue>();
        gameObject.AddComponent<ConnectionManager>();

        NetworkRequestTable.init();
        NetworkResponseTable.init();
    }

    void Start()
    {
        cManager = gameObject.GetComponent<ConnectionManager>();

        if (cManager)
        {
            cManager.setupSocket();
            StartCoroutine(UpdateLoop(1f / Constants.updatesPerSecond));
        }
    }

    public IEnumerator UpdateLoop(float updateDelay)
    {
        yield return new WaitForSeconds(updateDelay);

        if (cManager)
        {
            switch (gameState)
            {

                case GameState.LOBBY:
                    // will have specific request to keep it live at this point
                    // but doesn't yet...
                    // todo: add getLobbies code when lobbies are implemented
                    // break;
                case GameState.LOGIN:
                case GameState.MAIN_MENU:
                    if (lastState == GameState.GAME) sceneController.FadeAndLoadScene("Desktop"); // reload menu after a game;

                    RequestKeepAlive keepAlive = new RequestKeepAlive();
                    keepAlive.send();
                    cManager.send(keepAlive);
                    break;

                case GameState.GAME:
                    if (lastState == GameState.MAIN_MENU) {
                        sceneController.FadeAndLoadScene("Scene_A");

                        while (SceneManager.GetActiveScene().name != "Scene_A") yield return null;

                        Constants.loadSceneAConstants();

                        setCharacterValues();

                        Debug.Log("Joined Game");
                    }
                    
                    //Create the two request objects that will be sent to the server
                    RequestHeartbeat heartbeat = (RequestHeartbeat)NetworkRequestTable.get(Constants.CMSG_HEARTBEAT);
                    RequestPushUpdate pushUpdate = (RequestPushUpdate)NetworkRequestTable.get(Constants.CMSG_PUSHUPDATE);

                    //Create the messages to be sent
                    heartbeat.send();
                    pushUpdate.send();

                    //Send the messages
                    cManager.send(heartbeat);
                    cManager.send(pushUpdate);

                    break;
                default:
                    break;
            }

            lastState = gameState;
        }
        StartCoroutine(UpdateLoop(updateDelay));
    }
}