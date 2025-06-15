using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string menu;
    public string sceneName;
    public string leaderboard;
    public string settings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene(menu);
    }

    public void goToLeaderboard()
    {
        SceneManager.LoadScene(leaderboard);
    }

    public void goToSettings()
    {
        SceneManager.LoadScene(settings);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("quit");
    }
}
