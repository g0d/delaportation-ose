<?
	/*
		Dragon (User-defined MVC routes dispatcher)
		
		File name: dragon.php
		Description: This file contains the Dragon - User-defined MVC routes dispatcher.
		
		Coded by George Delaportas (ViR4X)
		Copyright (C) 2016
	*/

    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
	
	UTIL::Load_Extension('quicky', 'php');
	
	$this_lang = LANG::Get('this');
	$this_route = MVC::Get_Route('this');
	
	if (QUICKY::User_Logged_In())
	{
		if ($this_route === 'login' || $this_route === 'register')
			header('Location: /' . $this_lang . '/');
	}
	else
	{
		if ($this_route === 'dashboard' || $this_route === 'logout')
			header('Location: /' . $this_lang . '/');
	}
	
	unset($this_lang);
	unset($this_route);
	// ----------------------------
?>
