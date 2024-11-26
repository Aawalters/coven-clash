using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;

    public int TotalLives {get; set;}
    public int CurrentWave {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        TotalLives = lives;
        //TODO: wave stuff
    }

    private void ReduceLives(Enemy enemy)
    {
        TotalLives--;
        if (TotalLives <= 0)
        {
            TotalLives = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.LogError("GAME OVER!!!");
        Time.timeScale = 0; 

        //TODO: load a scene or UI element that lets you restart
    }

    private void OnEnable()
    {
        Enemy.OnEndReached += ReduceLives;
        //TODO: wave stuff?
    }

    private void OnDisable() 
    {
        Enemy.OnEndReached -= ReduceLives;
    }

}
