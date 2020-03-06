using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Michsky.UI.FieldCompleteMainMenu;

public class LoginUserManager : MonoBehaviour
{
    [Header("RESOURCES")]
    public SwitchToMainPanels switchPanelMain;
    public UIElementSound soundScript;
    public Animator wrongAnimator;
    public Text usernameText;
    public Text passwordText;

    private string username = "";
    private string password = "";
    private GameObject mainObject;
    private MessageQueue msgQueue;
    private ConnectionManager cManager;

    void Awake()
    {
        mainObject = GameObject.Find("MainObject");        
        cManager = mainObject.GetComponent<ConnectionManager>();
        msgQueue = mainObject.GetComponent<MessageQueue>();
        msgQueue.AddCallback(Constants.SMSG_LOGIN, ResponseLogin);
    }

    private void Start()
    {
        if (cManager)
        {
            cManager.setupSocket();
        }
    }

    public void Login()
    {
        //username = usernameText.text.Trim();
        //password = passwordText.text.Trim();
        //RequestLogin request = new RequestLogin();
        //request.send(username, password);
        //cManager.send(request);

        User user = new User();
        user.ID = 12345;
        user.userName = "Guest User";
        user.email = "test@gmail.com";
        user.played = 1;
        user.won = 1;
        user.lost = 1;
        Main.user = user;
        Main.gameState = Main.GameState.MAIN_MENU;
        switchPanelMain.Animate();
    }

    public void ResponseLogin(ExtendedEventArgs eventArgs)
    {
        ResponseLoginEventArgs args = eventArgs as ResponseLoginEventArgs;
        if (args.status == 0)
        {
            User user = new User();
            user.ID = args.user_id;
            user.userName = args.username;
            user.email = args.email;
            user.played = args.gamesPlayed;
            user.won = args.gamesWon;
            user.lost = args.gamesLost;
            Main.user = user;
            Main.gameState = Main.GameState.MAIN_MENU;
            switchPanelMain.Animate();        }
        else
        {
            wrongAnimator.Play("Notification In");
            soundScript.Notification();
        }
    }
}
