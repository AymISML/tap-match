using System.Threading.Tasks;
using UnityEngine;

public class MatchableElement : MonoBehaviour
{
    private MatchableSO matchableSO;
    private SpriteRenderer itemSpriteRenderer;
    private Vector3 targetPosition;

    public int InstanceId { get; private set; }

    //private void Update()
    //{

    //}

    public void SetItem(int instanceId, Vector2 targetPosition, Vector2 spawnPosition)
    {
        if (itemSpriteRenderer == null)
        {
            itemSpriteRenderer = GetComponent<SpriteRenderer>();
            itemSpriteRenderer.sortingOrder = 1;
            itemSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        InstanceId = instanceId;
        matchableSO = TapMatchConfigSO.Instance.GetMatchableOfInstance(instanceId);

        itemSpriteRenderer.sprite = matchableSO.Sprite;
        itemSpriteRenderer.color = matchableSO.Color;

        transform.localPosition = spawnPosition;
        this.targetPosition = targetPosition;
    }

    public void SetPosition(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public void OnTapped()
    {
        itemSpriteRenderer.color = Color.black;
        MatchablePool.ReleaseItem(this);
    }

    public void ResetItem()
    {
        InstanceId = -1;
        matchableSO = null;
        transform.localPosition = Vector3.zero;
    }

    public async Task FallDown(float fallSpeed, AnimationCurve fallAnimationCurve)
    {
        var currentPosition = transform.localPosition;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fallSpeed;
            transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, fallAnimationCurve.Evaluate(t));
            await Task.Yield();
        }


        //var distance = targetPosition - transform.localPosition;

        //while (distance.sqrMagnitude > 0)
        //{
        //    var movment = distance.normalized * Time.deltaTime * fallSpeed;

        //    if (distance.sqrMagnitude < movment.sqrMagnitude)
        //        transform.localPosition = targetPosition;
        //    else
        //        transform.localPosition += movment;

        //    await Task.Yield();

        //    distance = targetPosition - transform.localPosition;
        //}
    }
}
