import RPi.GPIO as GPIO
import time

GPIO.setmode(GPIO.BCM)
GPIO.setup(18,GPIO.IN, pull_up_down = GPIO.PUD_UP)
try:
    while(True):
        if(GPIO.input(18) == True):
            print "Pressed"
        time.sleep(0.001)
except:
    print "Cleaning Up"
    GPIO.cleanup()
