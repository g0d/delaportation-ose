<?php
    /*
        micro-MVC
        
        File name: download.php
        Description: This file contains the "DOWNLOAD MODEL" class.
        
        Coded by George Delaportas (ViR4X)
        Copyright (C) 2015
    */
    
    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
    
    // DOWNLOAD MODEL class
    class DOWNLOAD_MODEL extends ROOT_MODEL
    {
        public static function Get_Data()
        {
            if (LANG::Get('this') === 'en')
            {
                return '';
            }
            else
            {
                return '';
            }
        }
    }
?>
