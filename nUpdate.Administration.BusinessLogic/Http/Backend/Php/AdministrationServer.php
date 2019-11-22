<?php

use Datto\JsonRpc\Http\Server;
use Config;

class AdministrationServer extends Server
{
    private static $realm = 'nUpdate Administration Remote';

    public function reply()
    {
        if (!self::isAuthenticated()) {
            self::errorUnauthenticated();
        }
        parent::reply();
    }

    private static function isAuthenticated()
    {
        $username = $_SERVER['PHP_AUTH_USER'] ?? null;
        $password = $_SERVER['PHP_AUTH_PW'] ?? null;

        return ($username === Config.getValue('user') && password_verify($password, Config.getValue('passwordHash'));
    }

    private static function errorUnauthenticated()
    {
        header('WWW-Authenticate: Basic realm="'. self::$realm . '"');
        header('HTTP/1.1 401 Unauthorized');
        exit();
    }
}