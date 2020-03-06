using System.Collections.Generic;
using UnityEngine;

public class Constants
{

    //Constants
    public static readonly string CLIENT_VERSION = "1.00";
    public static readonly string REMOTE_HOST = "13.52.133.88";
    //public static readonly string REMOTE_HOST = "localhost";
    public static readonly int REMOTE_PORT = 9252;
    public static readonly int updatesPerSecond = 30;
    public static readonly int maxUpdateNumber = 10000;
    public static readonly int maxUpdateDistance = 1000;

    //Net code
    //Request: 1xx
    //Response: 2xx

    //General APIs:    x0x
    public static readonly short CMSG_HEARTBEAT = 101;
    public static readonly short SMSG_HEARTBEAT = 201;
    public static readonly short CMSG_PUSHUPDATE = 102;
    public static readonly short CMSG_KEEPALIVE = 103;

    //Authentication:  x1x
    public static readonly short CMSG_REGISTER = 111;
    public static readonly short SMSG_REGISTER = 211;
    public static readonly short CMSG_LOGIN = 112;
    public static readonly short SMSG_LOGIN = 212;

    //Lobby APIs:      x2x
    public static readonly short CMSG_GETLOBBIES = 121;
    public static readonly short SMSG_GETLOBBIES = 221;
    public static readonly short CMSG_CREATELOBBY = 122;
    public static readonly short SMSG_CREATELOBBY = 222;
    public static readonly short CMSG_JOINLOBBY = 123;
    public static readonly short SMSG_JOINLOBBY = 223;
    public static readonly short CMSG_STARTGAME = 124;
    public static readonly short SMSG_STARTGAME = 224;
    public static readonly short CMSG_JOINGAME = 125;
    public static readonly short SMSG_JOINGAME = 225;



    //Inventory Items

    //Guns:        1xx
    //Grenades:    2xx



    //Actions
    //Pickups:  x4x
    public static readonly short CMSG_PICKUP = 140;
    public static readonly short SMSG_PICKUP = 240;

    //Hit: x5x
    public static readonly short CMSG_HIT = 150;
    public static readonly short SMSG_HIT = 250;

    //Other
    public static readonly string IMAGE_RESOURCES_PATH = "Images/";
    public static readonly string PREFAB_RESOURCES_PATH = "Prefabs/";
    public static readonly string TEXTURE_RESOURCES_PATH = "Textures/";

    //GUI Window IDs
    public enum GUI_ID
    {
        Login
    };

    public static int USER_ID = -1;



    //Character constants
    public static string MONSTER = "Bog_lord";
    public static string GIRL = "Izzy";
    public static string GUY1 = "Max";
    public static string GUY2 = "Winston";

    //Dictionary to map ids to the characters
    public static Dictionary<int, string> IDtoCharacter;

    //Dictionary to map characters to the ids
    public static Dictionary<string, int> CharacterToID;

    //Animation Parameters
    public static readonly string[] monsterAnimParams = { "isJumping", "isWalking", "isDead", "isAttacking", "isHit", "isShouting" };
    public static readonly string[] humanAnimParams = { "isJumping", "isWalking", "isShooting", "isRunning", "isForward", "isBackward", "isLeft", "isRight" };

    //Dictionary to map characters to animation parameters
    public static Dictionary<string, string[]> characterAnimations;

    //Dictionary to map characters to movement scripts
    public static Dictionary<string, string> movementScripts;

    //A struct to store the components attatched to a character
    public struct characterComponents
    {
        public GameObject player;
        public NetworkMovement networkMovement;
        public Animator animController;

        public characterComponents(GameObject player, NetworkMovement networkMovement, Animator animController)
        {
            this.player = player;
            this.networkMovement = networkMovement;
            this.animController = animController;
        }
    }

    //A table to map characters to their components
    public static Dictionary<string, characterComponents> components;


    //Static constructor to populate the dictionarys
    static Constants()
    {
        IDtoCharacter = new Dictionary<int, string>();
        IDtoCharacter.Add(0, MONSTER);
        IDtoCharacter.Add(1, GIRL);
        IDtoCharacter.Add(2, GUY1);
        IDtoCharacter.Add(3, GUY2);

        CharacterToID = new Dictionary<string, int>();
        CharacterToID.Add(MONSTER, 0);
        CharacterToID.Add(GIRL, 1);
        CharacterToID.Add(GUY1, 2);
        CharacterToID.Add(GUY2, 3);

        characterAnimations = new Dictionary<string, string[]>();
        characterAnimations.Add(MONSTER, monsterAnimParams);
        characterAnimations.Add(GIRL, humanAnimParams);
        characterAnimations.Add(GUY1, humanAnimParams);
        characterAnimations.Add(GUY2, humanAnimParams);

        movementScripts = new Dictionary<string, string>();
        movementScripts.Add(MONSTER, "MonsterMovement");
        movementScripts.Add(GIRL, "PlayerMovement2");
        movementScripts.Add(GUY1, "MaxMovement");
        movementScripts.Add(GUY2, "MaxMovement");
    }

    // static method to populate scene specific dictionary
    public static void loadSceneAConstants()
    {


        components = new Dictionary<string, characterComponents>();
        GameObject monster = GameObject.FindWithTag(MONSTER);
        components.Add(MONSTER, new characterComponents(monster, monster.GetComponent<NetworkMovement>(), monster.GetComponent<Animator>()));
        GameObject girl = GameObject.FindWithTag(GIRL);
        components.Add(GIRL, new characterComponents(girl, girl.GetComponent<NetworkMovement>(), girl.GetComponent<Animator>()));
        GameObject guy1 = GameObject.FindWithTag(GUY1);
        components.Add(GUY1, new characterComponents(guy1, guy1.GetComponent<NetworkMovement>(), guy1.GetComponent<Animator>()));
        GameObject guy2 = GameObject.FindWithTag(GUY2);
        components.Add(GUY2, new characterComponents(guy2, guy2.GetComponent<NetworkMovement>(), guy2.GetComponent<Animator>()));
    }
}