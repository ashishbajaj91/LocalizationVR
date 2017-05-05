import win32api
import time
from urllib.request import urlopen
 
user = 'ashishb'

def SendGetRequest(urltoopen):
	content = urlopen(urltoopen).read()
	return content

def geturlforButtonPress():
    global user
    url = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/buttonpress.cgi?user=" + user + "&buttonState=True"
    return url
	
def buttonpress():
	url = geturlforButtonPress()
	SendGetRequest(url)
	return

def geturlforKinectMotion(dx, dz):
    global user
    url = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/kinectadd.cgi?user=" + user + "&dx=" + str(dx) + "&dz=" + str(dz)
    return url

def kinectMotion(dx, dz):
    url = geturlforKinectMotion(dx, dz)
    SendGetRequest(url)
    return
     
cenX = int(1364/2);
cenY = int(766/2);
          
while True:
    x, y = win32api.GetCursorPos()
    if(x!=cenX or y != cenY):
        dx = x-cenX
        dy = y-cenY
        kinectMotion(dx, dy)
        print (dx,dy)
    
    win32api.SetCursorPos((cenX,cenY))
    
    left_click = win32api.GetKeyState(0x01)
    if ( left_click != 0 and left_click != 1):
        buttonpress()
        print('buttonPress')
    
    right_click = win32api.GetKeyState(0x02)
    if (right_click != 0 and right_click != 1):
        break
    
    time.sleep(0.1)
print('Breaking out')            