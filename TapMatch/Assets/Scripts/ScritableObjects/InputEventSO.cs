using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Input Event", menuName = "TapMatch/Input Event")]
public class InputEventSO : ScriptableObject
{
    public UnityAction<Vector2Int> OnTileTapped;

    public void RaiseTileTapped(Vector2Int position)
    {
        OnTileTapped?.Invoke(position);
    }
}