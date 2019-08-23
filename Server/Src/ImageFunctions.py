import cv2
import numpy as np


def merge(*args):
    if(len(args) < 2):
        return args
    ret = args[0]
    for arr in args[1:]:
        ret = np.bitwise_and(ret, arr)
    return ret


def merge2(*args):
    if(len(args) < 2):
        return args
    ret = args[0]
    for arr in args[1:]:
        ret = cv2.addWeighted(ret, 1-(1/len(args)), arr, 1/len(args), 0)
    return cv2.normalize(ret, None, 0, 255, cv2.NORM_MINMAX)


def getChannel(img, channel):
    chan = np.zeros(img.shape, dtype="uint8")
    chan[:, :, channel] = img[:, :, channel]
    return chan


def getFlatChannel(img, channel):
    return img[:, :, channel]


def cannyChannel(img, channel):
    chan = getChannel(img, channel)
    return canny(chan)


def cartoonEdges(img):

    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    gray = cv2.medianBlur(gray, 5)
    edges = cv2.adaptiveThreshold(gray, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 9, 2)
    return edges


def cartoon(img):
    # 1) Edges
    edges = cartoonEdges(img)
    # 2) Color
    img = cv2.medianBlur(img, 5)
    color = cv2.bilateralFilter(img, 9, 300, 300)
    # 3) Cartoon
    return cv2.bitwise_and(color, color, mask=edges)


def canny(img):

    original = img
    sigma = 0.1
    if len(img.shape) == 3:
        img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    v = np.median(img)

    # apply automatic Canny edge detection using the computed median
    lower = 0  # int(max(0, (1.0 - sigma) * v))
    upper = 10  # int(min(255, (1.0 + sigma) * v))

    chancanny = cv2.Canny(img, lower, upper)
    #img = np.zeros(img.shape, dtype="uint8")
    img = np.bitwise_or(original, cv2.cvtColor(chancanny, cv2.COLOR_GRAY2BGR))
    return (img, chancanny)


def contourChannel(img, channel, color):

    cannyimg, canny = cannyChannel(img, channel)
    chancontours, hier = cv2.findContours(
        cv2.cvtColor(canny, cv2.COLOR_BGR2GRAY),
        cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    imgcontours = np.zeros(img.shape, dtype="uint8")
    cv2.drawContours(imgcontours, chancontours, -1, color, 1)

    return (imgcontours, chancontours)


def getContours(image, color=(0, 255, 0), draw=True):

    if len(image.shape) != 2:
        image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    contours, hier = cv2.findContours(
        image,
        cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

    imgcontours = np.zeros(image.shape, dtype="uint8")
    if draw:
        cv2.cvtColor(imgcontours, cv2.COLOR_GRAY2BGR)
        cv2.drawContours(imgcontours, contours, -1, color, 1)

    return (imgcontours, contours)
