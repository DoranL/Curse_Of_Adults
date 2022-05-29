using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public GameObject optionScreen;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToSettingsMenu()
    {
        optionScreen.SetActive(true);
    }

    public void CloseSettingMenu()
    {
        optionScreen.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
    public void QuitGame()
    {
        Application.Quit();
    }
}
