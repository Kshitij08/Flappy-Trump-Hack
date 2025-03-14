using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Thirdweb.Unity.Examples
{
    public class Web3Manager : MonoBehaviour
    {
        #region Wallet Options

        [field: SerializeField, Header("Wallet Options")]
        private ulong ActiveChainId = 10143;  // Monad Testnet

        [field: SerializeField]
        private bool WebglForceMetamaskExtension = true;

        // Chain metadata details fetched from Thirdweb.
        private ThirdwebChainData _chainDetails;

        // Reference to the connected wallet interface.
        IThirdwebWallet thirdwaveWallet;
        public string walletAddress;

        // UI elements for connecting the wallet.
        public Button connectWalletButton;
        public TMP_Text connectWalletText;
        public GameObject connectWalletPanelGo;

        // Engine wallet configuration (used for gas fees and transaction signing).
        string engineURL = "";
        string authToken = "";

        #endregion

        #region Engine Wallets

        // Nine engine wallets that can be used to distribute transaction load.
        public EngineWallet engineWallet1;
        string backendWalletAddress1 = "";
        public EngineWallet engineWallet2;
        string backendWalletAddress2 = "";
        public EngineWallet engineWallet3;
        string backendWalletAddress3 = "";
        public EngineWallet engineWallet4;
        string backendWalletAddress4 = "";
        public EngineWallet engineWallet5;
        string backendWalletAddress5 = "";
        public EngineWallet engineWallet6;
        string backendWalletAddress6 = "";
        public EngineWallet engineWallet7;
        string backendWalletAddress7 = "";
        public EngineWallet engineWallet8;
        string backendWalletAddress8 = "";
        public EngineWallet engineWallet9;
        string backendWalletAddress9 = "";

        #endregion

        #region Counters and Transaction Stats

        // Counters for tracking NFT-related transactions.
        public int pendingTransactionCount = 0;
        public int transactionCount = 0;
        public int nftMintCount = 0;
        public int nftUpdateCount = 0;

        #endregion

        #region NFT Metadata Info

        // NGMI NFT Metadata
        [Header("NGMI NFT Metadata")]
        public string nGMINFTName = "NGMI";
        public string nGMINFTDescription = "Minted by destroying NGMI Obstacle. Requires 25 XP to Level up.";
        public List<string> nGMINFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMI.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMISilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMIGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMIRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMIGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMIVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NGMIHell.mp4"
        };
        public string nGMINFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string nGMINFTBackgroundColor = "";
        public string nGMINFTLevel = "1";
        public string nGMINFTXp = "0";
        public int nGMINFTXpThreshold = 25;
        public string nGMINFTAnimationUrl = null;

        // Paper Hands NFT Metadata
        [Header("Paper Hands NFT Metadata")]
        public string paperHandsNFTName = "Paper Hands";
        public string paperHandsNFTDescription = "Minted by destroying Paper Hands Obstacle.  Requires 25 XP to Level up.";
        public List<string> paperHandsNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHands.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHandsSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHandsGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHandsRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHandsGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHandsVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PaperHandsHell.mp4"
        };
        public string paperHandsNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string paperHandsNFTBackgroundColor = "";
        public string paperHandsNFTLevel = "1";
        public string paperHandsNFTXp = "0";
        public int paperHandsNFTXpThreshold = 25;
        public string paperHandsNFTAnimationUrl = null;

        // FUD NFT Metadata
        [Header("FUD NFT Metadata")]
        public string fUDNFTName = "FUD";
        public string fUDNFTDescription = "Minted by destroying FUD Obstacle. Requires 20 XP to Level up.";
        public List<string> fUDNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUD.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUDSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUDGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUDRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUDGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUDVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/FUDHell.mp4"
        };
        public string fUDNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string fUDNFTBackgroundColor = "";
        public string fUDNFTLevel = "1";
        public string fUDNFTXp = "0";
        public int fUDNFTXpThreshold = 20;
        public string fUDNFTAnimationUrl = null;

        // Gary NFT Metadata
        [Header("Gary NFT Metadata")]
        public string garyNFTName = "Gary";
        public string garyNFTDescription = "Minted by defeating Gary. Requires 10 XP to Level up.";
        public List<string> garyNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/Gary.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/GarySilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/GaryGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/GaryRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/GaryGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/GaryVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/GaryHell.mp4"
        };
        public string garyNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string garyNFTBackgroundColor = "";
        public string garyNFTLevel = "1";
        public string garyNFTXp = "0";
        public int garyNFTXpThreshold = 10;
        public string garyNFTAnimationUrl = null;

        // Melania NFT Metadata
        [Header("Melania NFT Metadata")]
        public string melaniaNFTName = "Melania";
        public string melaniaNFTDescription = "Minted by defeating Melania. Requires 8 XP to Level up.";
        public List<string> melaniaNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/Melania.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MelaniaSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MelaniaGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MelaniaRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MelaniaGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MelaniaVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MelaniaHell.mp4"
        };
        public string melaniaNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string melaniaNFTBackgroundColor = "";
        public string melaniaNFTLevel = "1";
        public string melaniaNFTXp = "0";
        public int melaniaNFTXpThreshold = 8;
        public string melaniaNFTAnimationUrl = null;

        // Elon NFT Metadata
        [Header("Elon NFT Metadata")]
        public string elonNFTName = "Elon";
        public string elonNFTDescription = "Minted by defeating Elon. Requires 4 XP to Level up.";
        public List<string> elonNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/Elon.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/ElonSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/ElonGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/ElonRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/ElonGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/ElonVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/ElonHell.mp4"
        };
        public string elonNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string elonNFTBackgroundColor = "";
        public string elonNFTLevel = "1";
        public string elonNFTXp = "0";
        public int elonNFTXpThreshold = 4;
        public string elonNFTAnimationUrl = null;

        // Bitcoin Fly NFT Metadata
        [Header("Bitcoin Fly NFT Metadata")]
        public string bitcoinFlyNFTName = "Bitcoin Fly";
        public string bitcoinFlyNFTDescription = "Minted by defeating Bitcoin Fly. Requires 2 XP to Level up.";
        public List<string> bitcoinFlyNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFly.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFlySilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFlyGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFlyRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFlyGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFlyVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/BitcoinFlyHell.mp4"
        };
        public string bitcoinFlyNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string bitcoinFlyNFTBackgroundColor = "";
        public string bitcoinFlyNFTLevel = "1";
        public string bitcoinFlyNFTXp = "0";
        public int bitcoinFlyNFTXpThreshold = 2;
        public string bitcoinFlyNFTAnimationUrl = null;

        // Diamond Hands NFT Metadata
        [Header("Diamond Hands NFT Metadata")]
        public string diamondHandsNFTName = "Diamond Hands";
        public string diamondHandsNFTDescription = "Minted by collecting Diamond Hands. Requires 5 XP to Level up.";
        public List<string> diamondHandsNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHands.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHandsSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHandsGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHandsRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHandsGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHandsVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/DiamondHandsHell.mp4"
        };
        public string diamondHandsNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string diamondHandsNFTBackgroundColor = "";
        public string diamondHandsNFTLevel = "1";
        public string diamondHandsNFTXp = "0";
        public int diamondHandsNFTXpThreshold = 5;
        public string diamondHandsNFTAnimationUrl = null;

        // MAGA Hat NFT Metadata
        [Header("MAGA Hat NFT Metadata")]
        public string mAGAHatNFTName = "MAGA Hat";
        public string mAGAHatNFTDescription = "Minted by collecting MAGA Hat. Requires 5 XP to Level up.";
        public List<string> mAGAHatNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHat.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHatSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHatGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHatRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHatGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHatVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/MAGAHatHell.mp4"
        };
        public string mAGAHatNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string mAGAHatNFTBackgroundColor = "";
        public string mAGAHatNFTLevel = "1";
        public string mAGAHatNFTXp = "0";
        public int mAGAHatNFTXpThreshold = 5;
        public string mAGAHatNFTAnimationUrl = null;

        // Nuke 'Em NFT Metadata
        [Header("Nuke 'Em NFT Metadata")]
        public string nukeEmNFTName = "Nuke 'Em";
        public string nukeEmNFTDescription = "Minted by collecting Nuke 'Em. Requires 5 XP to Level up.";
        public List<string> nukeEmNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEm.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEmSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEmGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEmRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEmGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEmVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/NukeEmHell.mp4"
        };
        public string nukeEmNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string nukeEmNFTBackgroundColor = "";
        public string nukeEmNFTLevel = "1";
        public string nukeEmNFTXp = "0";
        public int nukeEmNFTXpThreshold = 5;
        public string nukeEmNFTAnimationUrl = null;

        // Liquidity Injection NFT Metadata
        [Header("Liquidity Injection NFT Metadata")]
        public string liquidityInjectionNFTName = "Liquidity Injection";
        public string liquidityInjectionNFTDescription = "Minted by collecting Liquidity Injection. Requires 5 XP to Level up.";
        public List<string> liquidityInjectionNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjection.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjectionSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjectionGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjectionRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjectionGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjectionVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/LiquidityInjectionHell.mp4"
        };
        public string liquidityInjectionNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string liquidityInjectionNFTBackgroundColor = "";
        public string liquidityInjectionNFTLevel = "1";
        public string liquidityInjectionNFTXp = "0";
        public int liquidityInjectionNFTXpThreshold = 5;
        public string liquidityInjectionNFTAnimationUrl = null;

        // Airdrop NFT Metadata
        [Header("Airdrop NFT Metadata")]
        public string airdropNFTName = "Airdrop";
        public string airdropNFTDescription = "Minted by collecting Airdrop. Requires 5 XP to Level up.";
        public List<string> airdropNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/Airdrop.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/AirdropSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/AirdropGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/AirdropRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/AirdropGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/AirdropVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/AirdropHell.mp4"
        };
        public string airdropNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string airdropNFTBackgroundColor = "";
        public string airdropNFTLevel = "1";
        public string airdropNFTXp = "0";
        public int airdropNFTXpThreshold = 5;
        public string airdropNFTAnimationUrl = null;

        // Pepe Boost NFT Metadata
        [Header("Pepe Boost NFT Metadata")]
        public string pepeBoostNFTName = "Pepe Boost";
        public string pepeBoostNFTDescription = "Minted by collecting Pepe Boost. Requires 5 XP to Level up.";
        public List<string> pepeBoostNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoostSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoostGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoostRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoostGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoostVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PepeBoostHell.mp4"
        };
        public string pepeBoostNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string pepeBoostNFTBackgroundColor = "";
        public string pepeBoostNFTLevel = "1";
        public string pepeBoostNFTXp = "0";
        public int pepeBoostNFTXpThreshold = 5;
        public string pepeBoostNFTAnimationUrl = null;

        // USDC Shield NFT Metadata
        [Header("USDC Shield NFT Metadata")]
        public string uSDCShieldNFTName = "USDC Shield";
        public string uSDCShieldNFTDescription = "Minted by collecting USDC Shield. Requires 5 XP to Level up.";
        public List<string> uSDCShieldNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShield.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShieldSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShieldGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShieldRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShieldGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShieldVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/USDCShieldHell.mp4"
        };
        public string uSDCShieldNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string uSDCShieldNFTBackgroundColor = "";
        public string uSDCShieldNFTLevel = "1";
        public string uSDCShieldNFTXp = "0";
        public int uSDCShieldNFTXpThreshold = 5;
        public string uSDCShieldNFTAnimationUrl = null;

        // Pump And Dump NFT Metadata
        [Header("Pump And Dump NFT Metadata")]
        public string pumpAndDumpNFTName = "Pump And Dump";
        public string pumpAndDumpNFTDescription = "Minted by collecting Pump And Dump. Requires 5 XP to Level up.";
        public List<string> pumpAndDumpNFTImageList = new List<string>
        {
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDump.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDumpSilver.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDumpGold.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDumpRainbow.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDumpGhost.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDumpVaporwave.mp4",
            "https://plum-coyote-976718.hostingersite.com/NFTs/PumpAndDumpHell.mp4"
        };
        public string pumpAndDumpNFTExternalUrl = "https://flappy-trump-beta.vercel.app/";
        public string pumpAndDumpNFTBackgroundColor = "";
        public string pumpAndDumpNFTLevel = "1";
        public string pumpAndDumpNFTXp = "0";
        public int pumpAndDumpNFTXpThreshold = 5;
        public string pumpAndDumpNFTAnimationUrl = null;

        #endregion

        #region Player NFT Info

        // The following fields store the currently active NFT data for each type.

        // NGMI
        [Header("Current NGMI NFT")]
        public int nGMINFTCurrentId = -1;
        public int nGMINFTCurrentLevel = 1;
        public int nGMINFTCurrentXp = 0;
        public string nGMINFTCurrentUri;

        // Paper Hands
        [Header("Current Paper Hands NFT")]
        public int paperHandsNFTCurrentId = -1;
        public int paperHandsNFTCurrentLevel = 1;
        public int paperHandsNFTCurrentXp = 0;
        public string paperHandsNFTCurrentUri;

        // FUD
        [Header("Current FUD NFT")]
        public int fUDNFTCurrentId = -1;
        public int fUDNFTCurrentLevel = 1;
        public int fUDNFTCurrentXp = 0;
        public string fUDNFTCurrentUri;

        // Gary
        [Header("Current Gary NFT")]
        public int garyNFTCurrentId = -1;
        public int garyNFTCurrentLevel = 1;
        public int garyNFTCurrentXp = 0;
        public string garyNFTCurrentUri;

        // Melania
        [Header("Current Melania NFT")]
        public int melaniaNFTCurrentId = -1;
        public int melaniaNFTCurrentLevel = 1;
        public int melaniaNFTCurrentXp = 0;
        public string melaniaNFTCurrentUri;

        // Elon
        [Header("Current Elon NFT")]
        public int elonNFTCurrentId = -1;
        public int elonNFTCurrentLevel = 1;
        public int elonNFTCurrentXp = 0;
        public string elonNFTCurrentUri;

        // Bitcoin Fly
        [Header("Current Bitcoin Fly NFT")]
        public int bitcoinFlyNFTCurrentId = -1;
        public int bitcoinFlyNFTCurrentLevel = 1;
        public int bitcoinFlyNFTCurrentXp = 0;
        public string bitcoinFlyNFTCurrentUri;

        // Diamond Hands
        [Header("Current Diamond Hands NFT")]
        public int diamondHandsNFTCurrentId = -1;
        public int diamondHandsNFTCurrentLevel = 1;
        public int diamondHandsNFTCurrentXp = 0;
        public string diamondHandsNFTCurrentUri;

        // MAGA Hat
        [Header("Current MAGA Hat NFT")]
        public int mAGAHatNFTCurrentId = -1;
        public int mAGAHatNFTCurrentLevel = 1;
        public int mAGAHatNFTCurrentXp = 0;
        public string mAGAHatNFTCurrentUri;

        // Nuke 'Em
        [Header("Current Nuke Em NFT")]
        public int nukeEmNFTCurrentId = -1;
        public int nukeEmNFTCurrentLevel = 1;
        public int nukeEmNFTCurrentXp = 0;
        public string nukeEmNFTCurrentUri;

        // Liquidity Injection
        [Header("Current Liquidity Injection NFT")]
        public int liquidityInjectionNFTCurrentId = -1;
        public int liquidityInjectionNFTCurrentLevel = 1;
        public int liquidityInjectionNFTCurrentXp = 0;
        public string liquidityInjectionNFTCurrentUri;

        // Airdrop
        [Header("Current Airdrop NFT")]
        public int airdropNFTCurrentId = -1;
        public int airdropNFTCurrentLevel = 1;
        public int airdropNFTCurrentXp = 0;
        public string airdropNFTCurrentUri;

        // Pepe Boost
        [Header("Current Pepe Boost NFT")]
        public int pepeBoostNFTCurrentId = -1;
        public int pepeBoostNFTCurrentLevel = 1;
        public int pepeBoostNFTCurrentXp = 0;
        public string pepeBoostNFTCurrentUri;

        // USDC Shield
        [Header("Current USDC Shield NFT")]
        public int uSDCShieldNFTCurrentId = -1;
        public int uSDCShieldNFTCurrentLevel = 1;
        public int uSDCShieldNFTCurrentXp = 0;
        public string uSDCShieldNFTCurrentUri;

        // Pump And Dump
        [Header("Current Pump And Dump NFT")]
        public int pumpAndDumpNFTCurrentId = -1;
        public int pumpAndDumpNFTCurrentLevel = 1;
        public int pumpAndDumpNFTCurrentXp = 0;
        public string pumpAndDumpNFTCurrentUri;

        #endregion

        #region Game NFT Counters

        // Counters to track the number of NFT-related events.
        public int nGMINFTCount = 0;
        public int paperHandsNFTCount = 0;
        public int fUDNFTCount = 0;
        public int garyNFTCount = 0;
        public int melaniaNFTCount = 0;
        public int elonNFTCount = 0;
        public int bitcoinFlyNFTCount = 0;
        public int diamondHandsNFTCount = 0;
        public int mAGAHatNFTCount = 0;
        public int nukeEmNFTCount = 0;
        public int liquidityInjectionNFTCount = 0;
        public int airdropNFTCount = 0;
        public int pepeBoostNFTCount = 0;
        public int uSDCShieldNFTCount = 0;
        public int pumpAndDumpNFTCount = 0;

        #endregion

        #region MonoBehaviour Lifecycle

        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            // Ensure the wallet connection panel is visible on startup.
            connectWalletPanelGo.SetActive(true);
        }

        // Start is called before the first frame update.
        private async void Start()
        {
            // Attempt to retrieve chain metadata from Thirdweb.
            try
            {
                _chainDetails = await Utils.GetChainMetadata(client: ThirdwebManager.Instance.Client, chainId: ActiveChainId);
            }
            catch
            {
                // Fallback to default metadata if retrieval fails.
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

            // Create engine wallets for handling transactions.
            CreateEngineWallet1();
            CreateEngineWallet2();
            CreateEngineWallet3();
            CreateEngineWallet4();
            CreateEngineWallet5();
            CreateEngineWallet6();
            CreateEngineWallet7();
            CreateEngineWallet8();
            CreateEngineWallet9();

            // Update connection UI.
            connectWalletText.text = "Please connect wallet to continue!";
        }

        #endregion

        #region Wallet Connection Methods

        // Triggered by UI when the user clicks the "Connect Wallet" button.
        public void OnConnectWalletClicked()
        {
            connectWalletButton.interactable = false;
            var options = GetWalletOptions(WalletProvider.WalletConnectWallet);
            ConnectWallet(options);
        }

        // Returns wallet options based on the selected provider.
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
                    throw new NotImplementedException("Wallet provider not implemented for this example.");
            }
        }

        // Connects to the wallet using the provided options.
        private async void ConnectWallet(WalletOptions options)
        {
            try
            {
                connectWalletText.text = "Connecting to wallet...";
                var internalWalletProvider = options.Provider == WalletProvider.MetaMaskWallet ? WalletProvider.WalletConnectWallet : options.Provider;

                thirdwaveWallet = await ThirdwebManager.Instance.ConnectWallet(options);
                var address = await thirdwaveWallet.GetAddress();
                walletAddress = address;

                GameManager.Instance.playerWalletAddress = address;
                GameManager.Instance.menu.walletAddressText.text = address;

                connectWalletText.text = "Connected to " + walletAddress;

                await CheckTokenBalance();

                GameManager.Instance.menu.welcomeText.text = "Welcome, " + GameManager.Instance.playerWalletAddress + ", you have " + GameManager.Instance.playerFlapBalance + " $FLAPs!";

                // Store and update user score on the server.
                StartCoroutine(GameManager.Instance.webManager.GetOrCreateUserScoreCoroutine(walletAddress));
                StartCoroutine(GameManager.Instance.webManager.UpdateUserScoreCoroutine(walletAddress, GameManager.Instance.playerFlapBalance));

                connectWalletText.text = "Retrieving NFTs for " + walletAddress;
                await GetUserNFTs();

                connectWalletButton.interactable = true;
                connectWalletPanelGo.SetActive(false);

                Debug.Log("Connected to: " + walletAddress);
            }
            catch (Exception ex)
            {
                Debug.Log("Error connecting wallet: " + ex.Message);
                connectWalletText.text = "Opps, something went wrong. Please try again!";
                connectWalletButton.interactable = true;
            }
        }

        #endregion

        #region Token and NFT Functions

        // Claims ERC20 tokens based on score; attempts multiple times if needed.
        public async Task ClaimTokens(float score)
        {
            var contract = await ThirdwebManager.Instance.GetContract("0x3d6A5699eeBeC63cDeCeDb625164Bd57424f13F3", chainId: 10143);
            int maxAttempts = 5;
            int attempt = 0;
            bool success = false;

            transactionCount += 1;
            pendingTransactionCount += 1;

            while (!success && attempt < maxAttempts)
            {
                try
                {
                    // Build a list of available engine wallets.
                    var engineWallets = new List<EngineWallet>
                    {
                        engineWallet1, engineWallet2, engineWallet3, engineWallet4, engineWallet5,
                        engineWallet6, engineWallet7, engineWallet8, engineWallet9
                    };

                    // Randomly select a valid wallet.
                    EngineWallet selectedWallet = null;
                    int startIndex = UnityEngine.Random.Range(0, engineWallets.Count);
                    int index = startIndex;
                    do
                    {
                        selectedWallet = engineWallets[index];
                        if (selectedWallet != null)
                            break;
                        index = (index + 1) % engineWallets.Count;
                    } while (index != startIndex);

                    if (selectedWallet != null)
                    {
                        var result = await contract.DropERC20_Claim(selectedWallet, walletAddress, score.ToString());
                        GameManager.Instance.menu.TransactionInfoPopUp(result.TransactionHash);
                        success = true;
                    }
                    else
                    {
                        Debug.LogWarning("No valid engine wallets available.");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    attempt++;
                    Debug.LogWarning($"Attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(1000);
                }
            }

            pendingTransactionCount -= 1;
            if (!success)
            {
                Debug.LogError("Failed to claim tokens after multiple attempts.");
            }
        }

        // Checks the token balance of the connected wallet.
        public async Task CheckTokenBalance()
        {
            var contract = await ThirdwebManager.Instance.GetContract("0x3d6A5699eeBeC63cDeCeDb625164Bd57424f13F3", chainId: 10143);
            int maxAttempts = 5;
            int attempt = 0;
            bool success = false;
            BigInteger balance = 0;
            while (!success && attempt < maxAttempts)
            {
                try
                {
                    balance = await contract.ERC20_BalanceOf(walletAddress);
                    success = true;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Attempt {attempt + 1} failed: {ex.Message}");
                    attempt++;
                    await Task.Delay(1000);
                }
            }
            if (success)
            {
                GameManager.Instance.playerFlapBalance = (int)balance.AdjustDecimals(18, 0);
                Debug.Log("Token balance updated successfully.");
            }
            else
            {
                Debug.LogError("Failed to get token balance after multiple attempts.");
            }
        }

        // Coroutine to mint or update NFT metadata.
        public IEnumerator MintNFTCoroutine(string nftName, string nftDescription, string nftImageURL, string nftExternalURL, string nftLevel, string nftXp, string nftAnimationURL)
        {
            // Generate NFT metadata JSON.
            string nftJson = NFTJsonGenerator.GenerateNFTMetadata(
                name: nftName,
                description: nftDescription,
                image: nftImageURL,
                externalUrl: nftExternalURL,
                backgroundColor: "",
                level: nftLevel,
                xp: nftXp,
                animationUrl: nftAnimationURL
            );

            // Upload the JSON metadata and capture the URL.
            string metadataUrl = null;
            yield return StartCoroutine(GameManager.Instance.webManager.UploadJson(nftJson, nftName, (result) =>
            {
                metadataUrl = result;
            }));

            if (!string.IsNullOrEmpty(metadataUrl))
            {
                Task mintTask = MintNFTs(metadataUrl);
                yield return new WaitUntil(() => mintTask.IsCompleted);
            }
            else
            {
                Debug.LogError("Metadata upload failed. Aborting mint.");
            }
        }

        // Mints an NFT using the provided metadata URL.
        public async Task MintNFTs(string jsonUrl)
        {
            var contract = await ThirdwebManager.Instance.GetContract("0xb2570aB0c9e1dBac805C40E78121d61BFa4eEAaa", chainId: 10143);
            int maxAttempts = 5;
            int attempt = 0;
            bool success = false;
            nftMintCount += 1;
            pendingTransactionCount += 1;

            while (!success && attempt < maxAttempts)
            {
                try
                {
                    var engineWallets = new List<EngineWallet>
                    {
                        engineWallet1, engineWallet2, engineWallet3, engineWallet4, engineWallet5,
                        engineWallet6, engineWallet7, engineWallet8, engineWallet9
                    };

                    int walletindex = UnityEngine.Random.Range(0, engineWallets.Count);
                    if (engineWallets[walletindex] != null)
                    {
                        var result = await contract.TokenERC721_MintTo(engineWallets[walletindex], walletAddress, jsonUrl);
                        GameManager.Instance.menu.TransactionInfoPopUp(result.TransactionHash);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    attempt++;
                    Debug.LogWarning($"Attempt {attempt} failed: {ex.Message}");
                    await Task.Delay(1000);
                }
            }
            pendingTransactionCount -= 1;
            if (!success)
            {
                Debug.LogError("Failed to mint NFT after multiple attempts.");
            }
        }

        // Retrieves the user's NFTs and updates the current NFT info for each type.
        public async Task GetUserNFTs()
        {
            var contract = await ThirdwebManager.Instance.GetContract("0xb2570aB0c9e1dBac805C40E78121d61BFa4eEAaa", chainId: 10143);
            List<NFT> result = await contract.ERC721_GetOwnedNFTs(walletAddress);
            connectWalletText.text = "Retrieved " + result.Count + " NFTs";

            // Dictionary to store the highest-level NFT for each unique name.
            Dictionary<string, NFT> highestNftsByName = new Dictionary<string, NFT>();

            foreach (NFT nft in result)
            {
                int level = 0;
                int xp = 0;
                if (nft.Metadata.Attributes != null)
                {
                    var attributeArray = nft.Metadata.Attributes as JArray;
                    if (attributeArray != null)
                    {
                        NftTrait[] traits = attributeArray.ToObject<NftTrait[]>();
                        foreach (var trait in traits)
                        {
                            if (trait.trait_type.Equals("level", StringComparison.OrdinalIgnoreCase))
                            {
                                int.TryParse(trait.value, out level);
                            }
                            else if (trait.trait_type.Equals("xp", StringComparison.OrdinalIgnoreCase))
                            {
                                int.TryParse(trait.value, out xp);
                            }
                        }
                    }
                }
                string nftName = nft.Metadata.Name;
                if (!highestNftsByName.ContainsKey(nftName))
                {
                    highestNftsByName[nftName] = nft;
                }
                else
                {
                    int storedLevel = 0, storedXp = 0;
                    NFT storedNft = highestNftsByName[nftName];
                    if (storedNft.Metadata.Attributes != null)
                    {
                        var storedAttributes = storedNft.Metadata.Attributes as JArray;
                        if (storedAttributes != null)
                        {
                            NftTrait[] storedTraits = storedAttributes.ToObject<NftTrait[]>();
                            foreach (var trait in storedTraits)
                            {
                                if (trait.trait_type.Equals("level", StringComparison.OrdinalIgnoreCase))
                                    int.TryParse(trait.value, out storedLevel);
                                else if (trait.trait_type.Equals("xp", StringComparison.OrdinalIgnoreCase))
                                    int.TryParse(trait.value, out storedXp);
                            }
                        }
                    }
                    if (level > storedLevel || (level == storedLevel && xp > storedXp))
                    {
                        highestNftsByName[nftName] = nft;
                    }
                }
            }

            // Update current NFT info based on the highest NFTs.
            foreach (var kvp in highestNftsByName)
            {
                if (kvp.Key == "NGMI")
                {
                    nGMINFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    nGMINFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    nGMINFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(nGMINFTCurrentId.ToString()));
                    nGMINFTCurrentUri = result1;
                }
                else if (kvp.Key == "Paper Hands")
                {
                    paperHandsNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    paperHandsNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    paperHandsNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(paperHandsNFTCurrentId.ToString()));
                    paperHandsNFTCurrentUri = result1;
                }
                else if (kvp.Key == "FUD")
                {
                    fUDNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    fUDNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    fUDNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(fUDNFTCurrentId.ToString()));
                    fUDNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Gary")
                {
                    garyNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    garyNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    garyNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(garyNFTCurrentId.ToString()));
                    garyNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Melania")
                {
                    melaniaNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    melaniaNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    melaniaNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(melaniaNFTCurrentId.ToString()));
                    melaniaNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Elon")
                {
                    elonNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    elonNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    elonNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(elonNFTCurrentId.ToString()));
                    elonNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Bitcoin Fly")
                {
                    bitcoinFlyNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    bitcoinFlyNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    bitcoinFlyNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(bitcoinFlyNFTCurrentId.ToString()));
                    bitcoinFlyNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Diamond Hands")
                {
                    diamondHandsNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    diamondHandsNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    diamondHandsNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(diamondHandsNFTCurrentId.ToString()));
                    diamondHandsNFTCurrentUri = result1;
                }
                else if (kvp.Key == "MAGA Hat")
                {
                    mAGAHatNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    mAGAHatNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    mAGAHatNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(mAGAHatNFTCurrentId.ToString()));
                    mAGAHatNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Nuke 'Em")
                {
                    nukeEmNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    nukeEmNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    nukeEmNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(nukeEmNFTCurrentId.ToString()));
                    nukeEmNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Liquidity Injection")
                {
                    liquidityInjectionNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    liquidityInjectionNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    liquidityInjectionNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(liquidityInjectionNFTCurrentId.ToString()));
                    liquidityInjectionNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Airdrop")
                {
                    airdropNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    airdropNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    airdropNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(airdropNFTCurrentId.ToString()));
                    airdropNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Pepe Boost")
                {
                    pepeBoostNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    pepeBoostNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    pepeBoostNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(pepeBoostNFTCurrentId.ToString()));
                    pepeBoostNFTCurrentUri = result1;
                }
                else if (kvp.Key == "USDC Shield")
                {
                    uSDCShieldNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    uSDCShieldNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    uSDCShieldNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(uSDCShieldNFTCurrentId.ToString()));
                    uSDCShieldNFTCurrentUri = result1;
                }
                else if (kvp.Key == "Pump And Dump")
                {
                    pumpAndDumpNFTCurrentId = int.Parse(kvp.Value.Metadata.Id);
                    pumpAndDumpNFTCurrentLevel = GetTraitValueAsInt(kvp.Value, "level");
                    pumpAndDumpNFTCurrentXp = GetTraitValueAsInt(kvp.Value, "xp");
                    var result1 = await contract.ERC721_TokenURI(BigInteger.Parse(pumpAndDumpNFTCurrentId.ToString()));
                    pumpAndDumpNFTCurrentUri = result1;
                }
            }
        }

        // Helper method to extract a trait's value as an integer.
        private int GetTraitValueAsInt(NFT nft, string traitType)
        {
            int value = 0;
            if (nft.Metadata.Attributes != null)
            {
                var attributeArray = nft.Metadata.Attributes as JArray;
                if (attributeArray != null)
                {
                    NftTrait[] traits = attributeArray.ToObject<NftTrait[]>();
                    foreach (var trait in traits)
                    {
                        if (trait.trait_type.Equals(traitType, StringComparison.OrdinalIgnoreCase))
                        {
                            int.TryParse(trait.value, out value);
                            break;
                        }
                    }
                }
            }
            return value;
        }

        // Coroutine to update NFT metadata on the blockchain.
        public IEnumerator UpdateNFTMetadataCoroutine(string nftName, string nftDescription, string nftImageURL, string nftExternalURL, string nftLevel, string nftXp, string nftAnimationURL, string nftURI)
        {
            string nftJson = NFTJsonGenerator.GenerateNFTMetadata(
                name: nftName,
                description: nftDescription,
                image: nftImageURL,
                externalUrl: nftExternalURL,
                backgroundColor: "",
                level: nftLevel,
                xp: nftXp,
                animationUrl: nftAnimationURL
            );

            nftUpdateCount += 1;
            yield return StartCoroutine(GameManager.Instance.webManager.UpdateMetadata(nftURI, nftJson));
        }

        // Batch processes NFT minting or updating for all NFT types.
        public IEnumerator BatchMintUpdateNFTs()
        {
            if (nGMINFTCount > 0) { transactionCount += 1; NGMINFTMint(); }
            if (paperHandsNFTCount > 0) { transactionCount += 1; PaperHandsNFTMint(); }
            if (fUDNFTCount > 0) { transactionCount += 1; FUDNFTMint(); }
            if (garyNFTCount > 0) { transactionCount += 1; GaryNFTMint(); }
            if (melaniaNFTCount > 0) { transactionCount += 1; MelaniaNFTMint(); }
            if (elonNFTCount > 0) { transactionCount += 1; ElonNFTMint(); }
            if (bitcoinFlyNFTCount > 0) { transactionCount += 1; BitcoinFlyNFTMint(); }
            if (diamondHandsNFTCount > 0) { transactionCount += 1; DiamondHandsNFTMint(); }
            if (mAGAHatNFTCount > 0) { transactionCount += 1; MAGAHatNFTMint(); }
            if (nukeEmNFTCount > 0) { transactionCount += 1; NukeEmNFTMint(); }
            if (liquidityInjectionNFTCount > 0) { transactionCount += 1; LiquidityInjectionNFTMint(); }
            if (airdropNFTCount > 0) { transactionCount += 1; AirdropNFTMint(); }
            if (pepeBoostNFTCount > 0) { transactionCount += 1; PepeBoostNFTMint(); }
            if (uSDCShieldNFTCount > 0) { transactionCount += 1; USDCShieldNFTMint(); }
            if (pumpAndDumpNFTCount > 0) { transactionCount += 1; PumpAndDumpNFTMint(); }
            yield return null;
        }

        #endregion

        #region NFT Minting Methods

        // Each method below handles minting or updating NFTs based on current counts.
        public void NGMINFTMint()
        {
            if (nGMINFTCurrentId == -1)
            {
                int nftLevel = (nGMINFTCount / nGMINFTXpThreshold) + 1;
                int nftXP = nGMINFTCount % nGMINFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(nGMINFTName, nGMINFTDescription, nGMINFTImageList[nftLevel - 1], nGMINFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), nGMINFTAnimationUrl));
            }
            else
            {
                int newNFTCount = nGMINFTCount + ((nGMINFTCurrentLevel * nGMINFTXpThreshold) + nGMINFTCurrentXp);
                int nftLevel = (newNFTCount / nGMINFTXpThreshold) + 1;
                int nftXP = newNFTCount % nGMINFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(nGMINFTName, nGMINFTDescription, nGMINFTImageList[nftLevel - 1], nGMINFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), nGMINFTAnimationUrl, nGMINFTCurrentUri));
            }
        }

        public void PaperHandsNFTMint()
        {
            if (paperHandsNFTCurrentId == -1)
            {
                int nftLevel = (paperHandsNFTCount / paperHandsNFTXpThreshold) + 1;
                int nftXP = paperHandsNFTCount % paperHandsNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(paperHandsNFTName, paperHandsNFTDescription, paperHandsNFTImageList[nftLevel - 1], paperHandsNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), paperHandsNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = paperHandsNFTCount + ((paperHandsNFTCurrentLevel * paperHandsNFTXpThreshold) + paperHandsNFTCurrentXp);
                int nftLevel = (newNFTCount / paperHandsNFTXpThreshold) + 1;
                int nftXP = newNFTCount % paperHandsNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(paperHandsNFTName, paperHandsNFTDescription, paperHandsNFTImageList[nftLevel - 1], paperHandsNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), paperHandsNFTAnimationUrl, paperHandsNFTCurrentUri));
            }
        }

        public void FUDNFTMint()
        {
            if (fUDNFTCurrentId == -1)
            {
                int nftLevel = (fUDNFTCount / fUDNFTXpThreshold) + 1;
                int nftXP = fUDNFTCount % fUDNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(fUDNFTName, fUDNFTDescription, fUDNFTImageList[nftLevel - 1], fUDNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), fUDNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = fUDNFTCount + ((fUDNFTCurrentLevel * fUDNFTXpThreshold) + fUDNFTCurrentXp);
                int nftLevel = (newNFTCount / fUDNFTXpThreshold) + 1;
                int nftXP = newNFTCount % fUDNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(fUDNFTName, fUDNFTDescription, fUDNFTImageList[nftLevel - 1], fUDNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), fUDNFTAnimationUrl, fUDNFTCurrentUri));
            }
        }

        public void GaryNFTMint()
        {
            if (garyNFTCurrentId == -1)
            {
                int nftLevel = (garyNFTCount / garyNFTXpThreshold) + 1;
                int nftXP = garyNFTCount % garyNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(garyNFTName, garyNFTDescription, garyNFTImageList[nftLevel - 1], garyNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), garyNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = garyNFTCount + ((garyNFTCurrentLevel * garyNFTXpThreshold) + garyNFTCurrentXp);
                int nftLevel = (newNFTCount / garyNFTXpThreshold) + 1;
                int nftXP = newNFTCount % garyNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(garyNFTName, garyNFTDescription, garyNFTImageList[nftLevel - 1], garyNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), garyNFTAnimationUrl, garyNFTCurrentUri));
            }
        }

        public void MelaniaNFTMint()
        {
            if (melaniaNFTCurrentId == -1)
            {
                int nftLevel = (melaniaNFTCount / melaniaNFTXpThreshold) + 1;
                int nftXP = melaniaNFTCount % melaniaNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(melaniaNFTName, melaniaNFTDescription, melaniaNFTImageList[nftLevel - 1], melaniaNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), melaniaNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = melaniaNFTCount + ((melaniaNFTCurrentLevel * melaniaNFTXpThreshold) + melaniaNFTCurrentXp);
                int nftLevel = (newNFTCount / melaniaNFTXpThreshold) + 1;
                int nftXP = newNFTCount % melaniaNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(melaniaNFTName, melaniaNFTDescription, melaniaNFTImageList[nftLevel - 1], melaniaNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), melaniaNFTAnimationUrl, melaniaNFTCurrentUri));
            }
        }

        public void ElonNFTMint()
        {
            if (elonNFTCurrentId == -1)
            {
                int nftLevel = (elonNFTCount / elonNFTXpThreshold) + 1;
                int nftXP = elonNFTCount % elonNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(elonNFTName, elonNFTDescription, elonNFTImageList[nftLevel - 1], elonNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), elonNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = elonNFTCount + ((elonNFTCurrentLevel * elonNFTXpThreshold) + elonNFTCurrentXp);
                int nftLevel = (newNFTCount / elonNFTXpThreshold) + 1;
                int nftXP = newNFTCount % elonNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(elonNFTName, elonNFTDescription, elonNFTImageList[nftLevel - 1], elonNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), elonNFTAnimationUrl, elonNFTCurrentUri));
            }
        }

        public void BitcoinFlyNFTMint()
        {
            if (bitcoinFlyNFTCurrentId == -1)
            {
                int nftLevel = (bitcoinFlyNFTCount / bitcoinFlyNFTXpThreshold) + 1;
                int nftXP = bitcoinFlyNFTCount % bitcoinFlyNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(bitcoinFlyNFTName, bitcoinFlyNFTDescription, bitcoinFlyNFTImageList[nftLevel - 1], bitcoinFlyNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), bitcoinFlyNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = bitcoinFlyNFTCount + ((bitcoinFlyNFTCurrentLevel * bitcoinFlyNFTXpThreshold) + bitcoinFlyNFTCurrentXp);
                int nftLevel = (newNFTCount / bitcoinFlyNFTXpThreshold) + 1;
                int nftXP = newNFTCount % bitcoinFlyNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(bitcoinFlyNFTName, bitcoinFlyNFTDescription, bitcoinFlyNFTImageList[nftLevel - 1], bitcoinFlyNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), bitcoinFlyNFTAnimationUrl, bitcoinFlyNFTCurrentUri));
            }
        }

        public void DiamondHandsNFTMint()
        {
            if (diamondHandsNFTCurrentId == -1)
            {
                int nftLevel = (diamondHandsNFTCount / diamondHandsNFTXpThreshold) + 1;
                int nftXP = diamondHandsNFTCount % diamondHandsNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(diamondHandsNFTName, diamondHandsNFTDescription, diamondHandsNFTImageList[nftLevel - 1], diamondHandsNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), diamondHandsNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = diamondHandsNFTCount + ((diamondHandsNFTCurrentLevel * diamondHandsNFTXpThreshold) + diamondHandsNFTCurrentXp);
                int nftLevel = (newNFTCount / diamondHandsNFTXpThreshold) + 1;
                int nftXP = newNFTCount % diamondHandsNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(diamondHandsNFTName, diamondHandsNFTDescription, diamondHandsNFTImageList[nftLevel - 1], diamondHandsNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), diamondHandsNFTAnimationUrl, diamondHandsNFTCurrentUri));
            }
        }

        public void MAGAHatNFTMint()
        {
            if (mAGAHatNFTCurrentId == -1)
            {
                int nftLevel = (mAGAHatNFTCount / mAGAHatNFTXpThreshold) + 1;
                int nftXP = mAGAHatNFTCount % mAGAHatNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(mAGAHatNFTName, mAGAHatNFTDescription, mAGAHatNFTImageList[nftLevel - 1], mAGAHatNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), mAGAHatNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = mAGAHatNFTCount + ((mAGAHatNFTCurrentLevel * mAGAHatNFTXpThreshold) + mAGAHatNFTCurrentXp);
                int nftLevel = (newNFTCount / mAGAHatNFTXpThreshold) + 1;
                int nftXP = newNFTCount % mAGAHatNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(mAGAHatNFTName, mAGAHatNFTDescription, mAGAHatNFTImageList[nftLevel - 1], mAGAHatNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), mAGAHatNFTAnimationUrl, mAGAHatNFTCurrentUri));
            }
        }

        public void NukeEmNFTMint()
        {
            if (nukeEmNFTCurrentId == -1)
            {
                int nftLevel = (nukeEmNFTCount / nukeEmNFTXpThreshold) + 1;
                int nftXP = nukeEmNFTCount % nukeEmNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(nukeEmNFTName, nukeEmNFTDescription, nukeEmNFTImageList[nftLevel - 1], nukeEmNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), nukeEmNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = nukeEmNFTCount + ((nukeEmNFTCurrentLevel * nukeEmNFTXpThreshold) + nukeEmNFTCurrentXp);
                int nftLevel = (newNFTCount / nukeEmNFTXpThreshold) + 1;
                int nftXP = newNFTCount % nukeEmNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(nukeEmNFTName, nukeEmNFTDescription, nukeEmNFTImageList[nftLevel - 1], nukeEmNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), nukeEmNFTAnimationUrl, nukeEmNFTCurrentUri));
            }
        }

        public void LiquidityInjectionNFTMint()
        {
            if (liquidityInjectionNFTCurrentId == -1)
            {
                int nftLevel = (liquidityInjectionNFTCount / liquidityInjectionNFTXpThreshold) + 1;
                int nftXP = liquidityInjectionNFTCount % liquidityInjectionNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(liquidityInjectionNFTName, liquidityInjectionNFTDescription, liquidityInjectionNFTImageList[nftLevel - 1], liquidityInjectionNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), liquidityInjectionNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = liquidityInjectionNFTCount + ((liquidityInjectionNFTCurrentLevel * liquidityInjectionNFTXpThreshold) + liquidityInjectionNFTCurrentXp);
                int nftLevel = (newNFTCount / liquidityInjectionNFTXpThreshold) + 1;
                int nftXP = newNFTCount % liquidityInjectionNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(liquidityInjectionNFTName, liquidityInjectionNFTDescription, liquidityInjectionNFTImageList[nftLevel - 1], liquidityInjectionNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), liquidityInjectionNFTAnimationUrl, liquidityInjectionNFTCurrentUri));
            }
        }

        public void AirdropNFTMint()
        {
            if (airdropNFTCurrentId == -1)
            {
                int nftLevel = (airdropNFTCount / airdropNFTXpThreshold) + 1;
                int nftXP = airdropNFTCount % airdropNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(airdropNFTName, airdropNFTDescription, airdropNFTImageList[nftLevel - 1], airdropNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), airdropNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = airdropNFTCount + ((airdropNFTCurrentLevel * airdropNFTXpThreshold) + airdropNFTCurrentXp);
                int nftLevel = (newNFTCount / airdropNFTXpThreshold) + 1;
                int nftXP = newNFTCount % airdropNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(airdropNFTName, airdropNFTDescription, airdropNFTImageList[nftLevel - 1], airdropNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), airdropNFTAnimationUrl, airdropNFTCurrentUri));
            }
        }

        public void PepeBoostNFTMint()
        {
            if (pepeBoostNFTCurrentId == -1)
            {
                int nftLevel = (pepeBoostNFTCount / pepeBoostNFTXpThreshold) + 1;
                int nftXP = pepeBoostNFTCount % pepeBoostNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(pepeBoostNFTName, pepeBoostNFTDescription, pepeBoostNFTImageList[nftLevel - 1], pepeBoostNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), pepeBoostNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = pepeBoostNFTCount + ((pepeBoostNFTCurrentLevel * pepeBoostNFTXpThreshold) + pepeBoostNFTCurrentXp);
                int nftLevel = (newNFTCount / pepeBoostNFTXpThreshold) + 1;
                int nftXP = newNFTCount % pepeBoostNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(pepeBoostNFTName, pepeBoostNFTDescription, pepeBoostNFTImageList[nftLevel - 1], pepeBoostNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), pepeBoostNFTAnimationUrl, pepeBoostNFTCurrentUri));
            }
        }

        public void USDCShieldNFTMint()
        {
            if (uSDCShieldNFTCurrentId == -1)
            {
                int nftLevel = (uSDCShieldNFTCount / uSDCShieldNFTXpThreshold) + 1;
                int nftXP = uSDCShieldNFTCount % uSDCShieldNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(uSDCShieldNFTName, uSDCShieldNFTDescription, uSDCShieldNFTImageList[nftLevel - 1], uSDCShieldNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), uSDCShieldNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = uSDCShieldNFTCount + ((uSDCShieldNFTCurrentLevel * uSDCShieldNFTXpThreshold) + uSDCShieldNFTCurrentXp);
                int nftLevel = (newNFTCount / uSDCShieldNFTXpThreshold) + 1;
                int nftXP = newNFTCount % uSDCShieldNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(uSDCShieldNFTName, uSDCShieldNFTDescription, uSDCShieldNFTImageList[nftLevel - 1], uSDCShieldNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), uSDCShieldNFTAnimationUrl, uSDCShieldNFTCurrentUri));
            }
        }

        public void PumpAndDumpNFTMint()
        {
            if (pumpAndDumpNFTCurrentId == -1)
            {
                int nftLevel = (pumpAndDumpNFTCount / pumpAndDumpNFTXpThreshold) + 1;
                int nftXP = pumpAndDumpNFTCount % pumpAndDumpNFTXpThreshold;
                StartCoroutine(MintNFTCoroutine(pumpAndDumpNFTName, pumpAndDumpNFTDescription, pumpAndDumpNFTImageList[nftLevel - 1], pumpAndDumpNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), pumpAndDumpNFTAnimationUrl));
            }
            else
            {
                int newNFTCount = pumpAndDumpNFTCount + ((pumpAndDumpNFTCurrentLevel * pumpAndDumpNFTXpThreshold) + pumpAndDumpNFTCurrentXp);
                int nftLevel = (newNFTCount / pumpAndDumpNFTXpThreshold) + 1;
                int nftXP = newNFTCount % pumpAndDumpNFTXpThreshold;
                StartCoroutine(UpdateNFTMetadataCoroutine(pumpAndDumpNFTName, pumpAndDumpNFTDescription, pumpAndDumpNFTImageList[nftLevel - 1], pumpAndDumpNFTExternalUrl, nftLevel.ToString(), nftXP.ToString(), pumpAndDumpNFTAnimationUrl, pumpAndDumpNFTCurrentUri));
            }
        }

        #endregion

        #region Engine Wallet Creation

        // Methods to create engine wallets asynchronously.
        public async void CreateEngineWallet1() { engineWallet1 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress1); }
        public async void CreateEngineWallet2() { engineWallet2 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress2); }
        public async void CreateEngineWallet3() { engineWallet3 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress3); }
        public async void CreateEngineWallet4() { engineWallet4 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress4); }
        public async void CreateEngineWallet5() { engineWallet5 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress5); }
        public async void CreateEngineWallet6() { engineWallet6 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress6); }
        public async void CreateEngineWallet7() { engineWallet7 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress7); }
        public async void CreateEngineWallet8() { engineWallet8 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress8); }
        public async void CreateEngineWallet9() { engineWallet9 = await EngineWallet.Create(ThirdwebManager.Instance.Client, engineURL, authToken, backendWalletAddress9); }

        #endregion
    }

    #region Helper NFT Classes and JSON Generator

    [System.Serializable]
    public class NftTrait
    {
        public string trait_type;
        public string value;
    }

    [Serializable]
    public class NFTMetadataJson
    {
        public string name;
        public string description;
        public string image;
        public string animation_url;  // May be null if not provided.
        public string external_url;
        public string background_color;
        public NftTrait[] attributes;
    }

    public static class NFTJsonGenerator
    {
        /// <summary>
        /// Generates NFT metadata JSON.
        /// </summary>
        /// <param name="name">NFT name.</param>
        /// <param name="description">NFT description.</param>
        /// <param name="image">URL to the image.</param>
        /// <param name="externalUrl">External URL for the NFT.</param>
        /// <param name="backgroundColor">Background color.</param>
        /// <param name="level">The level trait value.</param>
        /// <param name="xp">The xp trait value.</param>
        /// <param name="animationUrl">Optional animation URL.</param>
        /// <returns>Formatted JSON string for NFT metadata.</returns>
        public static string GenerateNFTMetadata(
            string name,
            string description,
            string image,
            string externalUrl,
            string backgroundColor,
            string level,
            string xp,
            string animationUrl = null)
        {
            NFTMetadataJson metadata = new NFTMetadataJson
            {
                name = name,
                description = description,
                image = image,
                animation_url = animationUrl,
                external_url = externalUrl,
                background_color = backgroundColor,
                attributes = new NftTrait[2]
            };

            metadata.attributes[0] = new NftTrait { trait_type = "level", value = level };
            metadata.attributes[1] = new NftTrait { trait_type = "xp", value = xp };

            // Convert the metadata object to pretty-printed JSON.
            return JsonUtility.ToJson(metadata, true);
        }
    }

    #endregion
}
