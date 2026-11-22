using UnityEngine;
using UnityEngine.UI;

public class MinimapCoinIndicator : MonoBehaviour
{
    public Transform player;          // PlayerCapsule
    public Transform coinsParent;     // Hierarchy-র Coins গ্রুপ
    public RectTransform arrowUI;     // CoinDirectionArrow UI
    public float edgeRadius = 35f;    // মিনিম্যাপের কেন্দ্র থেকে নিরাপদ বর্ডার দূরত্ব

    void Update()
    {
        if (player == null || coinsParent == null || arrowUI == null) return;

        Transform nearestCoin = GetNearestActiveCoin();

        // সব কয়েন সংগ্রহ হয়ে গেলে তীরটি লুকিয়ে রাখবে
        if (nearestCoin == null)
        {
            arrowUI.gameObject.SetActive(false);
            return;
        }

        arrowUI.gameObject.SetActive(true);

        // প্লেয়ার থেকে কয়েনের দিক (X এবং Z অক্ষে) বের করা
        Vector3 direction = nearestCoin.position - player.position;
        direction.y = 0f;

        // UI-তে তীরের পজিশন মিনিম্যাপের ঠিক কেন্দ্র (40.3, -41) থেকে চারপাশে সেট করা
        Vector2 centerOffset = new Vector2(40.3f, -41f);
        Vector2 dir2D = new Vector2(direction.x, direction.z).normalized;
        Vector2 targetPos = dir2D * (edgeRadius * 1.4f);

        arrowUI.anchoredPosition = centerOffset + new Vector2(
            Mathf.Clamp(targetPos.x, -edgeRadius, edgeRadius),
            Mathf.Clamp(targetPos.y, -edgeRadius, edgeRadius)
        );

        // তীরটিকে কয়েনের দিকে ঘোরানো
        float angle = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg;
        arrowUI.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    Transform GetNearestActiveCoin()
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform coin in coinsParent)
        {
            if (coin.gameObject.activeInHierarchy)
            {
                float dist = Vector3.Distance(player.position, coin.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = coin;
                }
            }
        }
        return nearest;
    }
}