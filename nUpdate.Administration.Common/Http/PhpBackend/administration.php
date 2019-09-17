<?php

$evaluator = new AdministrationEvaluator();
$server = new AuthenticatedServer($evaluator);
$server->reply();