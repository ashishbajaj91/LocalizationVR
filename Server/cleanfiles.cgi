#!/usr/bin/python

import cgi
import os
import shutil

userdata_dir = os.path.join('..','userdata')
read_dir = os.path.join('..','read')

def removefile(file):
	os.remove(file)
	return

def removedirectory(directory):
	shutil.rmtree(directory)
	return

def cleandirectory(directory):
	for the_file in os.listdir(directory):
		file_path = os.path.join(directory, the_file)
		try:
			if os.path.isfile(file_path):
				os.unlink(file_path)
			elif os.path.isdir(file_path): 
				shutil.rmtree(file_path)
		except Exception as e:
			print(e)
	return

	
def main():
	cgi.test()
	print "<br>Cleaning the userdata"
	cleandirectory(userdata_dir)
	print "<br>userdata deleted"
	print "<br>Cleaning the read data"
	cleandirectory(read_dir)
	print "<br>readdata deleted"
	return
	
main()
