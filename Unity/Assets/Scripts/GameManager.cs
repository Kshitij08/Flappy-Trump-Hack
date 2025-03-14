using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isAnalyticsInitialized = false;

    [Header("Player Information")]
    public string playerWalletAddress;
    public int playerFlapBalance;
    public Thirdweb.Unity.Examples.Web3Manager web3Manager;

    [Header("Score Management")]
    public float playerScore = 0;
    public TMP_Text scoreText;

    [Header("Health & UI")]
    public GameObject[] healthBarArray;
    public int health = 3;

    [Header("Game Systems")]
    public EnemyWaveSpawner enemyWaveSpawner;
    public PowerupSpawner powerupSpawner;
    public Menu menu;

    [Header("Audio Management")]
    public AudioSource audioSource;
    public AudioClip[] audioClipList;
    private int currentClipIndex = 0;
    public float startVolume = 0.3f;
    public float endVolume = 1f;
    public float fadeDuration = 5f;

    [Header("Game State")]
    public bool isMenuEnabled = true;
    public bool isBullMarket = true;

    [Header("Player GameObject")]
    public GameObject playerGO;
    public PlayerController playerController;
    public Vector3 iniitalPlayerPosition = new Vector3(-4, 0, 0);

    [Header("Server Data")]
    public WebManager webManager;
    public string serverWalletAddress;
    public int serverTotalScore;
    public int serverUserRank;
    public string[] serverLeaderboardWalletAddresseArray = new string[10];
    public int[] serverLeaderboardScoreArray = new int[10];

    [Header("NFT Configurations")]
    public List<Material> nftMaterialArrayList = new List<Material>(7);
    public List<float> nftMultiplierList = new List<float> { 1f, 1.5f, 2f, 3f, 5f, 7.5f, 10f };

    void Awake()
    {
        // Singleton pattern to ensure only one GameManager instance exists.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Uncomment the next line if you want this object to persist across scenes.
            // DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Ensure screen is set to landscape mode.
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // Initialize Unity Services & Analytics
        InitializeAnalytics();

        // Set initial score display.
        scoreText.text = "$FLAP: " + playerScore;

        // Store initial player position.
        iniitalPlayerPosition = playerGO.transform.position;

        // Set up audio and start playback.
        InitializeAudio();
    }

    private void Update()
    {
        // Cycle to the next audio clip when pressing '1' key.
        if (Input.GetKeyDown(KeyCode.Alpha1) && audioClipList.Length > 0)
        {
            currentClipIndex = (currentClipIndex + 1) % audioClipList.Length;
            audioSource.clip = audioClipList[currentClipIndex];
            audioSource.Play();
        }
    }

    /// <summary>
    /// Initializes Unity Analytics Service.
    /// </summary>
    private async void InitializeAnalytics()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            isAnalyticsInitialized = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Analytics initialization failed: {e.Message}");
        }
    }

    /// <summary>
    /// Initializes audio settings and starts playing the first track.
    /// </summary>
    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioClipList != null && audioClipList.Length > 0)
        {
            audioSource.clip = audioClipList[currentClipIndex];
        }

        audioSource.volume = startVolume;
        audioSource.Play();

        // Start the volume fade effect.
        StartCoroutine(FadeVolume());
    }

    /// <summary>
    /// Updates the player's health bar UI.
    /// </summary>
    public void UpdateHealthBar()
    {
        for (int i = 0; i < healthBarArray.Length; i++)
        {
            if (i < (3 - health))
            {
                healthBarArray[i].GetComponent<Image>().color = new Color(255f / 255f, 145f / 255f, 145f / 255f, 145f / 255f);
            }
            else
            {
                healthBarArray[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    /// <summary>
    /// Gradually increases the volume from startVolume to endVolume over fadeDuration.
    /// </summary>
    IEnumerator FadeVolume()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    /// <summary>
    /// Sends player's final score to Unity Analytics.
    /// </summary>
    public void AnalyticsScore()
    {
        if (!isAnalyticsInitialized) return;

        CustomEvent myEvent = new CustomEvent("final_score")
        {
            {"score", playerScore }
        };

        AnalyticsService.Instance.RecordEvent(myEvent);
    }

    /// <summary>
    /// Sends a start game event to Unity Analytics.
    /// </summary>
    public void AnalyticsStart()
    {
        if (!isAnalyticsInitialized) return;
        AnalyticsService.Instance.RecordEvent("start_game");
    }

    /// <summary>
    /// Updates the player's score and updates the UI.
    /// </summary>
    public void UpdateScore(float score)
    {
        // Round the score to two decimal places.
        playerScore += Mathf.Round(score * 100f) / 100f;

        // Update UI
        scoreText.text = "$FLAP: " + playerScore;

        // Debug.Log($"Updated Score: {score}");
        // await web3Manager.ClaimTokens(roundedScore);
    }
}
