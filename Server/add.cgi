#!/usr/bin/python

import cgi
import os
from time import gmtime
from time import strftime
import urlparse
import json
import shutil

data_dir = os.path.join('..','userdata')
backup_dir = os.path.join('..','backup')

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
	backup_directory = os.path.join(backup_dir, user)
	create_user_directory(directory)
	create_user_directory(backup_directory)
	return directory, backup_directory
	
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
			values = extractvalueforkeyfromurl(url,'Action')
			if (values != None and len(values) > 0):
				value = values[0]
	return value
	
def getCurrentTimeAsString():
	return strftime("%Y_%m_%d_%H_%M_%S", gmtime())

def GetData(Action, time_now):
	data = {}
	data['Action'] = Action
	data['Time'] = time_now
	return data
	
def getActionFilename(time_now):
	filename = 'Action' + '_' + time_now + '.json'
	return filename
	
def SaveJson(data, filename, directory, backup_directory):
	filepath = os.path.join(directory, filename)
	backup_file = os.path.join(backup_directory, filename)
	
	with open(filepath, 'w') as outfile:
		json.dump(data, outfile)
	outfile.close()
	
	with open(backup_file, 'w') as backup_outfile:
		json.dump(data, backup_outfile)
	backup_outfile.close()
	
	return
	
def CreateAndSaveJson(Action, directory, backup_directory):
	time_now =getCurrentTimeAsString()
	data = GetData(Action, time_now)
	filename = getActionFilename(time_now)
	SaveJson(data, filename, directory, backup_directory)
	return
	
def UpdateActionToFile(directory, backup_directory):
	Action = GetAction()
	if (Action != None):
		CreateAndSaveJson(Action, directory, backup_directory)
	return

def main():
	cgi.test()
	directory, backup_directory = CreateUserFolder()
	UpdateActionToFile(directory, backup_directory)
	return
	
main()