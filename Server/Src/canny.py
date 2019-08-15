import cv2
import sys
import numpy as np
import multiprocessing as mp
import time


class Rect:

    def __init__(self, x, y, w, h):
        self.x = x
        self.y = y
        self.w = w
        self.h = h
        self.max_x = x+w
        self.max_y = y+h
        self.area = w * h


class ClusterRef:

    @property
    def rect(self):
        return self.__rect

    @property
    def contours(self):
        return self.__contours

    def appendContours(self, contours):
        if not isinstance(contours, list):
            contours = [contours]
        points = []

        for c in contours:
            self.__contours.append(c)
        for contour in self.__contours:
            (x, y, w, h) = cv2.boundingRect(contour)
            points.append([x, y])
            points.append([x+w, y+h])
        rect = cv2.boundingRect(np.array(points).astype(int))

        self.__rect = Rect(*rect)

    @property
    def clusterColor(self):
        return self.__clusterColor

    @clusterColor.setter
    def clusterColor(self, val):
        self.__clusterColor = val

    @property
    def clusterNumber(self):
        return self.__clusterNumber

    def intersect(self, other):
        rect = self.rect
        other = other.rect

        if rect.x > other.max_x or rect.max_x < other.x:
            return False
        if rect.y > other.max_y or rect.max_y < other.y:
            return False
        return True

    def dist(self, other):

        compareColor = not hasattr(self.__clusterColor, "__len__")

        if (compareColor and self.__clusterColor != other.clusterColor) or not self.intersect(other):
            return sys.maxsize
        return max(self.rect.area - other.rect.area, 0)

    @clusterNumber.setter
    def clusterNumber(self, val):
        self.__clusterNumber = val

    def __init__(self, clusterColor, contours):
        self.__clusterColor = clusterColor
        self.__clusterNumber = -1
        self.__contours = []
        self.appendContours(contours)


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

    min = 0
    max = 20

    chan = getChannel(img, channel)
    chancanny = cv2.cvtColor(cv2.Canny(chan, min, max), cv2.COLOR_GRAY2BGR)
    chan = np.bitwise_or(chan, chancanny)
    return (chan, chancanny)


def canny(chanimg):

    min = 0
    max = 20
    if len(chanimg.shape) == 3:
        cv2.cvtColor(chanimg, cv2.COLOR_BGR2GRAY)
    chancanny = cv2.Canny(chanimg, min, max)
    chanimg = np.bitwise_or(chanimg, chancanny)
    return (cv2.cvtColor(chanimg, cv2.COLOR_GRAY2BGR), chancanny)


