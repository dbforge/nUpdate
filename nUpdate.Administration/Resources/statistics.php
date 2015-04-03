<?php

  $dbUrl = "_DBURL";
  $dbUser = "_DBUSER";
  $dbPass = "_DBPASS";
  $dbName = "_DBNAME";

if (isset($_REQUEST['versionid']) && isset($_REQUEST['os'])) // Version-ID and operating system is set
{
  global $dbUrl;
  global $dbUser;
  global $dbPass;
  global $dbName;

  $versionid = trim($_REQUEST['versionid']);
  $os = trim($_REQUEST['os']);

  addEntryToDb($versionid, $os);
}
else
{
  exit("No version-id and/or operating system set.");
}

function addEntryToDb($versionid, $operatingSystem)
{
  global $dbUrl;
  global $dbUser;
  global $dbPass;
  global $dbName;

  date_default_timezone_set("Europe/Germany");
  $date = date("Y-m-d H:i:s");

  /*
  $versionid = mysqli_escape_string($versionid);
  $operatingSystem = mysqli_escape_string($operatingSystem);
  */
  $mysqli = new mysqli($dbUrl, $dbUser, $dbPass, $dbName);
  if ($mysqli->connect_errno) {
     echo "Failed to connect to MySQL: (" . $mysqli->connect_errno . ") " . $mysqli->connect_error;
  }

  if (!($stmt = $mysqli->prepare("INSERT INTO `Download` (`Version_ID`, `DownloadDate`, `OperatingSystem`) VALUES (?, ?, ?)"))) {
     echo "Prepare failed: (" . $mysqli->errno . ") " . $mysqli->error;
  }

  if (!$stmt->bind_param("iss", $versionid, $date, $operatingSystem)) {
    echo "Binding parameters failed: (" . $stmt->errno . ") " . $stmt->error;
  }

  if (!$stmt->execute()) {
    echo "Execute failed: (" . $stmt->errno . ") " . $stmt->error;
  }
}
?>
