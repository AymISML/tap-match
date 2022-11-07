using UnityEngine;

[System.Serializable]
public struct GameState
{
    public int score;
    public int[][] matchableStates;

    public GameState(Vector2Int size)
    {
        score = 0;
        matchableStates = new int[size.x][];
        for (int i = 0; i < size.x; i++)
        {
            matchableStates[i] = new int[size.y];
        }
    }
}