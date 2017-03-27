# -*- coding: utf-8 -*-
"""
Created on Mon Mar 27 16:21:48 2017

@author: Skyler Lee
"""
import numpy as np
import cv2
def get_centorid(cnt):
    M = cv2.moments(cnt)
    cx, cy = None, None
    if(M['m00'] != 0.0):
        cx = M['m10']/M['m00']
        cy = M['m01']/M['m00']
    return [cx, cy]

def cam():    
    cap = cv2.VideoCapture('8.mp4')
    fgbg = cv2.createBackgroundSubtractorMOG2()
    position = dict()
    position['pos'] = []
    while(1):
        ret, frame = cap.read()
        fgmask = fgbg.apply(frame)
        ret,thresh = cv2.threshold(fgmask,145,255,0)
        im2, contours, hierarchy = cv2.findContours(thresh,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
        #result = im2.shape#(960,544)
        #print("result",result)
        for i in range(len(contours)):
            cnt = contours[i]
            if (cv2.contourArea(cnt) > 80):                
                cv2.drawContours(fgmask, cnt, -1, (255,255,255), 10)
                pos = get_centorid(cnt)
                #print("pos", pos)
                #position['pos'].append(pos)
                #print("dict",position)
        cv2.imshow('frame',fgmask)
        
        k = cv2.waitKey(30) & 0xff
        print("k",k)
        if k == 27:
            break
    cap.release()
    cv2.destroyAllWindows()
    return position
print(cam())
    