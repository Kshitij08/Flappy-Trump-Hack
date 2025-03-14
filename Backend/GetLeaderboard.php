<?php
// Include the database connection settings
require 'ConnectionSettings.php';

// Check for connection errors
if ($conn->connect_error) {
    echo json_encode(array(
        "status" => "error", 
        "message" => "Database connection failed: " . $conn->connect_error
    ));
    exit();
}

// Get the wallet address for the current user from POST data
$walletAddress = filter_input(INPUT_POST, 'walletAddress', FILTER_SANITIZE_STRING);
if (!$walletAddress) {
    echo json_encode(array(
        "status" => "error", 
        "message" => "Invalid input. Wallet address is required."
    ));
    exit();
}

// Query to fetch the top 10 users by score
$leaderboardQuery = "SELECT walletAddress, score FROM userscores ORDER BY score DESC LIMIT 10";
$leaderboardResult = $conn->query($leaderboardQuery);
$leaderboard = array();
if ($leaderboardResult) {
    while ($row = $leaderboardResult->fetch_assoc()) {
        $leaderboard[] = $row;
    }
}

// Get the current user's score
$stmt = $conn->prepare("SELECT score FROM userscores WHERE walletAddress = ?");
$stmt->bind_param("s", $walletAddress);
$stmt->execute();
$stmt->bind_result($userScore);
if (!$stmt->fetch()) {
    // If the wallet address is not found, we return a rank of null
    $userScore = null;
}
$stmt->close();

// Calculate the user's rank if the score exists
$userRank = null;
if ($userScore !== null) {
    // The rank is determined by counting how many users have a higher score than the current user.
    $rankStmt = $conn->prepare("SELECT COUNT(*)+1 AS rank FROM userscores WHERE score > ?");
    $rankStmt->bind_param("i", $userScore);
    $rankStmt->execute();
    $rankStmt->bind_result($rank);
    if ($rankStmt->fetch()) {
        $userRank = $rank;
    }
    $rankStmt->close();
}

// Return the leaderboard and current user's rank as JSON
echo json_encode(array(
    "status" => "success",
    "leaderboard" => $leaderboard,
    "userRank" => $userRank
));

$conn->close();
?>
