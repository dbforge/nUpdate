<?php

class Config {
	
	private static $config = array('user' => '@username@', 'passwordHash' => '@passwordHash@', 'baseDir' => '@baseDir@');

	public static function getValue($key) {
		return $config[$key];
	}
}