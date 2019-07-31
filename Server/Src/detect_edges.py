# import the necessary packages
import argparse
import cv2
import os
import time
import sys
import numpy as np



# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-d", "--edge-detector", type=str, required=True,
                help="path to OpenCV's deep learning edge detector")
ap.add_argument("-i", "--image", type=str, required=False,
                help="path to input image")
args = vars(ap.parse_args())


class CropLayer(object):
    def __init__(self, params, blobs):
        # initialize our starting and ending (x, y)-coordinates of the crop
        self.startX = 0
        self.startY = 0
        self.endX = 0
        self.endY = 0

    def getMemoryShapes(self, inputs):
        # the crop layer will receive two inputs -- we need to crop
        # the first input blob to match the shape of the second one,
        # keeping the batch size and number of channels
        (inputShape, targetShape) = (inputs[0], inputs[1])
        (batchSize, numChannels) = (inputShape[0], inputShape[1])
        (H, W) = (targetShape[2], targetShape[3])

        # compute the starting and ending crop coordinates
        self.startX = int((inputShape[3] - targetShape[3]) / 2)
        self.startY = int((inputShape[2] - targetShape[2]) / 2)
        self.endX = self.startX + W
        self.endY = self.startY + H

        # return the shape of the volume (we'll perform the actual
        # crop during the forward pass)
        return [[batchSize, numChannels, H, W]]

    def forward(self, inputs):
        # use the derviced (x, y)-coordinates to perform the crop
        return [inputs[0][:, :, self.startY:self.endY,
                                self.startX:self.endX]]


# load our serialized edge detector from disk
print("[INFO] loading edge detector...")
protoPath = os.path.sep.join([args["edge_detector"],
                             "deploy.prototxt"])
modelPath = os.path.sep.join([args["edge_detector"],
                             "hed_pretrained_bsds.caffemodel"])
net = cv2.dnn.readNetFromCaffe(protoPath, modelPath)

# register our new layer with the model
cv2.dnn_registerLayer("Crop", CropLayer)

if(args["image"] is not None):
    image = cv2.imread(args["image"])
else:
    cam = cv2.VideoCapture(0)
    time.sleep(1.000) 
    ret, image = cam.read()
    if(not ret):
        print("camera capture failed")
        sys.exit()

H, W = image.shape[:2]

# convert the image to grayscale, blur it, and perform Canny
# edge detection
# print("[INFO] performing Canny edge detection...")
# gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
# blurred = cv2.GaussianBlur(gray, (5, 5), 0)
# canny = cv2.Canny(blurred, 30, 150)

# # construct a blob out of the input image for the Holistically-Nested
# # Edge Detector
blob = cv2.dnn.blobFromImage(image, scalefactor=1.0, size=(W, H),
                            mean=(104.00698794, 116.66876762, 122.67891434),
                            swapRB=False, crop=False)

# # set the blob as the input to the network and perform a forward pass
# # to compute the edges
print("[INFO] performing holistically-nested edge detection...")
net.setInput(blob)
hed = net.forward()
hed = cv2.resize(hed[0, 0], (W, H))
hed = (255 * hed).astype("uint8")
ret,hed = cv2.threshold(hed,127,255,cv2.THRESH_TRUNC)

# contourshed, hierarchyhed = cv2.findContours(hed,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
# contourhed = np.zeros((H, W,3), dtype = "uint8")
# cv2.drawContours(contourhed, contourshed, -1, (0,255,0), 1)

# cannycolor =  cv2.cvtColor(canny, cv2.COLOR_GRAY2BGR)
# blob = cv2.dnn.blobFromImage(cannycolor, scalefactor=1.0, size=(W, H),
#                             mean=(104.00698794, 116.66876762, 122.67891434),
#                             swapRB=False, crop=False)
 
# print("[INFO] performing holistically-nested edge detection...")
# net.setInput(blob)
# hedcanny = net.forward()
# hedcanny = cv2.resize(hedcanny[0, 0], (W, H))
# hedcanny = (255 * hedcanny).astype("uint8")


# contourshedoncanny, hierarchyhedoncanny = cv2.findContours(hedcanny,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
# imgcontourhedoncanny = np.zeros((H, W,3), dtype = "uint8")
# cv2.drawContours(imgcontourhedoncanny, contourshedoncanny, -1, (0,255,0), 1)

cannyhed = cv2.Canny(hed,125,255,12)
contourscannyonhed, hierarchycannyonhed = cv2.findContours(cannyhed,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
imgcontourcannyonhed = np.zeros((H, W,3), dtype = "uint8")
cv2.drawContours(imgcontourcannyonhed, contourscannyonhed, -1, (255,255,255), 2)

# cannyoncontours = cv2.Canny(imgcontourcannyonhed,125,255)
# contourscannyoncontours, hierarchycannyoncontours = cv2.findContours(cannyoncontours,cv2.RETR_TREE,cv2.CHAIN_APPROX_SIMPLE)
# imgcontourcannyoncontours = np.zeros((H, W,3), dtype = "uint8")
# cv2.drawContours(imgcontourcannyoncontours, contourscannyoncontours, -1, (255,255,255), 1)

# show the output edge detection result for Canny and
# Holistically-Nested Edge Detection
cv2.imshow("Input", image)
# cv2.imshow("Canny", canny)
cv2.imshow("HED", hed)
# cv2.imshow("HED on Canny", hedcanny)
cv2.imshow("canny on HED", cannyhed)
#cv2.imshow("Contours HED", contourhed)
# cv2.imshow("Contours HED on Canny", imgcontourhedoncanny)
cv2.imshow("Contours Canny on HED", imgcontourcannyonhed)
# cv2.imshow("Contours Canny on Contours", imgcontourcannyoncontours)
# cv2.imwrite('out.jpg',imgcontourcannyonhed)
contour = 0
while True:
    key = cv2.waitKey(0)
    if key==27:    # Esc key to stop
        break
    # cv2.drawContours(contourhed, contourshed, contour, (255,0,0), 1)
    # cv2.imshow("Contours HED", contourhed)
    # cv2.drawContours(imgcontourhedoncanny, contourshedoncanny,contour, (255,0,0), 1)
    # cv2.imshow("Contours HED on Canny", imgcontourhedoncanny)
    # cv2.drawContours(imgcontourcannyonhed, contourscannyonhed, contour, (255,0,0), 1)
    # cv2.imshow("Contours Canny on HED", imgcontourcannyonhed)
    # cv2.imshow("Contours Canny on Contours", imgcontourcannyoncontours)
    cv2.drawContours(imgcontourcannyonhed, contourscannyonhed, contour, (255,0,0), 1)
    cv2.imshow("Contours Canny on HED", imgcontourcannyonhed)
    contour = contour +1


cam.release()
cv2.destroyAllWindows()