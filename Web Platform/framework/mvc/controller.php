<?php
    /*
        micro-MVC
        
        File name: controller.php
        Description: This file contains the "MVC CONTROLLER" class.
        
        Coded by George Delaportas (ViR4X)
        Copyright (C) 2015
    */
    
    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
    
    // MVC CONTROLLER class
    class MVC_CONTROLLER
    {
        public static function root()
        {
            $result = ROOT_MODEL::Get_Data();
            MVC::Store_Content('root', $result);
            
            return true;
        }
        
        // Features route
        public static function features()
        {
            $result = FEATURES_MODEL::Get_Data();
            MVC::Store_Content('features', $result);
            
            return true;
        }

        // About route
        public static function about()
        {
            $result = ABOUT_MODEL::Get_Data();
            MVC::Store_Content('about', $result);
            
            return true;
        }

        // Download route
        public static function download()
        {
            $result = DOWNLOAD_MODEL::Get_Data();
            MVC::Store_Content('download', $result);
            
            return true;
        }
    }
?>
