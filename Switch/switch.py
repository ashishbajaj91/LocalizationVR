import RPi.GPIO as GPIO
import time
import urllib2
import serial

ser = serial.Serial('/dev/ttyACM0',9600)

def SendGetRequest(urltoopen):
	content = urllib2.urlopen(urltoopen).read()
	return content

def geturl(user="test"):
	url = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/buttonpress.cgi?user=" + user + "&buttonState=True"
	return url
	
def buttonpress():
	url = geturl()
	SendGetRequest(url)
	return
		
def SetupGPIO():
	GPIO.setmode(GPIO.BCM)
	GPIO.setup(18,GPIO.IN, pull_up_down = GPIO.PUD_UP)
	return 

def CheckbuttonPress():
	try:
		while(True):
			print len(ser.readline())
			if (len(ser.readline()) != 0):
				buttonpress()
				print ser.readline()
				print type(ser.readline())
			time.sleep(0.001)
	except:
		print "Cleaning Up"
		GPIO.cleanup()
	return
	
def main():
	CheckbuttonPress()
	return
	
main()
