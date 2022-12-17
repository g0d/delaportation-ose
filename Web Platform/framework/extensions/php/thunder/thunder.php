<?
	/*
		Thunder (REST Service)
		
		File name: thunder.php (Version: 1.2)
		Description: This file contains the Thunder - REST Service.
		
		Coded by George Delaportas (ViR4X)
		Copyright (C) 2017
	*/
	
    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
	
	class THUNDER
	{
		private static $__response_model = array('auth' => false, 'status' => 0, 'data' => null);
		private static $__data_types = array('FILE', 'IMAGE', 'AUDIO', 'TEXT');
		private static $__transfer_modes = array('UP', 'DOWN');
		private static $__operations = array('MOVE' , 'DELETE');
		
		public static function Process($params)
		{
			try
			{
				if (isset($params['auth']))
					self::Authenticate($params['auth']);
				else if (isset($params['data']))
				{
					$hash = $params['data']['hash'];
					$email = self::Authenticate($hash);
					
					if ($email)
					{
						$user_folder = md5($email . $hash);
						
						if (isset($params['data']['operation']))
						{
							$operation = $params['data']['operation'];
							
							if (in_array($operation, self::$__operations))
							{
								$files_list = $params['data']['files'];
								
								if ($operation === 'DELETE')
									$op_result = self::Delete_Files($user_folder, $files_list);
								else
									$op_result = null;
								
								if (!$op_result)
									self::Clear_Status();
								
								self::$__response_model['data'] = null;
							}
						}
						else if (isset($params['data']['check']))
						{
							if ($params['data']['check'] === '0')
							{
								$status_model = $params['data']['status_model'];
								
								//$result = file_put_contents(UTIL::Absolute_Path('delaported/' . $user_folder . '/new_data'), json_encode($status_model));
								
								$result = self::Delayed_Write('delaported/' . $user_folder . '/new_data', json_encode($status_model));
								
								if ($result === false)
									self::Clear_Status();
								else
									self::$__response_model['data'] = 'OK';
							}
							else
							{
								$status_model = file_get_contents(UTIL::Absolute_Path('delaported/' . $user_folder . '/new_data'));
								
								if ($status_model === false)
									self::Clear_Status();
								else
									self::$__response_model['data'] = json_decode($status_model, true);
							}
						}
						else if (isset($params['data']['type']))
						{
							$data_type = $params['data']['type'];
							
							if (in_array($data_type, self::$__data_types))
							{
								$transfer_mode = $params['data']['mode'];
								
								if (in_array($transfer_mode, self::$__transfer_modes))
								{
									if ($transfer_mode === 'UP')
									{
										$new_files_list = $params['data']['files'];
										
										$result = self::Update_Files_Lists($user_folder, $new_files_list);
										
										if ($result === false)
											self::Clear_Status();
										else
											self::$__response_model['data'] = null;
									}
									else
									{
										$current_files_json = file_get_contents(UTIL::Absolute_Path('delaported/' . $user_folder . '/recent_files_list'));
										
										if ($current_files_json === false)
											self::Clear_Status();
										else
											self::$__response_model['data'] = json_decode($current_files_json, true);
									}
								}
							}
						}
						else
							self::Clear_Status();
					}
				}
				
				echo json_encode(self::$__response_model);
			}
			catch (Exception $e)
			{
				UTIL::Log($e, 'error');
				
				self::Clear_Status();
				
				echo json_encode(self::$__response_model);
			}
		}
		
		private static function Authenticate($hash)
		{
			if (empty($hash))
				return false;
			
			$email = self::Auth($hash);
			
			if (!$email)
				self::Clear_Status();
			
			return $email;
		}
		
		private static function Auth($hash)
		{
			if (empty($hash))
				return false;
			
			$email = QUICKY::Login($hash);
			
			if ($email)
			{
				self::$__response_model['auth'] = true;
				self::$__response_model['status'] = 1;
				self::$__response_model['data'] = array('email' => $email,
														'user_folder_id' => md5($email . $hash));
				
				return $email;
			}
			
			return false;
		}
		
		private static function Delayed_Write($path, $data)
		{
			$result = file_put_contents(UTIL::Absolute_Path($path), $data);

			if ($result === false)
			{
				$sleep_time = mt_rand(10000, 100000);
				
				usleep($sleep_time);
				
				$result = file_put_contents(UTIL::Absolute_Path($path), $data);
				
				if ($result === false)
					return false;
			}
			
			return true;
		}
		
		private static function Update_Files_Lists($user_folder, $new_files_list)
		{
			if (empty($user_folder) || empty($new_files_list))
				return false;
			
			//$result = file_put_contents(UTIL::Absolute_Path('delaported/' . $user_folder . '/recent_files_list'), json_encode($new_files_list));
			
			$result = self::Delayed_Write('delaported/' . $user_folder . '/recent_files_list', json_encode($new_files_list));
			
			if ($result === false)
				return false;
			
			$all_files_json = file_get_contents(UTIL::Absolute_Path('delaported/' . $user_folder . '/all_files_list'));
			
			if ($all_files_json === false)
				return false;
			
			$all_files_list = json_decode($all_files_json, true);
			
			if ($all_files_list !== null)
				$updated_files_list = array_merge($all_files_list, $new_files_list);
			else
				$updated_files_list = $new_files_list;
			
			//$result = file_put_contents(UTIL::Absolute_Path('delaported/' . $user_folder . '/all_files_list'), json_encode($updated_files_list));
			
			$result = self::Delayed_Write('delaported/' . $user_folder . '/all_files_list', json_encode($updated_files_list));
			
			return $result;
		}

		private static function Delete_Files($user_folder, $files_list)
		{
			if (empty($user_folder) || empty($files_list))
				return false;
			
			foreach ($files_list as $file)
			{
				$file_path = UTIL::Absolute_Path('delaported/' . $user_folder . '/files/') . $file;
				
				if (!file_exists($file_path))
					continue;
				
				if (is_dir($file_path))
				{
					if (!rmdir($file_path))
						return false;
				}
				else
				{
					if (!unlink($file_path))
						return false;
				}
			}
			
			return true;
		}
		
		private static function Clear_Status()
		{
			self::$__response_model['status'] = 0;
			self::$__response_model['data'] = null;
		}
	}
	
	// ----------------------------
?>
