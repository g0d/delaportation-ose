<?
	/*
		Quicky (Fast registration & login tool)
		
		File name: quicky.php (Version: 1.8)
		Description: This file contains the Quicky - Fast registration & login tool.
		
		Coded by George Delaportas (ViR4X)
		Copyright (C) 2017
	*/

    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
    
	class QUICKY
	{
		public static function Register($email)
		{
			if (empty($email))
				return false;
			
			if (!filter_var($email, FILTER_VALIDATE_EMAIL))
				return false;
			
			$all_users = scandir(UTIL::Absolute_Path('framework/extensions/php/quicky/users/'));
			
			if ($all_users === false)
				return false;
			
			foreach ($all_users as $user)
			{
				if ($user === '.' || $user === '..')
					continue;
				
				$this_email = file_get_contents(UTIL::Absolute_Path('framework/extensions/php/quicky/users/' . $user));
				
				if (!$this_email)
					return false;
				
				if ($this_email === $email)
					return false;
			}
			
			$user_hash = sha1(time() . $email);
			$delaported_hash = md5($email . $user_hash);
			
			if (!mkdir(UTIL::Absolute_Path('delaported/') . $delaported_hash))
				return false;
			
			if (!mkdir(UTIL::Absolute_Path('delaported/' . $delaported_hash) . '/files'))
				return false;
			
			if (file_put_contents(UTIL::Absolute_Path('delaported/' . $delaported_hash) . '/recent_files_list', null) === false)
				return false;
			
			if (file_put_contents(UTIL::Absolute_Path('delaported/' . $delaported_hash) . '/friends_files_list', null) === false)
				return false;
			
			if (file_put_contents(UTIL::Absolute_Path('delaported/' . $delaported_hash) . '/all_files_list', null) === false)
				return false;
			
			if (file_put_contents(UTIL::Absolute_Path('delaported/' . $delaported_hash) . '/new_data', null) === false)
				return false;
			
			if (file_exists(UTIL::Absolute_Path('framework/extensions/php/quicky/users/' . $user_hash)))
				return false;
			
			if (!file_put_contents(UTIL::Absolute_Path('framework/extensions/php/quicky/users/') . $user_hash, $email))
				return false;
			
			return $user_hash;
		}
		
		public static function Login($user_hash)
		{
			if (empty($user_hash))
				return false;
			
			if (!file_exists(UTIL::Absolute_Path('framework/extensions/php/quicky/users/' . $user_hash)))
				return false;
			
			$email = file_get_contents(UTIL::Absolute_Path('framework/extensions/php/quicky/users/' . $user_hash));
			
			if (!$email)
				return false;
			
			$user = UTIL::Get_Variable('quicky_user');
			
			if (!empty($user))
				return false;
			
			UTIL::Set_Variable('quicky_user', $email);
			
			return $email;
		}
		
		public static function User_Logged_In()
		{
			$user = UTIL::Get_Variable('quicky_user');
			
			if (empty($user))
				return false;
			
			return true;
		}
		
		public static function Logout()
		{
			$user = UTIL::Get_Variable('quicky_user');

			if (empty($user))
				return false;
			
			UTIL::Set_Variable('quicky_user', null);

			return true;
		}
	}

	// ----------------------------
?>
