using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{

    public float endSceneLoadDelay = 3f;
    public CanvasGroup endGameFadeCanvasGroup;
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";
    public float delayBeforeFadeToBlack = 4f;
    public float delayBeforeWinMessage = 2f;
    public GameObject WinGameMessagePrefab;
    public bool gameIsEnding { get; private set; }

    PlayerCharacterController m_Player;   //human
    NotificationHUDManager m_NotificationHUDManager;
    ObjectiveManager m_ObjectiveManager;  //monster
    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;

    void Start()
    {
        m_Player = FindObjectOfType<PlayerCharacterController>(); 
        m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
    }

    void Update()
    {
        if (gameIsEnding)
        {
            float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / endSceneLoadDelay;
            endGameFadeCanvasGroup.alpha = timeRatio;


            // See if it's time to load the end scene (after the a certain time frame)
            if (Time.time >= m_TimeLoadEndGameScene)
            {
                SceneManager.LoadScene(m_SceneToLoad);
                gameIsEnding = false;
            }
        }
        else
        {
            if (m_ObjectiveManager.AreAllObjectivesCompleted())
                EndGame(true);

            // Test if player died
            if (m_Player.isDead)
                EndGame(false);
        }
    }

    void EndGame(bool win)
    {

        // load the appropriate end scene 
        gameIsEnding = true;
        endGameFadeCanvasGroup.gameObject.SetActive(true);
        if (win)
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;
        }
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay;
        }
    }
}