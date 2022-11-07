using UnityEngine;

[CreateAssetMenu(fileName = "New Matchable Config", menuName = "TapMatch/Matchable Config")]
public class MatchableSO : ScriptableObject
{
    public int Id { get => GetInstanceID(); }

    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get => sprite; }

    [SerializeField] private Color color = Color.white;
    public Color Color { get => color; }
}