import cv2
import numpy as np


class ClusterRef:

    @property
    def rect(self):
        return self.__rect

    @property
    def contours(self):
        return self.__contours

    @property
    def clusterColor(self):
        return self.__clusterColor

    @clusterColor.setter
    def clusterColor(self, val):
        self.__clusterColor = val

    @property
    def clusterNumber(self):
        return self.__clusterNumber

    @clusterNumber.setter
    def clusterNumber(self, val):
        self.__clusterNumber = val

    def __init__(self, rect, clusterColor, contours):
        self.__rect = rect
        self.__clusterColor = clusterColor
        self.__clusterNumber = -1
        self.__contours = contours


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


def area(rect):
    size = rect[0]
    return size[0] * size[1]


def cannyChannel(img, channel):

    min = 0
    max = 20

    chan = getChannel(img, channel)
    chancanny = cv2.cvtColor(cv2.Canny(chan, min, max), cv2.COLOR_GRAY2BGR)
    chan = np.bitwise_or(chan, chancanny)
    return (chan, chancanny)


def contourChannel(img, channel, color):

    cannyimg, canny = cannyChannel(img, channel)
    chancontours, hier = cv2.findContours(
        cv2.cvtColor(canny, cv2.COLOR_BGR2GRAY),
        cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    imgcontours = np.zeros(img.shape, dtype="uint8")
    cv2.drawContours(imgcontours, chancontours, -1, color, 1)

    return (imgcontours, chancontours)


def clusterContours(img, contours):

    def getPoint(c):
        (H, W) = np.vstack(c).squeeze()
        return (W, H)

    clusterRefs = list(map(lambda c: ClusterRef(cv2.minAreaRect(c), img[getPoint(c[0])], c), contours))
    clusternr = 0
    clusters = []
    for current in clusterRefs:
        intersections = list(
            filter(
                lambda c: c != current and
                cv2.rotatedRectangleIntersection(c.rect, current.rect) != cv2.INTERSECT_NONE and
                current.clusterColor == c.clusterColor, clusterRefs)
        )
        intersections.sort(key=lambda i: area(i.rect))

        if len(intersections) == 0:
            # this one gets a new cluster
            current.clusterNumber = clusternr
            clusternr = clusternr + 1
            clusters.append([current.contours])
        else:
            intersection = intersections[0]
            if intersection.clusterNumber == -1 and current.clusterNumber == -1:
                # intersection with 2 new clusters
                intersection.clusterNumber = clusternr
                current.clusterNumber = clusternr
                clusters.append([current.contours])
                clusters[clusternr].append(intersection.contours)
                clusternr = clusternr + 1
            elif current.clusterNumber == -1 and intersection.clusterNumber > -1:
                # new cluster gets merged with old cluster
                current.clusterNumber = intersection.clusterNumber
                clusters[intersection.clusterNumber].append(current.contours)
            elif current.clusterNumber > -1 and intersection.clusterNumber == -1:
                # new cluster gets merged with old cluster
                intersection.clusterNumber = current.clusterNumber
                clusters[current.clusterNumber].append(intersection.contours)
            else:
                # 2clusters merged
                clusters[current.clusterNumber].append(clusters[intersection.clusterNumber])
                clusters[intersection.clusterNumber] = None

                intersection.clusterNumber = current.clusterNumber

    clusters = filter(lambda c: c is not None, clusters)
    imgContours = np.zeros(img.size, dtype="uint8")
    cv2.drawContours(imgContours, contours, -1, (125, 125, 125), 1)
    for cluster in clusters:
        rect = cv2.minAreaRect(np.vstack(cluster).squeeze())
        box = cv2.boxPoints(rect)
        box = np.int0(box)
        cv2.drawContours(imgContours, [box], 0, (0, 0, 255), 1)
    return (imgContours, clusters)


def runCode():
    cap = cv2.VideoCapture(0)

    previouscanny = None
    while(True):
        # Capture frame-by-frame
        ret, image = cap.read()
        image = cv2.medianBlur(image, 9)
        # image = cv2.blur(image,(5,5))

        # hsv = cv2.cvtColor(image,cv2.COLOR_BGR2HSV)

        # hue,huecanny = cannyChannel(hsv,0)
        # sat, satcanny = cannyChannel(hsv,1)
        # val , valcanny =  cannyChannel(hsv,2)

        # hls = cv2.cvtColor(image,cv2.COLOR_BGR2HLS)

        # hue2, hue2canny = cannyChannel(hls,0)
        # light ,lightcanny =  cannyChannel(hls,1)
        # sat2 , sat2canny =  cannyChannel(hls,2)

        blue, bluecanny = cannyChannel(image, 0)
        imgbluecontours, bluecontours = contourChannel(image, 0, (255, 0, 0))
        bluechannelimg = getFlatChannel(image, 0)
        imgblueroi, clusters = clusterContours(bluechannelimg, bluecontours)
        green, greencanny = cannyChannel(image, 1)
        # imggreencontours , greencontours = contourChannel(image,1,(0,255,0))
        red,  redcanny = cannyChannel(image, 2)
        # imgredcontours ,  redcontours = contourChannel(image,2,(0,0,255))

        # bluecanny,huecanny,satcanny,valcanny,hue2canny,sat2canny,lightcanny)
        totalcanny = merge2(redcanny, greencanny)

        if(previouscanny is None):
            previouscanny = totalcanny

        totalcanny = merge2(totalcanny, previouscanny)

        previouscanny = totalcanny
        ret, totalcanny = cv2.threshold(totalcanny, 0, 255, cv2.THRESH_TRUNC)

        image = np.bitwise_or(image, totalcanny)

        cv2.imshow('Image', image)
        cv2.imshow('Blue', blue)
        cv2.imshow('Green', green)
        cv2.imshow('Red', red)
        cv2.imshow('Blue contours', imgbluecontours)
        cv2.imshow('Blue clusterd', imgblueroi)
    #  cv2.imshow('Green contours',imggreencontours)
    # cv2.imshow('Red contours',imgredcontours)
        cv2.imshow("Canny", totalcanny)
        # cv2.imshow('Hue',hue)
        # cv2.imshow('Sat',sat)
        # cv2.imshow('Val',val)
        # cv2.imshow('Hue2',hue2)
        # cv2.imshow('Light',light)
        # cv2.imshow('Sat2',sat2)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    # When everything done, release the capture
    cap.release()
    cv2.destroyAllWindows()


runCode()
