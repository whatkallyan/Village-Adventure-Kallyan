using UnityEngine;

public class TreePlantSpot : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject treePrefab;
    public GameObject promptUI; // "Press E to Plant Sapling"

    private bool playerInRange = false;
    private bool isPlanted = false;

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !isPlanted && Input.GetKeyDown(KeyCode.E))
        {
            // প্লেয়ারের হাতে অন্তত ১টি চারা থাকলেই কেবল লাগাতে পারবে
            if (GameMissionManager.Instance != null && GameMissionManager.Instance.saplingCount > 0)
            {
                PlantTree();
            }
            else
            {
                Debug.Log("No sapling in hand! Collect coins and survive first.");
            }
        }
    }

    void PlantTree()
    {
        isPlanted = true;
        GameMissionManager.Instance.saplingCount--; // ১টি চারা খরচ হলো

        if (treePrefab != null)
        {
            Instantiate(treePrefab, transform.position, Quaternion.identity);
        }

        GameMissionManager.Instance.OnTreePlanted();

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlanted)
        {
            playerInRange = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}