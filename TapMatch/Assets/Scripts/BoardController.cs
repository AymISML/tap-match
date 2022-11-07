using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private SpriteMask boardMask;
    [SerializeField] private SpriteRenderer boardSpriteRenderer;

    [SerializeField] private Transform elementsParent;

    [Header("Events")]
    [SerializeField] private GameStateEventSO gameStateEvent;
    [SerializeField] private InputEventSO[] inputEvents;

    [Header("Fall Props")]
    [SerializeField] private AnimationCurve fallAnimationCurve;
    [SerializeField] private float fallSpeed = 2.5f;

    private Vector2Int boardSize => TapMatchConfigSO.Instance.GetBoardSize();

    private Dictionary<Vector2Int, MatchableElement> matchables = new Dictionary<Vector2Int, MatchableElement>();
    private List<Task> fallDownTasks = new List<Task>();
    private bool isInputAllowed;

    private GameState gameState;

    private void Awake()
    {
        MatchablePool.Init();

        elementsParent.localPosition = new Vector2(
            -(boardSize.x * 0.5f) + 0.5f,
            -(boardSize.y * 0.5f) + 0.5f);

        boardMask.transform.localScale = new Vector3(boardSize.x, boardSize.y);
        boardSpriteRenderer.size = boardSize;

        gameState = new GameState(boardSize);

        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                var position = new Vector2Int(x, y);
                GetNewMatchableAtPosition(position, position);
            }
        }

        isInputAllowed = true;
        Camera.main.orthographicSize = Mathf.Max(boardSize.x, boardSize.y);

        foreach (var inputEvent in inputEvents)
            inputEvent.OnTileTapped += OnInputRecieved;

        gameStateEvent.OnRefreshGameState += UpdateGameState;
    }

    private void OnDestroy()
    {
        foreach (var inputEvent in inputEvents)
            inputEvent.OnTileTapped -= OnInputRecieved;

        gameStateEvent.OnRefreshGameState -= UpdateGameState;
    }

    private void UpdateGameState()
    {
        gameStateEvent.RaiseGameStateUpdated(gameState);
    }

    private void IncrementScore()
    {
        gameState.score++;
    }

    private void OnInputRecieved(Vector2Int input)
    {
        if (!ChecValidInput(input)) return;

        TapElementRecursivly(input, gameState.matchableStates[input.x][input.y]);
        RearrangeMatchables();
        IncrementScore();
    }

    private void TapElementRecursivly(Vector2Int elementPosition, int instanceId)
    {
        if (elementPosition.x > boardSize.x - 1 || elementPosition.x < 0 || elementPosition.y > boardSize.y - 1 || elementPosition.y < 0)
            return;

        if (instanceId < 0 || instanceId != gameState.matchableStates[elementPosition.x][elementPosition.y])
            return;

        IncrementScore();
        matchables[elementPosition].OnTapped();
        gameState.matchableStates[elementPosition.x][elementPosition.y] = -1;
        matchables[elementPosition] = null;

        TapElementRecursivly(elementPosition + Vector2Int.up, instanceId);
        TapElementRecursivly(elementPosition + Vector2Int.down, instanceId);
        TapElementRecursivly(elementPosition + Vector2Int.right, instanceId);
        TapElementRecursivly(elementPosition + Vector2Int.left, instanceId);
    }

    private void RearrangeMatchables()
    {
        bool repeat;
        int[] newPerColumn = new int[boardSize.x];
        do
        {
            repeat = false;

            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    if (gameState.matchableStates[x][y] < 0)
                    {
                        var newY = y + 1;
                        var position = new Vector2Int(x, y);
                        var newPosition = new Vector2Int(x, newY);

                        if (newY > boardSize.y - 1)
                        {
                            var matchable = GetNewMatchableAtPosition(position, newPosition + Vector2Int.up * newPerColumn[x]);
                            fallDownTasks.Add(matchable.FallDown(fallSpeed, fallAnimationCurve));

                            newPerColumn[x]++;
                        }
                        else
                        {
                            if (gameState.matchableStates[x][newY] < 0)
                                repeat = true;

                            gameState.matchableStates[x][y] = gameState.matchableStates[x][newY];
                            gameState.matchableStates[x][newY] = -1;

                            matchables[position] = matchables[newPosition];
                            matchables[newPosition] = null;

                            if (matchables[position] != null)
                            {
                                matchables[position].SetPosition(position);
                                fallDownTasks.Add(matchables[position].FallDown(fallSpeed, fallAnimationCurve));
                            }
                        }
                    }
                }
            }
        }
        while (repeat);

        FallAll();
    }

    private MatchableElement GetNewMatchableAtPosition(Vector2Int targetPosition, Vector2Int spawnPosition)
    {
        var matchableElement = MatchablePool.GetItem();
        matchableElement.transform.SetParent(elementsParent);

        var instanceId = Random.Range(0, TapMatchConfigSO.Instance.GetMatchablesCount());
        matchableElement.SetItem(instanceId, targetPosition, spawnPosition);
        gameState.matchableStates[targetPosition.x][targetPosition.y] = instanceId;

        if (matchables.ContainsKey(targetPosition))
            matchables[targetPosition] = matchableElement;
        else
            matchables.Add(targetPosition, matchableElement);

        return matchableElement;
    }

    private async void FallAll()
    {
        isInputAllowed = false;
        await Task.WhenAll(fallDownTasks);
        isInputAllowed = true;
        fallDownTasks.Clear();
        
        UpdateGameState();
    }

    private bool ChecValidInput(Vector2Int input)
    {
        bool valid = true;

        valid &= input.x >= 0;
        valid &= input.y >= 0;
        valid &= input.x < boardSize.x;
        valid &= input.y < boardSize.y;

        valid &= isInputAllowed;

        return valid;
    }
}
