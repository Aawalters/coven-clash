using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    PreWave,  // Shop & Upgrade available
    Wave,     // Combat phase (no upgrades/shop)
    PostWave, // Dialogue phase
    Settings  // Settings menu (overlay)
}

public class GameStateManager : MonoBehaviour 
{
    public static GameStateManager Instance { get; private set; }

    public GameState currentState { get; private set; } = GameState.PreWave; // begin in pre-wave

    void Awake() 
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ChangeState(GameState newState) 
    {
        currentState = newState;
        Debug.Log("Game State changed to: " + currentState);
    }

    public bool IsState(GameState state) 
    {
        return currentState == state;
    }
}

