# Flappy Trump 🚀

![Title Image](https://drive.google.com/uc?export=view&id=13ZLOJ5UTW6exbGmoyisIgU6LqRBO-JkU)

Flappy Trump is an engaging, satirical Web3 arcade game inspired by the classic Flappy Bird, integrated with Monad to offer seamless, fun, and frictionless user experiences. Players defeat crypto-themed enemies, earn tokens, and collect evolving NFTs that reflect their engagement and skills.

---

## 🔧 Project Architecture

Flappy Trump comprises four primary components:

1. **Core Game:**
   - Built using **Unity Engine** for easy cross-platform deployment (Web, Mobile, PC, Telegram).

2. **Backend Database:**
   - Developed with **PHP + MySQL**, hosted on **Hostinger** for cost efficiency and reliable performance.

3. **Wallet & Smart Contract Integration:**
   - Utilizes **Thirdweb** for robust wallet integration and streamlined smart contract interactions, providing gas-free and pop-up-free experiences for users.
   - **$FLAP Contract Address:** `0x3d6A5699eeBeC63cDeCeDb625164Bd57424f13F3`
   - **NFT Contract Address:** `0xb2570aB0c9e1dBac805C40E78121d61BFa4eEAaa`

4. **Web Hosting & Asset Optimization:**
   - Deployed via **Vercel** for rapid delivery and automatic deployments.
   - Optimized content delivery and bandwidth management through **Cloudflare R2** and custom caching rules.

---

## 🎯 Core Functionality & Game Flow

- **Wallet Connection:** Players connect their wallets via Thirdweb Unity SDK.
- **User Registration:** Automatically register and initialize player scores in MySQL.
- **NFT Integration:** Fetch player's NFT collection to determine enemy levels and point multipliers.
- **Gameplay:** Players defeat enemies to earn soulbound `$FLAP` tokens.
- **Game Over Logic:** Automatically execute token claims and NFT minting/upgrades without user pop-ups or gas fees.
- **Summary:** Provide detailed summaries post-game, showcasing tokens earned, NFTs evolved, and transaction stats.

---

## 🚀 Technologies & Dependencies

- **Unity Editor (2022 or higher)** for game development
- **PHP/MySQL** for backend management
- **Thirdweb SDK** for seamless blockchain interactions
- **Vercel & Cloudflare R2** for optimized asset hosting and delivery

---

## 🌟 Leveraging Monad's Accelerated EVM

Flappy Trump extensively leverages **Monad's accelerated EVM**, significantly enhancing transaction throughput and reducing transaction latency:

- Processed **252k transactions** by **31k users** with less than **5 seconds per transaction**.
- Minted **220k NFTs** and distributed **6.73M $FLAP tokens** seamlessly without user gas fees or transaction pop-ups.
- Monad's robust performance enabled handling high-volume simultaneous transactions using just **9 backend wallets**, greatly improving user experience and scalability.

---

## 📋 How to Run

### Prerequisites
- Unity Editor (2022 or newer)

### Steps:
1. Clone this repository.
2. Open the cloned project using Unity Editor.
3. Navigate to `File -> Build Settings -> Build and Run` to build and deploy the game.

### ⚠️ Authentication Keys
- Authentication keys required for Backend & Thirdweb integrations are not publicly included.
- **Contact:** [gajapure.kshitij@gmail.com](mailto:gajapure.kshitij@gmail.com) or via Telegram [@Kshitij_Gajapure](https://t.me/Kshitij_Gajapure).

---

## 📂 Commit History
Commit history maintained using PlasticSCM. (Screenshot below.)
![Plastic SCM Version History 2](https://drive.google.com/uc?export=view&id=15346f72w0REnWkPcvLczFY8vdZ5ylX41)
![Plastic SCM Version History 1](https://drive.google.com/uc?export=view&id=1LR6jPiNEvT5kqXmm9g2ix0_5TWGtu25m)

---

## 👤 Team & Contributions
- **Kshitij Gajapure:** Game Design, Unity Development, Backend management, Smart Contract integration, 2D Art.

---

## 📝 Reflection & Learnings
- Efficiently managed sudden high-traffic scenarios (600GB data within 5 hours).
- Implemented Cloudflare R2 & custom caching strategies, reducing daily hosting costs from $368.64 to $15.36, ensuring sustainability.

---

## 📹 Demo Video
A short, narrated demonstration (max 5 minutes) showcasing gameplay, blockchain integrations, and core features will be available.

---

## 🌐 Live Demo
Check out Flappy Trump live [here](https://flappy-trump-beta.vercel.app/).
