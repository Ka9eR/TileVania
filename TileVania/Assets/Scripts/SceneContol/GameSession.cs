using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;
    [SerializeField] float deathSceneReloadDelay = 1f;
    
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    private void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;

        if(numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }
    public void ProcessPlayerDeath()
    {
        StartCoroutine(PlayerDeath());
    }
    public IEnumerator PlayerDeath()
    {
        yield return new WaitForSecondsRealtime(deathSceneReloadDelay);
        int currectSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currectSceneIndex + 1;

        if (playerLives > 1)
        {
            TakeLife();
            //Debug.Log("Life taken");
        }
        else
        {
            ResetGameSession();
            //Debug.Log("Reset level");
        }
    }

    void TakeLife()
    {
        playerLives--;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        livesText.text = playerLives.ToString();
    }

    void ResetGameSession()
    {
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
