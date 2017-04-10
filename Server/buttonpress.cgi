#!/usr/bin/python

import cgi
import os
from time import gmtime
from time import strftime
import urlparse
import json
import shutil
from datetime import datetime

data_dir = os.path.join('..','buttonpress')

def getcurrenturl():
	url = os.environ['HTTP_HOST']
	uri = os.environ['REQUEST_URI']
	return url + uri

def create_user_directory(directory, deleteprevious = False):
	if (deleteprevious == True):
		shutil.rmtree(directory)
	if not os.path.exists(directory):
		os.makedirs(directory)
	return 

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
	directory = os.path.join(data_dir, user)
	create_user_directory(directory)
	return directory
	
def CheckIfRequestIsGet():
	isGet=False
	if(os.environ['REQUEST_METHOD'] == 'GET'):
		isGet=True
	return isGet

def GetAction():
	value = None
	if(IsWebUser()==True):
		url = getcurrenturl()
		if(CheckIfRequestIsGet() == True):
			values = extractvalueforkeyfromurl(url,'buttonState')
			if (values != None and len(values) > 0):
				value = values[0]
	return value
	
def getCurrentTimeAsString():
	return datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]
	
def GetData(Action, time_now):
	data = {}
	data['buttonState'] = Action
	data['Time'] = time_now
	return data
	
def getActionFilename(time_now):
	filename = 'ButtonPress' + '_' + time_now + '.json'
	return filename
	
def SaveJson(data, filename, directory):
	filepath = os.path.join(directory, filename)

	with open(filepath, 'w') as outfile:
		json.dump(data, outfile)
	outfile.close()
		
	return
	
def CreateAndSaveJson(Action, directory):
	time_now =getCurrentTimeAsString()
	data = GetData(Action, time_now)
	filename = getActionFilename(time_now)
	SaveJson(data, filename, directory)
	return
	
def UpdateActionToFile(directory):
	Action = GetAction()
	if (Action != None):
		CreateAndSaveJson(Action, directory)
	return

def main():
	cgi.test()
	directory = CreateUserFolder()
	UpdateActionToFile(directory)
	return
	
main()