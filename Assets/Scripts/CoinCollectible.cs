using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public int coinValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameMissionManager.Instance != null)
            {
                GameMissionManager.Instance.AddCoin(coinValue);
            }
            Destroy(gameObject);
        }
    }
}