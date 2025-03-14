using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Thirdweb.Unity.Examples
{
    [System.Serializable]
    public class WalletPanelUI
    {
        public string Identifier;
        public GameObject Panel;
        public Button Action1Button;
        public Button Action2Button;
        public Button Action3Button;
        public Button BackButton;
        public Button NextButton;
        public TMP_Text LogText;
        public TMP_InputField InputField;
        public Button InputFieldSubmitButton;
    }

    public class PlaygroundManager : MonoBehaviour
    {
        [field: SerializeField, Header("Wallet Options")]
        private ulong ActiveChainId = 10143;  //Monad Testnet

        [field: SerializeField]
        private bool WebglForceMetamaskExtension = false;

        [field: SerializeField, Header("Connect Wallet")]
        private GameObject ConnectWalletPanel;

        [field: SerializeField]
        private Button PrivateKeyWalletButton;

        [field: SerializeField]
        private Button EcosystemWalletButton;

        [field: SerializeField]
        private Button WalletConnectButton;

        [field: SerializeField, Header("Wallet Panels")]
        private List<WalletPanelUI> WalletPanels;

        private ThirdwebChainData _chainDetails;

        
        


        IThirdwebWallet thirdwaveWallet;
        public string walletAddress;
        public int score;

        private void Awake()
        {
            InitializePanels();
        }

        private async void Start()
        {
            try
            {
                _chainDetails = await Utils.GetChainMetadata(client: ThirdwebManager.Instance.Client, chainId: ActiveChainId);
            }
            catch
            {
                _chainDetails = new ThirdwebChainData()
                {
                    NativeCurrency = new ThirdwebChainNativeCurrency()
                    {
                        Decimals = 18,
                        Name = "ETH",
                        Symbol = "ETH"
                    }
                };
            }
        }

        private void InitializePanels()
        {
            CloseAllPanels();

            ConnectWalletPanel.SetActive(true);

            //PrivateKeyWalletButton.onClick.RemoveAllListeners();
            //PrivateKeyWalletButton.onClick.AddListener(() =>
            //{
            //    var options = GetWalletOptions(WalletProvider.PrivateKeyWallet);
            //    ConnectWallet(options);
            //});

            //EcosystemWalletButton.onClick.RemoveAllListeners();
            //EcosystemWalletButton.onClick.AddListener(() => InitializeEcosystemWalletPanel());

            WalletConnectButton.onClick.RemoveAllListeners();
            WalletConnectButton.onClick.AddListener(() =>
            {
                var options = GetWalletOptions(WalletProvider.WalletConnectWallet);
                ConnectWallet(options);
            });
        }

        private async void ConnectWallet(WalletOptions options)
        {
            // Connect the wallet

            var internalWalletProvider = options.Provider == WalletProvider.MetaMaskWallet ? WalletProvider.WalletConnectWallet : options.Provider;
            //var currentPanel = WalletPanels.Find(panel => panel.Identifier == internalWalletProvider.ToString());

            //Log(currentPanel.LogText, $"Connecting...");

            thirdwaveWallet = await ThirdwebManager.Instance.ConnectWallet(options);
            //var wallet = await ThirdwebManager.Instance.ConnectWallet(options);

            //var address = await wallet.GetAddress();
            var address = await thirdwaveWallet.GetAddress();
            address.CopyToClipboard();
            walletAddress = address;

            Debug.Log("Connected to: " + walletAddress);

            // Initialize the wallet panel

            //CloseAllPanels();

            //// Setup actions

            //ClearLog(currentPanel.LogText);
            //currentPanel.Panel.SetActive(true);

            //currentPanel.BackButton.onClick.RemoveAllListeners();
            //currentPanel.BackButton.onClick.AddListener(InitializePanels);

            //currentPanel.NextButton.onClick.RemoveAllListeners();
            //currentPanel.NextButton.onClick.AddListener(InitializeContractsPanel);

            //currentPanel.Action1Button.onClick.RemoveAllListeners();
            //currentPanel.Action1Button.onClick.AddListener(async () =>
            //{
            //    //var address = await wallet.GetAddress();
            //    var address = await thirdwaveWallet.GetAddress();
            //    address.CopyToClipboard();
            //    walletAddress = address;

            //    int rank = await GetRank();

            //    Log(currentPanel.LogText, $"Address: {address} with Rank: {rank}");
            //});

            //currentPanel.Action2Button.onClick.RemoveAllListeners();
            //currentPanel.Action2Button.onClick.AddListener(async () =>
            //{
            //    //var message = "Hello World!";
            //    ////var signature = await wallet.PersonalSign(message);
            //    //var signature = await thirdwaveWallet.PersonalSign(message);
            //    //Log(currentPanel.LogText, $"Signature: {signature}");

            //    //claimTokenButotnText.text = "claiming...";
            //    //claimTokenButton.interactable = false;

            //    Log(currentPanel.LogText, $"Claiming {score} tokens");


            //    //Claim Tokens based on score & show updated balance
            //    var contract = await ThirdwebManager.Instance.GetContract("0xCbdD8cFcbdf99F6207E0FF13d6626C33726c604A", chainId: ActiveChainId);
            //    var result = await contract.DropERC20_Claim(thirdwaveWallet, walletAddress, score.ToString());
            //    var balance = await contract.ERC20_BalanceOf(walletAddress);
            //    var symbol = await contract.ERC20_Symbol();


            //    Log(currentPanel.LogText, $"Claimed {score} {symbol} New balance: {balance.AdjustDecimals(18, 0)} {symbol}" );



            //    //claimTokenButotnText.text = "Claimed " + score + " tokens";


            //});

            //currentPanel.Action3Button.onClick.RemoveAllListeners();
            //currentPanel.Action3Button.onClick.AddListener(async () =>
            //{
            //    LoadingLog(currentPanel.LogText);
            //    ////var balance = await wallet.GetBalance(chainId: ActiveChainId);
            //    //var balance = await thirdwaveWallet.GetBalance(chainId: ActiveChainId);
            //    //var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
            //    //Log(currentPanel.LogText, $"Balance: {balanceEth} {_chainDetails.NativeCurrency.Symbol}");


            //    //Submit score to leaderboard
            //    await SubmitScore(score);
            //    //int rank = await GetRank();


            //    //Log(currentPanel.LogText, $"Claimed {score} {symbol} New balance: {balance.AdjustDecimals(18, 0)} {symbol} New Rank: {rank}" );
            //    Log(currentPanel.LogText, $"Submited Score: {score}");

            //    //Submit score to leaderboard
            //    //await SubmitScore(score);
            //    int rank = await GetRank();
            //    Log(currentPanel.LogText, $"New Rank: {rank}");



            //});
        }

        //Get Rank
        internal async Task<int> GetRank()
        {
            var contract = await ThirdwebManager.Instance.GetContract(
                "0xb4De476a27BC35e63Ce18113c223584bf6FA5386",
                chainId: ActiveChainId,
                "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"ScoreAdded\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"_scores\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getRank\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"rank\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"submitScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]"
                );

            var rank = await contract.Read<int>("getRank", walletAddress);

            return rank;
        }

        //Submit score to leaderboard
        internal async Task SubmitScore(int score)
        {
            var contract = await ThirdwebManager.Instance.GetContract(
                "0xb4De476a27BC35e63Ce18113c223584bf6FA5386",
                chainId: ActiveChainId,
                "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"ScoreAdded\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"_scores\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getRank\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"rank\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"submitScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]"
                );

            await contract.Write(thirdwaveWallet, "submitScore", BigInteger.Zero, (int)score);

        }

        private WalletOptions GetWalletOptions(WalletProvider provider)
        {
            switch (provider)
            {
                case WalletProvider.PrivateKeyWallet:
                    return new WalletOptions(provider: WalletProvider.PrivateKeyWallet, chainId: ActiveChainId);
                case WalletProvider.EcosystemWallet:
                    var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Google);
                    return new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
                case WalletProvider.WalletConnectWallet:
                    var externalWalletProvider =
                        Application.platform == RuntimePlatform.WebGLPlayer && WebglForceMetamaskExtension ? WalletProvider.MetaMaskWallet : WalletProvider.WalletConnectWallet;
                    return new WalletOptions(provider: externalWalletProvider, chainId: ActiveChainId);
                default:
                    throw new System.NotImplementedException("Wallet provider not implemented for this example.");
            }
        }

        //private void InitializeEcosystemWalletPanel()
        //{
        //    var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Authentication");

        //    CloseAllPanels();

        //    ClearLog(panel.LogText);
        //    panel.Panel.SetActive(true);

        //    panel.BackButton.onClick.RemoveAllListeners();
        //    panel.BackButton.onClick.AddListener(InitializePanels);

        //    // Email
        //    panel.Action1Button.onClick.RemoveAllListeners();
        //    panel.Action1Button.onClick.AddListener(() =>
        //    {
        //        InitializeEcosystemWalletPanel_Email();
        //    });

        //    // Phone
        //    panel.Action2Button.onClick.RemoveAllListeners();
        //    panel.Action2Button.onClick.AddListener(() =>
        //    {
        //        InitializeEcosystemWalletPanel_Phone();
        //    });

        //    // Socials
        //    panel.Action3Button.onClick.RemoveAllListeners();
        //    panel.Action3Button.onClick.AddListener(() =>
        //    {
        //        InitializeEcosystemWalletPanel_Socials();
        //    });
        //}

        //private void InitializeEcosystemWalletPanel_Email()
        //{
        //    var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Email");

        //    CloseAllPanels();

        //    ClearLog(panel.LogText);
        //    panel.Panel.SetActive(true);

        //    panel.BackButton.onClick.RemoveAllListeners();
        //    panel.BackButton.onClick.AddListener(InitializeEcosystemWalletPanel);

        //    panel.InputFieldSubmitButton.onClick.RemoveAllListeners();
        //    panel.InputFieldSubmitButton.onClick.AddListener(() =>
        //    {
        //        try
        //        {
        //            var email = panel.InputField.text;
        //            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", email: email);
        //            var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
        //            ConnectWallet(options);
        //        }
        //        catch (System.Exception e)
        //        {
        //            Log(panel.LogText, e.Message);
        //        }
        //    });
        //}

        //private void InitializeEcosystemWalletPanel_Phone()
        //{
        //    var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Phone");

        //    CloseAllPanels();

        //    ClearLog(panel.LogText);
        //    panel.Panel.SetActive(true);

        //    panel.BackButton.onClick.RemoveAllListeners();
        //    panel.BackButton.onClick.AddListener(InitializeEcosystemWalletPanel);

        //    panel.InputFieldSubmitButton.onClick.RemoveAllListeners();
        //    panel.InputFieldSubmitButton.onClick.AddListener(() =>
        //    {
        //        try
        //        {
        //            var phone = panel.InputField.text;
        //            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", phoneNumber: phone);
        //            var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
        //            ConnectWallet(options);
        //        }
        //        catch (System.Exception e)
        //        {
        //            Log(panel.LogText, e.Message);
        //        }
        //    });
        //}

        //private void InitializeEcosystemWalletPanel_Socials()
        //{
        //    var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Socials");

        //    CloseAllPanels();

        //    ClearLog(panel.LogText);
        //    panel.Panel.SetActive(true);

        //    panel.BackButton.onClick.RemoveAllListeners();
        //    panel.BackButton.onClick.AddListener(InitializeEcosystemWalletPanel);

        //    // socials action 1 is google, 2 is apple 3 is discord

        //    panel.Action1Button.onClick.RemoveAllListeners();
        //    panel.Action1Button.onClick.AddListener(() =>
        //    {
        //        try
        //        {
        //            Log(panel.LogText, "Authenticating...");
        //            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Google);
        //            var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
        //            ConnectWallet(options);
        //        }
        //        catch (System.Exception e)
        //        {
        //            Log(panel.LogText, e.Message);
        //        }
        //    });

        //    panel.Action2Button.onClick.RemoveAllListeners();
        //    panel.Action2Button.onClick.AddListener(() =>
        //    {
        //        try
        //        {
        //            Log(panel.LogText, "Authenticating...");
        //            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Apple);
        //            var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
        //            ConnectWallet(options);
        //        }
        //        catch (System.Exception e)
        //        {
        //            Log(panel.LogText, e.Message);
        //        }
        //    });

        //    panel.Action3Button.onClick.RemoveAllListeners();
        //    panel.Action3Button.onClick.AddListener(() =>
        //    {
        //        try
        //        {
        //            Log(panel.LogText, "Authenticating...");
        //            var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Discord);
        //            var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
        //            ConnectWallet(options);
        //        }
        //        catch (System.Exception e)
        //        {
        //            Log(panel.LogText, e.Message);
        //        }
        //    });
        //}

        private void InitializeContractsPanel()
        {
            var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "Contracts");

            CloseAllPanels();

            ClearLog(panel.LogText);
            panel.Panel.SetActive(true);

            panel.BackButton.onClick.RemoveAllListeners();
            panel.BackButton.onClick.AddListener(InitializePanels);

            panel.NextButton.onClick.RemoveAllListeners();
            panel.NextButton.onClick.AddListener(InitializeAccountAbstractionPanel);

            // Get NFT
            panel.Action1Button.onClick.RemoveAllListeners();
            panel.Action1Button.onClick.AddListener(async () =>
            {
                //try
                //{
                //    LoadingLog(panel.LogText);
                //    var dropErc1155Contract = await ThirdwebManager.Instance.GetContract(address: "0x94894F65d93eb124839C667Fc04F97723e5C4544", chainId: ActiveChainId);
                //    var nft = await dropErc1155Contract.ERC1155_GetNFT(tokenId: 1);
                //    Log(panel.LogText, $"NFT: {JsonConvert.SerializeObject(nft.Metadata)}");
                //    var sprite = await nft.GetNFTSprite(client: ThirdwebManager.Instance.Client);
                //    // spawn image for 3s
                //    var image = new GameObject("NFT Image", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                //    image.transform.SetParent(panel.Panel.transform, false);
                //    image.GetComponent<Image>().sprite = sprite;
                //    Destroy(image, 3f);
                //}
                //catch (System.Exception e)
                //{
                //    Log(panel.LogText, e.Message);
                //}


                //Create Contract
                try
                {
                    LoadingLog(panel.LogText);
                    var contract = await ThirdwebManager.Instance.GetContract(address: "0x0652755FF5608a1d82249894Ea4c0B3BF0A164Ea", chainId: ActiveChainId);
                    var result = await contract.ERC1155_URI(tokenId: 1);
                    Log(panel.LogText, $"Result (uri): {result}");
                }
                catch (System.Exception e)
                {
                    Log(panel.LogText, e.Message);
                }

            });

            // Call contract
            panel.Action2Button.onClick.RemoveAllListeners();
            panel.Action2Button.onClick.AddListener(async () =>
            {
                try
                {
                    LoadingLog(panel.LogText);
                    var contract = await ThirdwebManager.Instance.GetContract(address: "0x0652755FF5608a1d82249894Ea4c0B3BF0A164Ea", chainId: ActiveChainId);
                    var result = await contract.ERC1155_URI(tokenId: 1);
                    Log(panel.LogText, $"Result (uri): {result}");
                }
                catch (System.Exception e)
                {
                    Log(panel.LogText, e.Message);
                }
            });

            // Get ERC20 Balance
            panel.Action3Button.onClick.RemoveAllListeners();
            panel.Action3Button.onClick.AddListener(async () =>
            {
                try
                {
                    LoadingLog(panel.LogText);
                    var dropErc20Contract = await ThirdwebManager.Instance.GetContract(address: "0xEBB8a39D865465F289fa349A67B3391d8f910da9", chainId: ActiveChainId);
                    var symbol = await dropErc20Contract.ERC20_Symbol();
                    var balance = await dropErc20Contract.ERC20_BalanceOf(ownerAddress: await ThirdwebManager.Instance.GetActiveWallet().GetAddress());
                    var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 0, addCommas: false);
                    Log(panel.LogText, $"Balance: {balanceEth} {symbol}");
                }
                catch (System.Exception e)
                {
                    Log(panel.LogText, e.Message);
                }
            });
        }

        private async void InitializeAccountAbstractionPanel()
        {
            var currentWallet = ThirdwebManager.Instance.GetActiveWallet();
            var smartWallet = await ThirdwebManager.Instance.UpgradeToSmartWallet(personalWallet: currentWallet, chainId: ActiveChainId, smartWalletOptions: new SmartWalletOptions(sponsorGas: true));

            var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "AccountAbstraction");

            CloseAllPanels();

            ClearLog(panel.LogText);
            panel.Panel.SetActive(true);

            panel.BackButton.onClick.RemoveAllListeners();
            panel.BackButton.onClick.AddListener(InitializePanels);

            // Personal Sign (1271)
            panel.Action1Button.onClick.RemoveAllListeners();
            panel.Action1Button.onClick.AddListener(async () =>
            {
                try
                {
                    var message = "Hello, World!";
                    var signature = await smartWallet.PersonalSign(message);
                    Log(panel.LogText, $"Signature: {signature}");
                }
                catch (System.Exception e)
                {
                    Log(panel.LogText, e.Message);
                }
            });

            // Create Session Key
            panel.Action2Button.onClick.RemoveAllListeners();
            panel.Action2Button.onClick.AddListener(async () =>
            {
                try
                {
                    Log(panel.LogText, "Granting Session Key...");
                    var randomWallet = await PrivateKeyWallet.Generate(ThirdwebManager.Instance.Client);
                    var randomWalletAddress = await randomWallet.GetAddress();
                    var timeTomorrow = Utils.GetUnixTimeStampNow() + 60 * 60 * 24;
                    var sessionKey = await smartWallet.CreateSessionKey(
                        signerAddress: randomWalletAddress,
                        approvedTargets: new List<string> { Constants.ADDRESS_ZERO },
                        nativeTokenLimitPerTransactionInWei: "0",
                        permissionStartTimestamp: "0",
                        permissionEndTimestamp: timeTomorrow.ToString(),
                        reqValidityStartTimestamp: "0",
                        reqValidityEndTimestamp: timeTomorrow.ToString()
                    );
                    Log(panel.LogText, $"Session Key Created for {randomWalletAddress}: {sessionKey.TransactionHash}");
                }
                catch (System.Exception e)
                {
                    Log(panel.LogText, e.Message);
                }
            });

            // Get Active Signers
            panel.Action3Button.onClick.RemoveAllListeners();
            panel.Action3Button.onClick.AddListener(async () =>
            {
                try
                {
                    LoadingLog(panel.LogText);
                    var activeSigners = await smartWallet.GetAllActiveSigners();
                    Log(panel.LogText, $"Active Signers: {JsonConvert.SerializeObject(activeSigners)}");
                }
                catch (System.Exception e)
                {
                    Log(panel.LogText, e.Message);
                }
            });
        }

        private void CloseAllPanels()
        {
            ConnectWalletPanel.SetActive(false);
            foreach (var walletPanel in WalletPanels)
            {
                walletPanel.Panel.SetActive(false);
            }
        }

        private void ClearLog(TMP_Text logText)
        {
            logText.text = string.Empty;
        }

        private void Log(TMP_Text logText, string message)
        {
            logText.text = message;
            ThirdwebDebug.Log(message);
        }

        private void LoadingLog(TMP_Text logText)
        {
            logText.text = "Loading...";
        }

        //public async void ClaimTokens()
        //{
        //    claimTokenButotnText.text = "claiming...";
        //    claimTokenButton.interactable = false;
        //    //await ThirdwebManager.Instance.GetContract(address: "0x0652755FF5608a1d82249894Ea4c0B3BF0A164Ea", chainId: ActiveChainId);
        //    var contract = await ThirdwebManager.Instance.GetContract("0xCbdD8cFcbdf99F6207E0FF13d6626C33726c604A", chainId: ActiveChainId);
        //    var result = await contract.DropERC20_Claim(thirdwaveWallet, walletAddress, score.ToString());

        //    claimTokenButotnText.text = "Claimed " + score + " tokens";

        //}
    }
}
