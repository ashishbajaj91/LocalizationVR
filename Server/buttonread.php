<?php

$userdata_dir = '../buttonpress';
$read_dir = '../buttonpressread';

function join_paths() 
{
    $paths = array();
    foreach (func_get_args() as $arg) 
	{
        if ($arg !== '') 
		{ 
			$paths[] = $arg; 
		}
    }
    return preg_replace('#/+#','/',join('/', $paths));
}

function getreadfilename($filename, $timenow)
{
	$readfilename = basename($filename, ".json") . "_" . $timenow . ".json";
	return $readfilename;
}

function createdirectory($dir)
{
	if(file_exists($dir))
	{
		return;
	}
	mkdir($dir);	
}

function createuserdir($read_dir, $user)
{
	$user_dir = join_paths($read_dir,$user);
	createdirectory($user_dir);
}

function markfileread($userdata_dir, $read_dir, $filename, $timenow, $user)
{
	$readfilename = getreadfilename($filename, $timenow);
	$source = join_paths($userdata_dir, $user, $filename);
	$target = join_paths($read_dir, $user, $readfilename);
	rename( $source, $target);
}

function getfilecontent($userdata_dir, $user, $filename)
{
	$filepath = join_paths($userdata_dir, $user, $filename);
	$content = file_get_contents($filepath);
	return $content;
}

function getfilelist($userdata_dir, $user)
{

	$filepath = join_paths($userdata_dir, $user);
	if (file_exists($filepath))
	{
		$files = scandir($filepath);
		return $files;
	}
	return [];
}

function getuser()
{
	if (isset($_GET['user'])) 
	{
        return $_GET['user'];
    }
	return 'test';
}

function getcurrenttime()
{
	$now = DateTime::createFromFormat('U.u', microtime(true));
	return $now->format("m-d-Y H:i:s.u");	
}

function parsetojson($contents)
{
	return json_encode($contents);
}

function main($userdata_dir, $read_dir)
{
	$user = getuser();
	$files = getfilelist($userdata_dir, $user);
	$no_of_files = count($files);
	if ($no_of_files <= 2)
	{
		return;
	}
	$data = [];
	$timenow = getcurrenttime();
	createuserdir($read_dir, $user);
	for ($i = 2; $i < $no_of_files; $i++) 
	{			
		$content = getfilecontent($userdata_dir, $user, $files[$i]);
		markfileread($userdata_dir, $read_dir, $files[$i], $timenow, $user);
		$data['ele' . ($i-2) ] = json_decode($content);
	}
	print(parsetojson($data));
	return;
}

main($userdata_dir, $read_dir);
?>
