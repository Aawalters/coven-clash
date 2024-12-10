using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;
    [SerializeField] private Image _baseHealthBar;
    [SerializeField] private Button menuButton;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text loseText;

    public int TotalLives {get; set;}
    public int CurrentWave {get; set;}
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        TotalLives = lives;
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        menuButton.onClick.AddListener(() => LoadMenu());
        menuButton.gameObject.SetActive(false);
        audioManager = AudioManager.Instance;
    }

    private void ReduceLives(Enemy enemy)
    {
        TotalLives--;
         _baseHealthBar.fillAmount = (float)TotalLives / (float)lives;
        if (TotalLives <= 0)
        {
            TotalLives = 0;
            Lose();
        }
    }

    private void Lose()
    {
        //Debug.LogError("GAME OVER!!!");
        Time.timeScale = 0; 
       // audioManager.PlaySFX(audioManager.lose);
        loseText.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
    }

    public void Win()
    {
        Time.timeScale = 0; 
       // audioManager.PlaySFX(audioManager.win);
        winText.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += ReduceLives;
    }

    private void OnDisable() 
    {
        Enemy.OnEndReached -= ReduceLives;
    }

    private void LoadMenu()
    {
        audioManager.PlaySFX(audioManager.click);
        SceneManager.LoadSceneAsync(0);
    }

}
