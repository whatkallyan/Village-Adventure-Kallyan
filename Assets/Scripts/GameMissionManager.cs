using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using StarterAssets;

public class GameMissionManager : MonoBehaviour
{
    public static GameMissionManager Instance;

    [Header("Mission & Timer Settings")]
    public float missionTime = 60f;
    public int targetCoins = 5;
    private float currentTimer;
    private int currentCoins = 0;
    private bool isMissionActive = false;

    [Header("Sapling & Pollution Settings")]
    public int saplingCount = 0;
    public int treesPlanted = 0;
    public int totalTreesNeeded = 3;

    [Header("Fog Settings")]
    public float maxFogDensity = 0.04f;
    public float minFogDensity = 0.002f;

    [Header("HUD UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI saplingText;
    public Slider pollutionSlider;
    public TextMeshProUGUI statusMessageText;

    [Header("Screen Panels & Player")]
    public GameObject introPanel;
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public MonoBehaviour playerController;
    private PlayerInput playerInput;
    private StarterAssetsInputs starterInputs;

    void Awake()
    {
        Instance = this;
        if (playerController != null)
        {
            playerInput = playerController.GetComponent<PlayerInput>();
            starterInputs = playerController.GetComponent<StarterAssetsInputs>();
        }
    }

    void Start()
    {
        RenderSettings.fog = true;
        UpdateEnvironment();
        UpdateUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (introPanel != null)
        {
            introPanel.SetActive(true);
            PauseGameAndUnlockCursor();
        }
        else
        {
            BeginGameplay();
        }
    }

    public void BeginGameplay()
    {
        if (introPanel != null) introPanel.SetActive(false);
        Time.timeScale = 1f;

        if (playerController != null) playerController.enabled = true;
        if (starterInputs != null)
        {
            starterInputs.enabled = true;
            starterInputs.cursorLocked = true;
            starterInputs.cursorInputForLook = true;
        }
        if (playerInput != null)
        {
            playerInput.ActivateInput();
            playerInput.SwitchCurrentActionMap("Player");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartNewMission();
    }

    void Update()
    {
        if (!isMissionActive) return;

        currentTimer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(currentTimer) + "s";

        if (currentTimer <= 0f)
        {
            CheckMissionResult();
        }
    }

    public void StartNewMission()
    {
        if (treesPlanted >= totalTreesNeeded) return;

        currentTimer = missionTime;
        currentCoins = 0;
        isMissionActive = true;
        UpdateUI();
    }

    public void AddCoin(int amount)
    {
        if (!isMissionActive) return;
        currentCoins += amount;
        UpdateUI();
    }

    void CheckMissionResult()
    {
        isMissionActive = false;

        if (currentCoins >= targetCoins)
        {
            saplingCount++;
            ShowMessage("Round Complete! You got 1 Sapling! Plant it at a marked spot.");
        }
        else
        {
            ShowMessage("Time Up! Not enough coins.");
            TriggerGameOver();
        }
        UpdateUI();
    }

    public void OnTreePlanted()
    {
        treesPlanted++;
        UpdateEnvironment();
        UpdateUI();

        if (treesPlanted >= totalTreesNeeded)
        {
            isMissionActive = false;
            ShowMessage("Congratulations! Village Environment is now Clean & Normal!");
            TriggerWin();
        }
        else
        {
            ShowMessage("Pollution Reduced! Starting next round...");
            Invoke(nameof(StartNewMission), 2.5f);
        }
    }

    void TriggerWin()
    {
        if (winPanel != null) winPanel.SetActive(true);
        PauseGameAndUnlockCursor();
    }

    void TriggerGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        PauseGameAndUnlockCursor();
    }

    void PauseGameAndUnlockCursor()
    {
        if (playerController != null) playerController.enabled = false;
        if (starterInputs != null)
        {
            starterInputs.cursorLocked = false;
            starterInputs.cursorInputForLook = false;
        }
        if (playerInput != null) playerInput.DeactivateInput();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateEnvironment()
    {
        float pollutionRatio = 1f - ((float)treesPlanted / totalTreesNeeded);
        pollutionRatio = Mathf.Clamp01(pollutionRatio);

        if (pollutionSlider != null)
            pollutionSlider.value = pollutionRatio;

        RenderSettings.fogDensity = Mathf.Lerp(minFogDensity, maxFogDensity, pollutionRatio);
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + currentCoins + "/" + targetCoins;
        if (saplingText != null)
            saplingText.text = "Saplings: " + saplingCount;
    }

    void ShowMessage(string msg)
    {
        if (statusMessageText != null)
            statusMessageText.text = msg;
        Debug.Log(msg);
    }
}