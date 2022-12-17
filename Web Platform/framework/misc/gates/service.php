<?
    UTIL::Load_Extension('thunder', 'php');
    UTIL::Load_Extension('quicky', 'php');
    
    if (!isset($_POST['payload']))
        exit();
    
    $params = json_decode($_POST['payload'], true);
    
    // Use Thunder extension to handle requests
    Thunder::Process($params);
?>
