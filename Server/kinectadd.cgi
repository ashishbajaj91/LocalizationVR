#!/usr/bin/python

import cgi
import os
from time import gmtime
from time import strftime
import urlparse
import json
import shutil
from datetime import datetime

data_dir = os.path.join('..','kinectdata')

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

def GetXZValues():
	value = []
	if(IsWebUser()==True):
		url = getcurrenturl()
		if(CheckIfRequestIsGet() == True):
			actionvalues = GetDX(url)
			factorvalues = GetDZ(url)
			if (actionvalues != None and len(actionvalues) > 0 and factorvalues != None and len(factorvalues) > 0):
				value = [actionvalues[0], factorvalues[0]]
	return value
	
def GetDX(url):
	return extractvalueforkeyfromurl(url,'dx')

def GetDZ(url):
	return extractvalueforkeyfromurl(url,'dz')
	
def getCurrentTimeAsString():
	return datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]
	
def GetData(Coordinate, time_now):
	data = {}
	data['dx'] = Coordinate[0]
	data['dz'] = Coordinate[1]
	data['Time'] = time_now
	return data
	
def getActionFilename(time_now):
	filename = 'Kinect' + '_' + time_now + '.json'
	return filename
	
def SaveJson(data, filename, directory):
	filepath = os.path.join(directory, filename)
	
	with open(filepath, 'w') as outfile:
		json.dump(data, outfile)
	outfile.close()
		
	return
	
def CreateAndSaveJson(Coordinate, directory):
	time_now =getCurrentTimeAsString()
	data = GetData(Coordinate, time_now)
	filename = getActionFilename(time_now)
	SaveJson(data, filename, directory)
	return
	
def UpdateActionToFile(directory):
	Coordinate = GetXZValues()
	if (len(Coordinate) > 0):
		CreateAndSaveJson(Coordinate, directory)
	return

def main():
	cgi.test()
	directory = CreateUserFolder()
	UpdateActionToFile(directory)
	return
	
main()