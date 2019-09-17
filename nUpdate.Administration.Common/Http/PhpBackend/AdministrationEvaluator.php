<?php

use Datto\JsonRpc\Evaluator as JsonRpcEvaluator;
use Datto\JsonRpc\Exceptions\ArgumentException;
use Datto\JsonRpc\Exceptions\MethodException;

class AdministrationEvaluator implements JsonRpcEvaluator {

	public function evaluate($method, $arguments)
    {
        if ($method === 'DeleteDirectory') {
            return self::deleteDirectory($arguments);
        }
		else if ($method === 'DeleteFile') {
			return self::deleteFile($arguments);
		}
		else if ($method === 'DirectoryExists') {
			return self::directoryExists($arguments);
		}
		else if ($method === 'FileExists') {
			return self::fileExists($arguments);
		}
		else if ($method === 'List') {
			return self::list($arguments);
		}
		else if ($method === 'MakeDirectory') {
			return self::makeDirectory($arguments);
		}
		else if ($method === 'Rename') {
			return self::rename($arguments);
		}
		else if ($method === 'TestConnection') {
			return self::testConnection();
		}
		else if ($method === 'UploadFile') {
			return self::uploadFile($arguments);
		}
        throw new MethodException();
    }

	public function deleteDirectory($arguments) {
		// Implement
	}

	public function deleteFile($arguments) {
		// Implement
	}

	public function fileExists($arguments) {
		// Implement
	}

	public function list($arguments) {
		// Implement
	}

	public function makeDirectory($arguments) {
		// Implement
	}

	public function rename($arguments) {
		// Implement
	}

	public function testConnection() {
		echo true;
	}

	public function uploadFile($arguments) {
		// Implement
	}
}