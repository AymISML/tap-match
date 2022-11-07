using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TapMatchConfig", menuName = "TapMatch/Config")]
public class TapMatchConfigSO : ScriptableObject
{
    public static UnityAction ConfigUpdated;

    private static TapMatchConfigSO instance;
    public static TapMatchConfigSO Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<TapMatchConfigSO>("TapMatchConfig");
            }

            return instance;
        }
    }

    [Tooltip("Width of the Tap Match Board")]
    [SerializeField]
    [Range(Constants.BOARD_MIN_BOUND, Constants.BOARD_MAX_BOUND)]
    private int boardWidth = Constants.BOARD_MIN_BOUND;

    [Tooltip("Height of the Tap Match Board")]
    [SerializeField]
    [Range(Constants.BOARD_MIN_BOUND, Constants.BOARD_MAX_BOUND)]
    private int boardHeight = Constants.BOARD_MIN_BOUND;

    [SerializeField]
    private List<MatchableSO> matchables;

    public Vector2Int GetBoardSize()
    {
        return new Vector2Int(boardWidth, boardHeight);
    }

    public int GetMatchablesCount()
    {
        return matchables.Count;
    }

    public MatchableSO GetMatchableOfInstance(int instanceId)
    {
        return matchables[instanceId];
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            ConfigUpdated?.Invoke();
        }
    }
}