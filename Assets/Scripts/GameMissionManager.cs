using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMissionManager : MonoBehaviour
{
    public static GameMissionManager Instance;

    [Header("Mission & Timer Settings")]
    public float missionTime = 60f; // প্রতি রাউন্ডে বেঁচে থাকার সময় (সেকেন্ড)
    public int targetCoins = 5;     // প্রতিটি চারা পেতে যতগুলো কয়েন লাগবে
    private float currentTimer;
    private int currentCoins = 0;
    private bool isMissionActive = true;

    [Header("Sapling & Pollution Settings")]
    public int saplingCount = 0;      // প্লেয়ারের হাতে কয়টি চারা আছে
    public int treesPlanted = 0;      // কয়টি গাছ লাগানো হয়েছে (৩টি হলে জয়)
    public int totalTreesNeeded = 3;  // পরিবেশ স্বাভাবিক করতে মোট ৩টি গাছ লাগবে

    [Header("Fog Settings")]
    public float maxFogDensity = 0.04f;   // শুরুতে ঘন ধোঁয়া
    public float minFogDensity = 0.002f;  // ৩টি গাছ লাগানোর পর পরিষ্কার পরিবেশ

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI saplingText;
    public Slider pollutionSlider;
    public TextMeshProUGUI statusMessageText; // স্ক্রিনে মেসেজ দেখানোর জন্য

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RenderSettings.fog = true;
        StartNewMission();
        UpdateEnvironment();
        UpdateUI();
    }

    void Update()
    {
        if (!isMissionActive) return;

        currentTimer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(currentTimer) + "s";

        // সময় শেষ হলে চেক করবে টার্গেট কয়েন হয়েছে কি না
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
            saplingCount++; // সিস্টেম থেকে ১টি চারা রিওয়ার্ড পেল
            ShowMessage("Level Complete! You got 1 Sapling! Plant it now.");
        }
        else
        {
            ShowMessage("Time Up! Not enough coins. Retrying round...");
            Invoke(nameof(StartNewMission), 2.5f); // ২.৫ সেকেন্ড পর আবার চেষ্টা করবে
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
            ShowMessage("Congratulations! Village Environment is now Clean & Normal!");
        }
        else
        {
            ShowMessage("Pollution Reduced! Starting next survival round...");
            Invoke(nameof(StartNewMission), 2.5f);
        }
    }

    void UpdateEnvironment()
    {
        // ৩টি গাছ লাগালে ratio 1 থেকে কমে 0 হবে
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