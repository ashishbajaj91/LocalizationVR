#!/usr/bin/python

import cgi
#import cgitb
import os
from os import walk
from time import gmtime
from time import strftime
import urlparse
import json
import shutil
from datetime import datetime

#cgitb.enable()

data_dir = os.path.join('..','userdata')
read_dir = os.path.join('..','read')

def getcurrenturl():
	url = os.environ['HTTP_HOST']
	uri = os.environ['REQUEST_URI']
	return url + uri

def IsWebUser():
	webuser = False
	if 'REQUEST_METHOD' in os.environ :
		webuser = True
	return webuser
	
def getusername():
	if(IsWebUser()==True):
		if 'REMOTE_USER' in os.environ:
			remoteuser = os.environ['REMOTE_USER']
			user = remoteuser.split("@")[0]
		else:
			user =  GetUserFromLink()
	else:
		user = 'test'
	return user
	
def parseurl(url):
	return urlparse.urlparse(url)
	
def extractrequestsfromurl(url):
	parsed = parseurl(url)
	return urlparse.parse_qs(parsed.query)

def create_user_directory(directory, deleteprevious = False):
	if (deleteprevious == True):
		shutil.rmtree(directory)
	if not os.path.exists(directory):
		os.makedirs(directory)
	return 

def GetUserFromLink():
	user = None
	if(IsWebUser()==True):
		url = getcurrenturl()
		if(CheckIfRequestIsGet() == True):
			values = extractvalueforkeyfromurl(url,'user')
			if (values != None and len(values) > 0):
				user = values[0]
	return user
	
def extractvalueforkeyfromurl(url,key):
	requests = extractrequestsfromurl(url)
	value = None
	if key in requests:
		value = requests[key]
	return value
	
def CreateUserFolder():
	user = getusername()
	directory = os.path.join(read_dir, user)
	create_user_directory(directory)
	return directory

def GetUserFolder():
	user = getusername()
	directory = os.path.join(data_dir, user)
	return directory
	
def CheckIfRequestIsGet():
	isGet=False
	if(os.environ['REQUEST_METHOD'] == 'GET'):
		isGet=True
	return isGet
	

def getfilelist(directory):
	f = []
	for (dirpath, dirnames, filenames) in walk(directory):
		f.extend(filenames)
		break
	return f

def ReadJsonFile(directory,filename):
	file = os.path.join(directory,filename)
	with open(file) as data_file:    
		content = json.load(data_file)
	return content
	
def getCurrentTimeAsString():
	return datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]
	
def MarkRead(directory, filename, timenow):
	reddirectory = CreateUserFolder()
	source = os.path.join(directory,filename)
	fname, ext = os.path.splitext(filename)
	targetfilename = "".join([fname, "_", timenow, ext]);
	target = os.path.join(reddirectory,targetfilename)
	shutil.move(source, target)
	return 

def ReadActionFiles(directory):
	files = getfilelist(directory)
	data = dict()
	count = 0
	timenow = getCurrentTimeAsString()
	for file in files:
		content = ReadJsonFile(directory, file)
		MarkRead(directory, file, timenow)
		data[count] = content
		count += 1
	return data
	
def printJson(json):
	print "<div id='jsondata'>"
	print json
	print "</div>"
	return
	
def main():
	cgi.test()
	directory = GetUserFolder()
	json = ReadActionFiles(directory)
	printJson(json)
	return 
main()