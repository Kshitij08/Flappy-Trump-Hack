using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    // References to game objects and UI elements
    public GameObject spawnerGO;
    public GameObject playerGO;
    public PlayerController playerController;

    public GameObject menuPanelGo;
    public GameObject singlePlayerButtonGO;
    public GameObject pVPButtonGO;
    public GameObject tournamentButtonGO;
    public GameObject leaderabordButtonGO;

    public GameObject gameOverPanelGO;
    public TMP_Text flapClaimText;
    public GameObject claimFlapButtonGO;
    public GameObject claimFlapMenuButtonGO;
    public TMP_Text flapClaimedText;

    public GameObject followOnXButtonGO;
    public GameObject viewNFTButtonGO;
    public GameObject claimNFTButtonGO;

    public GameObject portraitPopup;

    public TMP_Text walletAddressText;

    // Coroutines to control enemy wave and powerup spawning
    Coroutine storedEnemyWaveSpawnCoroutine;
    Coroutine storedPowerupSpawnCoroutine;

    public GameObject welcomeGO;
    public TMP_Text welcomeText;

    // Leaderboard UI elements and arrays to display wallet addresses and scores
    public GameObject leaderboardPanelGO;
    public TMP_Text[] leaderboardWalletArrayList = new TMP_Text[10];
    public TMP_Text[] leaderboardScoreArrayList = new TMP_Text[10];

    public TMP_Text leaderboardCurrentWalletText;
    public TMP_Text leaderboardCurrentScoreText;

    // Difficulty setting UI elements for Bull/Bear market modes
    public GameObject difficultySettingPanelGO;
    public GameObject bullMarketModeTextGO;
    public GameObject bearMarketModeTextGO;
    public Button bullMarketModeButton;
    public TMP_Text bullMarketButtonText;
    public Button bearMarketModeButton;
    public TMP_Text bearMarketButtonText;

    // Trump Almanac UI panels and index control
    public GameObject trumpAlmanacPanelGO;
    public GameObject[] trumpAlmanacPanelsGOArray;
    public int currentTrumpAlmanacPanelIndex = 0;

    // Transaction pop-up elements for displaying transaction info
    public GameObject transactionPopUpParent;
    public GameObject transactionPopUpPrefab;

    // Array of tags used to find and destroy game objects (enemies, obstacles, etc.)
    public string[] tagsToDestroy = new string[] {
        "BitcoinFly",
        "Elon",
        "FUDMonster",
        "Melania",
        "Obstacle",
        "RugPuller",
        "SECBullet",
        "SECGary",
        "TrumpCoin",
        "MrPresident",
        "Airdrop",
        "DiamondHands",
        "LiquidityInjection",
        "MAGAHat",
        "PepeBoost",
        "PumpAndDump",
        "StableGainsShield"
    };

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Check device orientation on startup.
        CheckOrientation();
    }

    // Start is called before the first frame update.
    void Start()
    {
        // Set initial wallet status.
        walletAddressText.text = "Wallet Disconnected";

        // Show the main menu and single player button, hide game over and difficulty panels.
        menuPanelGo.SetActive(true);
        singlePlayerButtonGO.SetActive(true);
        gameOverPanelGO.SetActive(false);
        difficultySettingPanelGO.SetActive(false);

        // Initialize market mode to Bull Market and update UI accordingly.
        GameManager.Instance.isBullMarket = true;
        bullMarketModeTextGO.SetActive(true);
        bearMarketModeTextGO.SetActive(false);
        bullMarketModeButton.interactable = false;
        bullMarketButtonText.color = new Color(1f, 0.624f, 0f, 1);
        bearMarketModeButton.interactable = true;
        bearMarketButtonText.color = new Color(1f, 0.83f, 0.44f, 1);
    }

    // Update is called once per frame.
    void Update()
    {
        // Continuously check screen orientation.
        CheckOrientation();
    }

    // Displays the game-over overlay for single-player mode.
    public void ShowSinglePlayerGameOverOverlay()
    {
        // Hide claim and follow buttons.
        claimFlapMenuButtonGO.SetActive(false);
        followOnXButtonGO.SetActive(false);
        viewNFTButtonGO.SetActive(false);

        // Enable menu mode and show the game over panel.
        GameManager.Instance.isMenuEnabled = true;
        gameOverPanelGO.SetActive(true);

        // Stop enemy and powerup spawning and remove existing enemies/obstacles.
        StopCoroutine(storedEnemyWaveSpawnCoroutine);
        StopCoroutine(storedPowerupSpawnCoroutine);
        DestroyEnemiesAndObstaclesAndPowerups();

        // Begin minting tokens based on player score.
        StartCoroutine(MintTokensCoroutine());

        // Batch mint and update NFTs.
        StartCoroutine(GameManager.Instance.web3Manager.BatchMintUpdateNFTs());

        // Show token claim text and start checking for pending transactions.
        flapClaimText.gameObject.SetActive(true);
        StartCoroutine(WaitForTransactions());
    }

    // Coroutine to claim tokens based on the player's score and update server info.
    public IEnumerator MintTokensCoroutine()
    {
        // Asynchronously claim tokens.
        Task claimTokenAsycnTask = GameManager.Instance.web3Manager.ClaimTokens(GameManager.Instance.playerScore);
        yield return new WaitUntil(() => claimTokenAsycnTask.IsCompleted);

        // Check the updated token balance.
        Task checkTokenAsyncTask = GameManager.Instance.web3Manager.CheckTokenBalance();
        yield return new WaitUntil(() => checkTokenAsyncTask.IsCompleted);

        // Update the user's score on the server.
        StartCoroutine(GameManager.Instance.webManager.UpdateUserScoreCoroutine(GameManager.Instance.playerWalletAddress, GameManager.Instance.playerFlapBalance));
    }

    // Coroutine to wait for all pending transactions to complete before updating UI.
    private IEnumerator WaitForTransactions()
    {
        // Record the last time the pending transaction count changed.
        float lastUpdateTime = Time.time;
        int lastPendingCount = GameManager.Instance.web3Manager.pendingTransactionCount;

        // Continue waiting while transactions are pending and within the allowed wait time.
        while (GameManager.Instance.web3Manager.pendingTransactionCount > 0 && (Time.time - lastUpdateTime) < 120f)
        {
            int currentPending = GameManager.Instance.web3Manager.pendingTransactionCount;
            if (currentPending != lastPendingCount)
            {
                lastPendingCount = currentPending;
                lastUpdateTime = Time.time;
            }

            // Update the UI to display the number of pending transactions.
            flapClaimText.text = "Please wait until all transactions are finished... <br> (" + currentPending + " pending...)";
            yield return new WaitForSeconds(0.5f);
        }

        // Once transactions are complete, display the final token and transaction info.
        if (GameManager.Instance.playerScore > 0)
        {
            flapClaimText.text = "Congrats! <br> " +
                                "You've earned <color=#DD3A3A>" + GameManager.Instance.playerScore + "</color> $FLAPs<br>" +
                                "You've minted <color=#DD3A3A>" + GameManager.Instance.web3Manager.nftMintCount + "</color> NFTs <br>" +
                                "You've upgraded <color=#DD3A3A>" + GameManager.Instance.web3Manager.nftUpdateCount + "</color> NFTs <br>" +
                                "You've done <color=#DD3A3A>" + GameManager.Instance.web3Manager.transactionCount + "</color> Transactions";

            Task checkTokenAsyncTask = GameManager.Instance.web3Manager.CheckTokenBalance();
            yield return new WaitUntil(() => checkTokenAsyncTask.IsCompleted);

            welcomeGO.SetActive(false);
            yield return StartCoroutine(GameManager.Instance.webManager.UpdateUserScoreCoroutine(GameManager.Instance.playerWalletAddress, GameManager.Instance.playerFlapBalance));
            claimFlapMenuButtonGO.SetActive(true);
            followOnXButtonGO.SetActive(true);
            viewNFTButtonGO.SetActive(true);
        }
        else
        {
            // If no tokens were earned, notify the player.
            flapClaimedText.text = "No $FLAPs to claim. Please try again!";
            claimFlapMenuButtonGO.SetActive(true);
            followOnXButtonGO.SetActive(true);
            viewNFTButtonGO.SetActive(true);
        }
    }

    // Resets game variables and returns to the main menu from the game-over overlay.
    public void OnClaimFlapMenuButtonClicked()
    {
        GameManager.Instance.isMenuEnabled = true;
        gameOverPanelGO.SetActive(false);
        menuPanelGo.SetActive(true);

        // Reset score and health.
        GameManager.Instance.playerScore = 0;
        GameManager.Instance.health = 3;
        GameManager.Instance.UpdateHealthBar();

        // Reset transaction counters.
        GameManager.Instance.web3Manager.pendingTransactionCount = 0;
        GameManager.Instance.web3Manager.nftMintCount = 0;
        GameManager.Instance.web3Manager.nftUpdateCount = 0;
    }

    // Starts a new single-player game.
    public void OnSinglePlayerClicked()
    {
        // Begin analytics tracking.
        GameManager.Instance.AnalyticsStart();

        // Disable menu and hide welcome panel.
        GameManager.Instance.isMenuEnabled = false;
        menuPanelGo.SetActive(false);
        welcomeGO.SetActive(false);

        // Reset enemy wave timer.
        GameManager.Instance.enemyWaveSpawner.localTimer = 0;

        // Reset transaction-related counters.
        GameManager.Instance.web3Manager.transactionCount = 0;
        GameManager.Instance.web3Manager.pendingTransactionCount = 0;
        GameManager.Instance.web3Manager.nftMintCount = 0;
        GameManager.Instance.web3Manager.nftUpdateCount = 0;

        // Ensure player graphics and collider are enabled.
        playerGO.GetComponent<SpriteRenderer>().enabled = true;
        playerGO.GetComponent<BoxCollider2D>().enabled = true;

        // Reset player's position and status.
        playerGO.transform.position = GameManager.Instance.iniitalPlayerPosition;
        playerController.rb.isKinematic = false;
        playerController.isMelania = false;
        playerController.isElon = false;
        playerController.isLiquidityInjection = false;
        playerController.isPepeBoost = false;
        playerController.isMAGAHat = false;
        playerController.isStableGainShield = false;
        playerController.isPumpAndDump = false;
        playerController.isPumping = false;
        playerController.isDiamondHands = false;

        // Activate the player object.
        playerGO.SetActive(true);

        // Start enemy wave and powerup spawning routines.
        storedEnemyWaveSpawnCoroutine = StartCoroutine(GameManager.Instance.enemyWaveSpawner.ProcessSpawnEvents());
        storedPowerupSpawnCoroutine = StartCoroutine(GameManager.Instance.powerupSpawner.SpawnRoutine());
    }

    // Checks the device orientation and toggles the portrait popup accordingly.
    void CheckOrientation()
    {
        if (Screen.width < Screen.height)
        {
            if (portraitPopup != null && !portraitPopup.activeSelf)
            {
                portraitPopup.SetActive(true);
            }
        }
        else
        {
            if (portraitPopup != null && portraitPopup.activeSelf)
            {
                portraitPopup.SetActive(false);
            }
        }
    }

    // Destroys all game objects that match the specified tags.
    public void DestroyEnemiesAndObstaclesAndPowerups()
    {
        foreach (string tag in tagsToDestroy)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }
    }

    // Begins the leaderboard process by filling in data and displaying the leaderboard panel.
    public void OnLeaderboardButtonClicked()
    {
        StartCoroutine(FillAndShowLeaderboard());
    }

    // Coroutine to retrieve and display leaderboard data.
    public IEnumerator FillAndShowLeaderboard()
    {
        yield return StartCoroutine(GameManager.Instance.webManager.GetLeaderboardCoroutine(GameManager.Instance.playerWalletAddress));

        // Update leaderboard UI with addresses and scores.
        for (int i = 0; i < leaderboardWalletArrayList.Length; i++)
        {
            leaderboardWalletArrayList[i].text = i + ". " + ShortenWalletAddress(GameManager.Instance.serverLeaderboardWalletAddresseArray[i]);
            leaderboardScoreArrayList[i].text = GameManager.Instance.serverLeaderboardScoreArray[i].ToString();
        }

        leaderboardCurrentWalletText.text = GameManager.Instance.serverUserRank + ". " + ShortenWalletAddress(GameManager.Instance.playerWalletAddress);
        leaderboardCurrentScoreText.text = GameManager.Instance.playerFlapBalance.ToString();

        leaderboardPanelGO.SetActive(true);
    }

    // Hides the leaderboard panel.
    public void OnLeaderboardBackButtonClicked()
    {
        leaderboardPanelGO.SetActive(false);
    }

    // Helper method to shorten a wallet address for display.
    public static string ShortenWalletAddress(string walletAddress)
    {
        if (string.IsNullOrEmpty(walletAddress) || walletAddress.Length < 10)
        {
            return walletAddress;
        }
        string firstPart = walletAddress.Substring(0, 6);
        string lastPart = walletAddress.Substring(walletAddress.Length - 4);
        return firstPart + "..." + lastPart;
    }

    // Opens the difficulty selection panel and updates button states based on the current market mode.
    public void OnDifficultySelectButtonClicked()
    {
        difficultySettingPanelGO.SetActive(true);

        if (GameManager.Instance.isBullMarket)
        {
            bullMarketModeButton.interactable = false;
            bullMarketButtonText.color = new Color(1f, 0.624f, 0f, 1);
            bullMarketModeTextGO.SetActive(true);
            bearMarketModeButton.interactable = true;
            bearMarketButtonText.color = new Color(1f, 0.83f, 0.44f, 1);
            bearMarketModeTextGO.SetActive(false);
        }
        else
        {
            bullMarketModeButton.interactable = true;
            bullMarketButtonText.color = new Color(1f, 0.83f, 0.44f, 1);
            bullMarketModeTextGO.SetActive(false);
            bearMarketModeButton.interactable = false;
            bearMarketButtonText.color = new Color(1f, 0.624f, 0f, 1);
            bearMarketModeTextGO.SetActive(true);
        }
    }

    // Closes the difficulty selection panel.
    public void OnDifficultySelectBackButtonClicked()
    {
        difficultySettingPanelGO.SetActive(false);
    }

    // Sets the market mode to Bull Market and updates the UI.
    public void OnBullMarketButtonClicked()
    {
        GameManager.Instance.isBullMarket = true;

        bullMarketModeButton.interactable = false;
        bullMarketButtonText.color = new Color(1f, 0.624f, 0f, 1);
        bullMarketModeTextGO.SetActive(true);

        bearMarketModeButton.interactable = true;
        bearMarketButtonText.color = new Color(1f, 0.83f, 0.44f, 1);
        bearMarketModeTextGO.SetActive(false);
    }

    // Sets the market mode to Bear Market and updates the UI.
    public void OnBearMarketButtonClicked()
    {
        GameManager.Instance.isBullMarket = false;

        bullMarketModeButton.interactable = true;
        bullMarketButtonText.color = new Color(1f, 0.83f, 0.44f, 1);
        bullMarketModeTextGO.SetActive(false);

        bearMarketModeButton.interactable = false;
        bearMarketButtonText.color = new Color(1f, 0.624f, 0f, 1);
        bearMarketModeTextGO.SetActive(true);
    }

    // Opens the Trump Almanac panel and shows the first panel.
    public void OnTrumpAlmanacButtonClicked()
    {
        foreach (GameObject panelGo in trumpAlmanacPanelsGOArray)
        {
            panelGo.SetActive(false);
        }
        currentTrumpAlmanacPanelIndex = 0;
        trumpAlmanacPanelsGOArray[currentTrumpAlmanacPanelIndex].SetActive(true);
        trumpAlmanacPanelGO.SetActive(true);
    }

    // Closes the Trump Almanac panel.
    public void OnTrumpAlmanacBackButtonClicked()
    {
        trumpAlmanacPanelGO.SetActive(false);
    }

    // Advances to the next panel in the Trump Almanac.
    public void OnTrumpAlmanacNextButtonClicked()
    {
        trumpAlmanacPanelsGOArray[currentTrumpAlmanacPanelIndex].SetActive(false);
        currentTrumpAlmanacPanelIndex += 1;
        if (currentTrumpAlmanacPanelIndex >= trumpAlmanacPanelsGOArray.Length)
        {
            currentTrumpAlmanacPanelIndex = 0;
        }
        trumpAlmanacPanelsGOArray[currentTrumpAlmanacPanelIndex].SetActive(true);
    }

    // Returns to the previous panel in the Trump Almanac.
    public void OnTrumpAlmanacPreviousButtonClicked()
    {
        trumpAlmanacPanelsGOArray[currentTrumpAlmanacPanelIndex].SetActive(false);
        currentTrumpAlmanacPanelIndex -= 1;
        if (currentTrumpAlmanacPanelIndex < 0)
        {
            currentTrumpAlmanacPanelIndex = trumpAlmanacPanelsGOArray.Length - 1;
        }
        trumpAlmanacPanelsGOArray[currentTrumpAlmanacPanelIndex].SetActive(true);
    }

    // Displays a transaction pop-up with the specified message.
    public void TransactionInfoPopUp(string popUPText)
    {
        GameObject transactionPopUp = Instantiate(transactionPopUpPrefab, transactionPopUpParent.transform);
        transactionPopUp.GetComponentInChildren<TMP_Text>().text = GameManager.Instance.web3Manager.transactionCount + ". " + popUPText;
        Destroy(transactionPopUp, 3f);
    }

    // Opens the Twitter (X) URL for following.
    public void OnFollowOnXClicked()
    {
        string twitterUrl = $"https://x.com/KshitijGajapure";
        Application.OpenURL(twitterUrl);
    }

    // Opens the MagicEden URL to view NFTs associated with the player's wallet.
    public void OnViewNFTsClicked()
    {
        string url = "https://magiceden.io/u/" + GameManager.Instance.playerWalletAddress +
                     "?activeTab=%22allItems%22&chains=%5B%22monad-testnet%22%5D&wallets=%5B%22" +
                     GameManager.Instance.playerWalletAddress + "%22%5D";
        Application.OpenURL(url);
    }
}
