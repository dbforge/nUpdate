<?php

use Config;
use Datto\JsonRpc\Evaluator as JsonRpcEvaluator;
use Datto\JsonRpc\Exceptions\MethodException;

class AdministrationEvaluator implements JsonRpcEvaluator
{
    public function evaluate($method, $arguments) {
        if ($method === 'DeleteDirectory') {
            return self::deleteDirectory($arguments);
        } else if ($method === 'DeleteFile') {
            return self::deleteFile($arguments);
        } else if ($method === 'DirectoryExists') {
            return self::directoryExists($arguments);
        } else if ($method === 'FileExists') {
            return self::fileExists($arguments);
        } else if ($method === 'List') {
            return self::list($arguments);
        } else if ($method === 'MakeDirectory') {
            return self::makeDirectory($arguments);
        } else if ($method === 'Rename') {
            return self::rename($arguments);
        } else if ($method === 'TestConnection') {
            return self::testConnection();
        } else if ($method === 'UploadFile') {
            return self::uploadFile($arguments);
        }
        throw new MethodException();
    }

    private function deleteDirectory($directoryPath) {
        $baseDirectory = Config . getValue('baseDir');
        $path = $base_directory . $directoryPath;

        if (!file_exists($path)) {
            return;
        }

        if (!deleteDirectoryInternal($path)) {
            throw new ApplicationException('Unknown error while deleting directory at ' . $path . '. Please check your error log for details.');
        }
    }

    private function deleteDirectoryInternal($dir) {
        if (!file_exists($dir)) {
            return true;
        }

        if (!is_dir($dir)) {
            return unlink($dir);
        }

        foreach (scandir($dir) as $item) {
            if ($item == '.' || $item == '..') {
                continue;
            }

            if (!deleteDirectory($dir . DIRECTORY_SEPARATOR . $item)) {
                return false;
            }

        }

        return rmdir($dir);
    }

    private function deleteFile($filePath) {
        $baseDirectory = Config . getValue('baseDir');
        $path = $base_directory . $filePath;

        if (!is_dir($path)) {
            return;
        }

        if (!rmdir($path)) {
            throw new ApplicationException('Unknown error while deleting file at ' . $path . '. Please check your error log for details.');
        }
    }

	private function directoryExists($directoryPath) {
		$baseDirectory = Config . getValue('baseDir');
        $path = $base_directory . $directoryPath;
		return file_exists($path) && is_dir($path);
	}

    private function fileExists($filePath) {
		$baseDirectory = Config . getValue('baseDir');
        $path = $base_directory . $filePath;
		return file_exists($path) && !is_dir($path);
    }

    function list($directoryPath) {
		$result = array();
		$items = array_diff(scandir($directoryPath), array('.', '..'));
		foreach ($items as $item) {
			$itemPath = $directoryPath . $item;
			array_push($result, new ServerItem($item, filemtime($itemPath), filesize($itemPath), filetype($itemPath)))
		}
		return $result;
    }

    private function makeDirectory($arguments)
    {
        // Implement
    }

    private function rename($arguments)
    {
        // Implement
    }

    private function testConnection()
    {
		return true;
    }

    private function uploadFile($arguments)
    {
        // Implement
    }
}
