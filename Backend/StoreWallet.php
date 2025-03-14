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

// Get the wallet address from POST data and sanitize it
$walletAddress = filter_input(INPUT_POST, 'walletAddress', FILTER_SANITIZE_STRING);
if (!$walletAddress) {
    echo json_encode(array(
        "status" => "error", 
        "message" => "Invalid input. Wallet address is required."
    ));
    exit();
}

// Prepare a statement to check if the wallet address exists
$stmt = $conn->prepare("SELECT score FROM userscores WHERE walletAddress = ?");
if (!$stmt) {
    echo json_encode(array(
        "status" => "error", 
        "message" => "Failed to prepare the SQL statement."
    ));
    exit();
}
$stmt->bind_param("s", $walletAddress);
$stmt->execute();
$stmt->store_result();

// If the wallet exists, return the score; otherwise, add a new record with score 0.
if ($stmt->num_rows > 0) {
    $stmt->bind_result($score);
    $stmt->fetch();
    echo json_encode(array(
        "status" => "success", 
        "walletAddress" => $walletAddress, 
        "score" => $score
    ));
    $stmt->close();
} else {
    $stmt->close();
    $insertStmt = $conn->prepare("INSERT INTO userscores (walletAddress, score) VALUES (?, 0)");
    if (!$insertStmt) {
        echo json_encode(array(
            "status" => "error", 
            "message" => "Failed to prepare insert statement."
        ));
        exit();
    }
    $insertStmt->bind_param("s", $walletAddress);
    if ($insertStmt->execute()) {
        echo json_encode(array(
            "status" => "success", 
            "walletAddress" => $walletAddress, 
            "score" => 0,
            "message" => "Wallet address added with initial score 0."
        ));
    } else {
        echo json_encode(array(
            "status" => "error", 
            "message" => "Failed to add wallet address: " . $insertStmt->error
        ));
    }
    $insertStmt->close();
}

$conn->close();
?>
