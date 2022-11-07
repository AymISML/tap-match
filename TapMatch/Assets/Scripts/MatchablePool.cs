using UnityEngine;
using UnityEngine.Pool;

public static class MatchablePool
{
    private static IObjectPool<MatchableElement> pool;

    public static void Init()
    {
        int defaultCapacity = (int)Mathf.Pow(Constants.BOARD_MIN_BOUND, 2);
        int maxCapacirty = (int)Mathf.Pow(Constants.BOARD_MAX_BOUND, 2);

        pool = new ObjectPool<MatchableElement>(CreatePoolItem, OnGetPoolItem, OnReturnPoolItem, OnDestroyPoolItem, false, defaultCapacity, maxCapacirty);
    }

    public static MatchableElement GetItem()
    {
        return pool.Get();
    }

    public static void ReleaseItem(MatchableElement matchableElement)
    {
        pool.Release(matchableElement);
    }

    private static MatchableElement CreatePoolItem()
    {
        GameObject matchableElement = new GameObject("Matchable", typeof(SpriteRenderer), typeof(MatchableElement));
        matchableElement.SetActive(false);
        return matchableElement.GetComponent<MatchableElement>();
    }

    private static void OnGetPoolItem(MatchableElement matchableElement)
    {
        matchableElement.gameObject.SetActive(true);
    }

    private static void OnReturnPoolItem(MatchableElement matchableElement)
    {
        matchableElement.ResetItem();
        matchableElement.gameObject.SetActive(false);
    }

    private static void OnDestroyPoolItem(MatchableElement matchableElement)
    {
        Object.Destroy(matchableElement.gameObject);
    }
}
