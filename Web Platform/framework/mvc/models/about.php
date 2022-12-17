<?php
    /*
        micro-MVC
        
        File name: about.php
        Description: This file contains the "ABOUT MODEL" class.
        
        Coded by George Delaportas (ViR4X)
        Copyright (C) 2015
    */
    
    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
    
    // ABOUT MODEL class
    class ABOUT_MODEL extends ROOT_MODEL
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
