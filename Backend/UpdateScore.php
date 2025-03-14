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

// Get and sanitize inputs from POST data
$walletAddress = filter_input(INPUT_POST, 'walletAddress', FILTER_SANITIZE_STRING);
$newScore = filter_input(INPUT_POST, 'score', FILTER_VALIDATE_INT);
if (!$walletAddress || $newScore === null) {
    echo json_encode(array(
        "status" => "error", 
        "message" => "Invalid input. Wallet address and score are required."
    ));
    exit();
}

// Check if the wallet address exists
$stmt = $conn->prepare("SELECT id FROM userscores WHERE walletAddress = ?");
$stmt->bind_param("s", $walletAddress);
$stmt->execute();
$stmt->store_result();

if ($stmt->num_rows > 0) {
    $stmt->close();
    // Wallet exists; update the user's score
    $updateStmt = $conn->prepare("UPDATE userscores SET score = ? WHERE walletAddress = ?");
    $updateStmt->bind_param("is", $newScore, $walletAddress);
    if ($updateStmt->execute()) {
        echo json_encode(array(
            "status" => "success", 
            "walletAddress" => $walletAddress, 
            "score" => $newScore,
            "message" => "Score updated successfully."
        ));
    } else {
        echo json_encode(array(
            "status" => "error", 
            "message" => "Failed to update score: " . $updateStmt->error
        ));
    }
    $updateStmt->close();
} else {
    $stmt->close();
    // Wallet not found; insert a new record with the given score
    $insertStmt = $conn->prepare("INSERT INTO userscores (walletAddress, score) VALUES (?, ?)");
    $insertStmt->bind_param("si", $walletAddress, $newScore);
    if ($insertStmt->execute()) {
        echo json_encode(array(
            "status" => "success", 
            "walletAddress" => $walletAddress, 
            "score" => $newScore,
            "message" => "Wallet address added with provided score."
        ));
    } else {
        echo json_encode(array(
            "status" => "error", 
            "message" => "Failed to insert record: " . $insertStmt->error
        ));
    }
    $insertStmt->close();
}

$conn->close();
?>
