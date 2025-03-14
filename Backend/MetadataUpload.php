<?php
// MetadataUpload.php

$apiKey = ""; // Must match the one in Unity

if (!isset($_POST['apiKey']) || $_POST['apiKey'] !== $apiKey) {
    http_response_code(403);
    echo "Unauthorized";
    exit;
}

// Folder to store metadata files.
$uploadFolder = __DIR__ . "/Metadata/";

// Check if metadata was provided.
if (isset($_POST['metadata'])) {
    $jsonData = $_POST['metadata'];
    
    // Check for a custom string.
    $customString = "";
    if (isset($_POST['custom'])) {
        // Sanitize the custom string: allow only letters, numbers, underscores, and hyphens.
        $customString = preg_replace("/[^A-Za-z0-9_-]/", "", $_POST['custom']);
    }
    
    // Create a filename using the custom string and the current time.
    // For example: nft_metadata_customString_16782010.json
    $filename = "nft_metadata_" . $customString . "_" . time() . ".json";
    
    // Write the JSON data to the file.
    file_put_contents($uploadFolder . $filename, $jsonData);
    
    // Return the file URL.
    $fileUrl = "https://plum-coyote-976718.hostingersite.com/Metadata/" . $filename;
    echo $fileUrl;
} else {
    echo "No 'metadata' field found in POST";
}
?>
