using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Game State Event", menuName = "TapMatch/Game State Event")]
public class GameStateEventSO : ScriptableObject
{
    public UnityAction<GameState> OnGameStateUpdated;
    public UnityAction OnRefreshGameState;

    public void RaiseGameStateUpdated(GameState gameState)
    {
        OnGameStateUpdated?.Invoke(gameState);
    }

    public void RaiseRefreshGameState()
    {
        OnRefreshGameState?.Invoke();
    }
}