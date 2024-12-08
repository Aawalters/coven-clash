using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;
    public GameObject menuPanel;
    public Button settingsCloseButton;
    public GameObject settingsPanel;

    AudioManager audioManager;

    public void Start()
    {
        startButton.onClick.AddListener(() => PlayGame());
        settingsButton.onClick.AddListener(() => OpenSettings());
        quitButton.onClick.AddListener(() => ExitGame());
        settingsCloseButton.onClick.AddListener(() => CloseSettings());
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        audioManager = AudioManager.Instance;
    }

    public void PlayGame()
    {
        //Debug.Log("play button pressed");
        audioManager.PlaySFX(audioManager.click);
        SceneManager.LoadSceneAsync(1);
    }

    public void OpenSettings()
    {
        //Debug.Log("settings button pressed");
        audioManager.PlaySFX(audioManager.click);
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        //Debug.Log("exit button pressed");
        audioManager.PlaySFX(audioManager.click);
        Application.Quit();
    }

    public void CloseSettings()
    {
        audioManager.PlaySFX(audioManager.click);
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
}
