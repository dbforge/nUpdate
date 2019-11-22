<?php

class ServerItem {
	public $name;
	public $size;
	public $modified;
	public $type;

	function __construct($n, $s, $m, $t) {
		$this->name = $n;
		$this->size = $s;
		$this->modified = $m;
		$this->type = $t;
	}
}