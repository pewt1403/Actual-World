import cv2
import numpy as np
import dlib
import socket

def faceDetect():
    cap = cv2.VideoCapture(0)
    detector = dlib.get_frontal_face_detector()
    predictor = dlib.shape_predictor("shape_predictor_68_face_landmarks.dat")
    HOST = "127.0.0.1"
    PORT = 9000

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((HOST,PORT))
        while True:
            _, frame = cap.read()
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY) #Gray for detection : use less power

            faces = detector(gray) # get Rectangle (face coordinate) [(.,.) (.,.)]
            for face in faces:
                x1 = face.left()
                y1 = face.top()
                x2 = face.right()
                y2 = face.bottom()
                #cv2.rectangle(frame, (x1,y1), (x2,y2), (0,255,0), 3)
                landmarks = predictor(gray, face)
                dataOutX = str(landmarks.part(28).x)
                dataOutY =  str(landmarks.part(28).y)
                for i in range(29,68):

                    x = landmarks.part(i).x
                    dataOutX += ',' + str(landmarks.part(i).x)
                    y = landmarks.part(i).y
                    dataOutY += ',' + str(landmarks.part(i).y)
                    #print(x,y)
                    cv2.circle(frame,(x,y), 3, (255,0,0), -1)
                dataOut = dataOutX + "/" + dataOutY + "X"
                print(dataOut)
                s.sendall(bytes(dataOut,'UTF-8'))

            cv2.imshow("Frame", frame)

            key = cv2.waitKey(1)
            if key == 27:
                break

faceDetect()
#print(repr(data))