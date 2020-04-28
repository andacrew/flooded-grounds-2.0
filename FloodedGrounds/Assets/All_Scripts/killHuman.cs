using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Objective))]
public class ObjectiveKillEnemies : MonoBehaviour
{
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";
    public string loadScene; 
    public int killsToCompleteObjective = 3;

    EnemyManager m_EnemyManager;
    Objective m_Objective;
    int m_KillTotal;

    void Start()
    {
        m_Objective = GetComponent<Objective>();
        m_EnemyManager = FindObjectOfType<EnemyManager>();      
        m_EnemyManager.onRemoveEnemy += OnKillEnemy;
    }

    void OnKillEnemy(EnemyController enemy, int remaining)
    {
  
        m_KillTotal = m_EnemyManager.numberOfEnemiesTotal - remaining;
        int targetRemaning = mustKillAllEnemies ? remaining : killsToCompleteObjective - m_KillTotal;

        // update the objective text according to how many enemies remain to kill
        if (targetRemaning == 0)
        {
            //loadScene = SceneManagement.GetActiveScene().buildIndex + 1;
            loadScene = winScene;
            SceneManagement.LoadScene(loadScene);
            Debug.Log("You Win!");
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Max" || other.gameObject.tag == "Winston" || other.gameObject.tag == "Izzy")
        {
            if (hud.hasKeys && hud.hasGas && hud.hasSteeringWheel)
            {
                //loadScene = SceneManagement.GetActiveScene().buildIndex + 2;
                loadScene = loseScene;
                SceneManagement.LoadScene(loadScene);
                Debug.Log("You Win!");
            }
        
        }
    }
}