def contourChannel(img, channel, color):

    cannyimg, canny = cannyChannel(img, channel)
    chancontours, hier = cv2.findContours(
        cv2.cvtColor(canny, cv2.COLOR_BGR2GRAY),
        cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    imgcontours = np.zeros(img.shape, dtype="uint8")
    cv2.drawContours(imgcontours, chancontours, -1, color, 1)

    return (imgcontours, chancontours)


def getContours(chanimg, color):

    if len(chanimg.shape) != 2:
        chanimg = cv2.cvtColor(chanimg, cv2.COLOR_BGR2GRAY)

    chancontours, hier = cv2.findContours(
        chanimg,
        cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    imgcontours = np.zeros(chanimg.shape, dtype="uint8")
    cv2.cvtColor(imgcontours, cv2.COLOR_GRAY2BGR)
    cv2.drawContours(imgcontours, chancontours, -1, color, 1)

    return (imgcontours, chancontours)


def generateClusterRefs(img, contours):
    def getPoint(c):
        (W, H) = np.vstack(c).squeeze()
        return (W, H)

    def getClusterRef(img, c):
        (W, H) = getPoint(c[0])
        color = img[H, W]
        return ClusterRef(color, c)

    return list(map(lambda c: getClusterRef(img, c), contours))


def drawClusters(img, clusters):
    imgContours = img.copy()
    if len(imgContours.shape) != 3:
        imgContours = cv2.cvtColor(imgContours, cv2.COLOR_GRAY2BGR)
    for cluster in clusters:
        cv2.drawContours(imgContours, cluster.contours, -1, (255, 0, 0))
        rect = cluster.rect
        cv2.rectangle(imgContours, (rect.x, rect.y), (rect.max_x, rect.max_y), (0, 0, 255), 1)
    return imgContours


def drawAndClusterClusterRefs(image, clusterRefs):

    clusters = clusterContours(clusterRefs)
    imgContours = drawClusters(image, clusters)

    return (imgContours, clusters)


def clusterContours(clusterRefs):
    clusternr = 1
    first = clusterRefs[0]
    first.clusterNumber = 0
    clusters = [first]
    clusterRefs.remove(first)
    for current in clusterRefs:  # filter(lambda ref: ref.clusterColor == 179, clusterRefs):

        dists = filter(lambda t: t[0] != sys.maxsize,  map(lambda c: (c.dist(current), c), clusters))
        intersection = min(dists, default=(0, current), key=lambda i: i[0])[1]
        if intersection == current:
            # this one gets a new cluster
            current.clusterNumber = clusternr
            clusternr = clusternr + 1
            clusters.append(current)
            clusterRefs.remove(current)
        else:

            # if intersection.clusterNumber == -1 and current.clusterNumber == -1:
            #     # intersection with 2 new clusters
            #     intersection.clusterNumber = clusternr
            #     current.clusterNumber = clusternr
            #     current.appendContours(intersection.contours)
            #     clusterRefs.remove(intersection)
            #     clusternr = clusternr + 1
            # if current.clusterNumber == -1:  # and intersection.clusterNumber > -1:
            # new cluster gets merged with old cluster
            intersection.appendContours(current.contours)
            clusterRefs.remove(current)
            # elif current.clusterNumber > -1 and intersection.clusterNumber == -1:
            #     # new cluster gets merged with old cluster
            #     current.clusterNumber.appendContours(intersection.contours)
            #     clusterRefs.remove(intersection)
            # else:
            #     # 2clusters merged
            #     current.appendContours(intersection.contours)
            #     clusters.remove(intersection)
            #     clusterRefs.remove(intersection)
    return clusters


def calculateChannel_async(args):
    (image, channel) = args
    return calculateChannel(image, channel)


def calculateChannel(image, channel):
    channelimg = getFlatChannel(image, channel)
    img, imgcanny = canny(channelimg)
    imgcontours, contours = getContours(img, (255, 255, 255))
    clusterRefs = generateClusterRefs(image, contours)
    imgroi, clusters = drawAndClusterClusterRefs(channelimg, clusterRefs)

    return (channel, img, imgcontours, imgroi, clusters)


def runCode():
    cap = cv2.VideoCapture(0)
    #pool =  mp.Pool(processes=3)
    previouscanny = None
    while(True):
        # Capture frame-by-frame
        ret, image = cap.read()
        image = cv2.medianBlur(image, 9)
        cv2.imshow('Image', image)

        # image = cv2.blur(image,(5,5))

        #result = pool.map_async(calculateChannel_async,map(lambda ch:(image,ch),range(0,3)))
        result = map(lambda args: calculateChannel_async(args), map(lambda ch: (image, ch), range(0, 3)))
        # while not result.ready():
        #     print('Running')
        #     time.sleep(0.5)

        clusters = []
        # for r in result.get():
        for r in result:
            (channel, img, imgcontours, imgroi, clusteredContours) = r
            clusters.extend(clusteredContours)
            cv2.imshow('Channel {}'.format(channel), img)
            cv2.imshow('Channel {} contours'.format(channel), imgcontours)
            cv2.imshow('Channel {} clustered'.format(channel), imgroi)

        clusters = clusterContours(clusters)
        imgTotal = drawClusters(image, clusters)

        # hsv = cv2.cvtColor(image,cv2.COLOR_BGR2HSV)

        # hue,huecanny = cannyChannel(hsv,0)
        # sat, satcanny = cannyChannel(hsv,1)
        # val , valcanny =  cannyChannel(hsv,2)

        # hls = cv2.cvtColor(image,cv2.COLOR_BGR2HLS)

        # hue2, hue2canny = cannyChannel(hls,0)
        # light ,lightcanny =  cannyChannel(hls,1)
        # sat2 , sat2canny =  cannyChannel(hls,2)

        # blue, bluecanny = cannyChannel(image, 0)
        # imgbluecontours, bluecontours = contourChannel(image, 0, (255, 0, 0))
        # bluechannelimg = getFlatChannel(image, 0)
        # imgblueroi, clusters = clusterContours(bluechannelimg, bluecontours)
        # green, greencanny = cannyChannel(image, 1)
        # # imggreencontours , greencontours = contourChannel(image,1,(0,255,0))
        # red,  redcanny = cannyChannel(image, 2)
        # # imgredcontours ,  redcontours = contourChannel(image,2,(0,0,255))

        # # bluecanny,huecanny,satcanny,valcanny,hue2canny,sat2canny,lightcanny)
        # totalcanny = merge2(redcanny, greencanny)

        # if(previouscanny is None):
        #     previouscanny = totalcanny

        # totalcanny = merge2(totalcanny, previouscanny)

        # previouscanny = totalcanny
        # ret, totalcanny = cv2.threshold(totalcanny, 0, 255, cv2.THRESH_TRUNC)

        # image = np.bitwise_or(image, totalcanny)

        # cv2.imshow('Blue', blue)
        # cv2.imshow('Green', green)

    #  cv2.imshow('Green contours',imggreencontours)
    # cv2.imshow('Red contours',imgredcontours)
        # cv2.imshow("Canny", totalcanny)
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


def main():
    runCode()


if __name__ == '__main__':
    mp.freeze_support()
    main()
