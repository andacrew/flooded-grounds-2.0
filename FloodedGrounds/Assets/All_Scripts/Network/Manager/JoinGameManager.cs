using UnityEngine;
using UnityEngine.UI;

using Michsky.UI.FieldCompleteMainMenu;

public class JoinGameManager : MonoBehaviour
{
    private GameObject mainObject;
    private MessageQueue msgQueue;
    private ConnectionManager cManager;


    void Awake()
    {
        mainObject = GameObject.Find("MainObject");
        cManager = mainObject.GetComponent<ConnectionManager>();
        msgQueue = mainObject.GetComponent<MessageQueue>();
        msgQueue.AddCallback(Constants.SMSG_JOINGAME, ResponseJoinGame);
    }

    private void Start()
    {
        if (cManager)
        {
            cManager.setupSocket();
        }
    }

    public void JoinGame()
    {
        //RequestJoinGame request = new RequestJoinGame();
        //request.send();
        //cManager.send(request);

        Main.setCharacter("Max");
        Main.gameState = Main.GameState.GAME;
    }

    public void ResponseJoinGame(ExtendedEventArgs eventArgs)
    {         
        ResponseJoinGameEventArgs args = eventArgs as ResponseJoinGameEventArgs;
        if (args.character != "")
        {
            Main.setCharacter(args.character);
            Main.gameState = Main.GameState.GAME;            
        }
        
        
    }
}