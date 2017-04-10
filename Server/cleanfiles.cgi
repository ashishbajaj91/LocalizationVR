#!/usr/bin/python

import cgi
import os
import shutil

userdata_dir = os.path.join('..','userdata')
read_dir = os.path.join('..','read')

kinectdata_dir = os.path.join('..','kinectdata')
kinectread_dir = os.path.join('..','kinectread')

buttondata_dir = os.path.join('..','buttonpress')
buttonread_dir = os.path.join('..','buttonpressread')


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
	
	print "<br>Cleaning the kinect data"
	cleandirectory(kinectdata_dir)
	print "<br>kinectdata deleted"

	print "<br>Cleaning the kinectread data"
	cleandirectory(kinectread_dir)
	print "<br>kinectread data deleted"

	print "<br>Cleaning the button data"
	cleandirectory(buttondata_dir)
	print "<br>buttondata deleted"

	print "<br>Cleaning the buttonread data"
	cleandirectory(buttonread_dir)
	print "<br>buttonread data deleted"

	return
	
main()
