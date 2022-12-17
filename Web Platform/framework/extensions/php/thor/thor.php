<?php
    /*
        THOR (Server side XSS defender)
        
        File name: thor.php (Version: 1.5)
        Description: This file contains the THOR extension.
        
        Coded by George Delaportas (G0D)
        Copyright (c) 2014
    */
    
    // Check for direct access
    if (!defined('micro_mvc'))
        exit();
    
    // Validate a string
    function Thor($string, $type)
    {
        if (empty($string) || empty($type))
            return false;
        
        $result = 0;
        
        if ($type === 'general')
        {
            $match = array('\'', ';', ':', '/', '|', '~', '`', '@', 
                           '#', '$', '%', '^', '&', '*', '=', '+', 
                           '[', ']', '\\', ',', '<', '>', '"', '.');
            
            $result = str_replace($match, '', $string);
        }
        else if ($type === 'email')
        {
            $match = array('\'', ';', ':', '/', '|', '~', '`', '#', 
                           '$', '%', '^', '&', '*', '(', ')', '=', '+', 
                           '[', ']', '{', '}', '\\', ',', '<', '>', '"');
            
            $result = str_replace($match, '', $string);
        }
        else if ($type === 'password')
        {
            $match = array('\'', ';', '/', '`', '&', '(', ')', '=', '+', 
                           '[', ']', '{', '}', '\\', ',', '<', '>', '"');
            
            $result = str_replace($match, '', $string);
        }
        else if ($type === 'minimal')
        {
            $match = array('\'', '`', '&', '{', '}', '\\');
            
            $result = str_replace($match, '', $string);
        }
        else
            return false;
        
        if ($string !== $result)
            return false;
        
        return true;
    
    }
?>
