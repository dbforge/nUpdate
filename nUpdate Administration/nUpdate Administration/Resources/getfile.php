<?php

  $dbUrl = "";
  $dbUser = "";
  $dbPass = "";
  $dbName = "";
  
if (isset($_REQUEST['versionid'])) // Version-ID is set
{
  global $dbUrl;
  global $dbUser;
  global $dbPass;
  global $dbName;

  $versionid = trim($_REQUEST['versionid']);
  addEntryToDb($versionid);
}
else
{
  exit("No version-id set.");
}

function addEntryToDb($versionid)
{
  global $dbUrl;
  global $dbUser;
  global $dbPass;
  global $dbName;
  
  date_default_timezone_set("Europe/Germany");
  $date = date("Y-m-d H:i:s");   
  
  $mysqli = new mysqli($dbUrl, $dbUser, $dbPass, $dbName);
  if ($mysqli->connect_errno) {
     echo "Failed to connect to MySQL: (" . $mysqli->connect_errno . ") " . $mysqli->connect_error;
  }
  
  if (!($stmt = $mysqli->prepare("INSERT INTO `Download` (`Version_ID`, `DownloadDate`) VALUES (?, ?)"))) {
     echo "Prepare failed: (" . $mysqli->errno . ") " . $mysqli->error;
  }
  
  if (!$stmt->bind_param("is", $versionid, $date)) {
    echo "Binding parameters failed: (" . $stmt->errno . ") " . $stmt->error;
  }
  
  if (!$stmt->execute()) {
    echo "Execute failed: (" . $stmt->errno . ") " . $stmt->error;
  }
}
?>