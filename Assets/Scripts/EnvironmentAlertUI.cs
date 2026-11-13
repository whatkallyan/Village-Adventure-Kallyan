using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnvironmentAlertUI : MonoBehaviour
{
    [Header("UI Components")]
    public Text alertMessageText;
    public Text forestHealthText;

    [Header("Environment Settings")]
    public int maxForestHealth = 100;
    private int currentForestHealth;
    public float alertDisplayDuration = 3.5f;

    private Coroutine activeAlertCoroutine;

    void Start()
    {
        currentForestHealth = maxForestHealth;
        UpdateHealthUI();

        if (alertMessageText != null)
        {
            alertMessageText.text = "";
            alertMessageText.gameObject.SetActive(false);
        }
    }

    
    public void OnTreeCutDown(int damageAmount)
    {
        currentForestHealth = Mathf.Clamp(currentForestHealth - damageAmount, 0, maxForestHealth);
        UpdateHealthUI();

        string warning = "Warning: Cutting trees harms the island ecosystem! Forest Health: " + currentForestHealth + "%";
        ShowEnvironmentAlert(warning);
    }

    
    public void OnTreePlanted(int restoreAmount)
    {
        currentForestHealth = Mathf.Clamp(currentForestHealth + restoreAmount, 0, maxForestHealth);
        UpdateHealthUI();

        ShowEnvironmentAlert("Great job! Planting trees restores the environment.");
    }

    private void UpdateHealthUI()
    {
        if (forestHealthText != null)
        {
            forestHealthText.text = "Forest Health: " + currentForestHealth + "%";
        }
    }

    public void ShowEnvironmentAlert(string message)
    {
        if (alertMessageText == null) return;

        if (activeAlertCoroutine != null)
        {
            StopCoroutine(activeAlertCoroutine);
        }
        activeAlertCoroutine = StartCoroutine(DisplayAlertRoutine(message));
    }

    private IEnumerator DisplayAlertRoutine(string message)
    {
        alertMessageText.text = message;
        alertMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(alertDisplayDuration);

        alertMessageText.gameObject.SetActive(false);
    }
}