<?php
// UpdateMetadata.php

$apiKey = ""; // Must match Unity

// Check API key
if (!isset($_POST['apiKey']) || $_POST['apiKey'] !== $apiKey) {
    http_response_code(403);
    echo "Unauthorized";
    exit;
}

// Folder where metadata files are stored.
// __DIR__ is the directory containing this script.
$uploadFolder = __DIR__ . "/Metadata/";

// Check for required POST parameters.
if (!isset($_POST['fileUrl']) || !isset($_POST['metadata'])) {
    echo "Missing parameters";
    exit;
}

$fileUrl = $_POST['fileUrl'];
$jsonData = $_POST['metadata'];

// Extract filename from fileUrl (e.g., "nft_metadata_16782010.json")
$filename = basename($fileUrl);
$filepath = $uploadFolder . $filename;

// Check if the file exists.
if (!file_exists($filepath)) {
    echo "File not found";
    exit;
}

// Overwrite the file with the new metadata.
file_put_contents($filepath, $jsonData);

// Return a success message.
echo "Metadata updated successfully: " . $fileUrl;
?>
