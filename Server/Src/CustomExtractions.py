import multiprocessing as mp
import time
from ImageFunctions import *
import cv2
from ClusterRef import ClusterRef
from Rect import Rect
import numpy as np


def generateClusterRefs(img, contours):
    if len(img.shape) == 3:
        img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

    def getPoint(c):
        (W, H) = np.vstack(c).squeeze()
        return (W, H)

    def getClusterRef(img, c):
        (X, Y) = c.center
        color = img[Y, X]
        return ClusterRef(color, c)

    def getBoundingRect(c):
        (x, y, w, h) = cv2.boundingRect(c)
        return Rect((x, y), (x+w, y+h))
    return [getClusterRef(img, getBoundingRect(c)) for c in contours]


def drawClusters(img, clusters):
    imgContours = img.copy()
    if len(imgContours.shape) != 3:
        imgContours = cv2.cvtColor(imgContours, cv2.COLOR_GRAY2BGR)
    for cluster in clusters:
        #cv2.drawContours(imgContours, cluster.contours, -1, (255, 0, 0))
        rect = cluster.rect
        cv2.rectangle(imgContours, rect.topLeft, rect.bottomRight, (0, 0, 255), 1)
    return imgContours


def fillClusters(img, clusters):

    if len(img.shape) != 3:
        img = cv2.cvtColor(img, cv2.COLOR_GRAY2BGR)
    imgContours = np.zeros(img.shape[:2], dtype="uint8")
    for cluster in clusters:
        contours = cluster.getComputedContours()
        cv2.drawContours(imgContours, contours, -1, (255, 255, 255), -1)
    ret = img.copy()
    cv2.imshow("mask", imgContours)
    ret[imgContours == 0] = (0, 0, 0)
    return ret


def drawAndClusterClusterRefs(image, clusterRefs):

    clusterRefs = clusterContours(image, clusterRefs)
    imgContours = drawClusters(image, clusterRefs)

    return (imgContours, clusterRefs)


def clusterContours(image, clusterRefs):
    # clusternr = 1
    # first = clusterRefs[0]
    # first.clusterNumber = 0
    # clusters = [first]
    # clusterRefs.remove(first)
    if len(image.shape) == 3:
        image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    newClusters = []

    for current in clusterRefs:  # filter(lambda ref: ref.clusterColor == 179, clusterRefs):
        for cluster in clusterRefs:
            if cluster == current:
                continue
            newClusters = ClusterRef.merge(newClusters, current.collideAndDevide(image, cluster))
        clusterRefs.remove(current)
    return newClusters

    #     dists = filter(lambda t: t[0] != sys.maxsize,  map(lambda c: (c.dist(current), c), newClusters))
    #     intersection = min(dists, default=(0, current), key=lambda i: i[0])[1]
    #     if intersection == current:
    #         # this one gets a new cluster
    #         current.clusterNumber = clusternr
    #         clusternr = clusternr + 1
    #         clusters.append(current)
    #         clusterRefs.remove(current)
    #     else:

    #         # if intersection.clusterNumber == -1 and current.clusterNumber == -1:
    #         #     # intersection with 2 new clusters
    #         #     intersection.clusterNumber = clusternr
    #         #     current.clusterNumber = clusternr
    #         #     current.appendContours(intersection.contours)
    #         #     clusterRefs.remove(intersection)
    #         #     clusternr = clusternr + 1
    #         # if current.clusterNumber == -1:  # and intersection.clusterNumber > -1:
    #         # new cluster gets merged with old cluster
    #         intersection.appendContours(current.contours)
    #         clusterRefs.remove(current)
    #         # elif current.clusterNumber > -1 and intersection.clusterNumber == -1:
    #         #     # new cluster gets merged with old cluster
    #         #     current.clusterNumber.appendContours(intersection.contours)
    #         #     clusterRefs.remove(intersection)
    #         # else:
    #         #     # 2clusters merged
    #         #     current.appendContours(intersection.contours)
    #         #     clusters.remove(intersection)
    #         #     clusterRefs.remove(intersection)
    # return clusters


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


pool = None


def extractFeatures(image, asynch, resultPerChannel):

    image = cv2.medianBlur(image, 21)
    if asynch and pool is None:
        pool = mp.Pool(processes=3)

    result = []
    if resultPerChannel:
        if asynch:
            result = pool.map_async(calculateChannel_async, map(lambda ch: (image, ch), range(0, 3)))
            while not result.ready():
                print('Running')
                time.sleep(0.5)
            result = result.get()
        else:
            result = map(lambda args: calculateChannel_async(args), map(lambda ch: (image, ch), range(0, 3)))

        clusters = []
        for r in result:
            (channel, img, imgcontours, imgroi, clusteredContours) = r
            clusters.extend(clusteredContours)
            chanTotal = fillClusters(imgroi, set(clusteredContours))
            cv2.imshow('Channel {}'.format(channel), img)
            cv2.imshow('Channel {} contours'.format(channel), imgcontours)
            cv2.imshow('Channel {} clustered'.format(channel), imgroi)
            cv2.imshow('Channel {} total'.format(channel), chanTotal)

        imgTotal = fillClusters(image, set(clusters))
        cv2.imshow("total", imgTotal)
    else:
        img, imgcanny = canny(image)
        imgcontours, contours = getContours(img, (255, 255, 255))
        clusters = generateClusterRefs(image, contours)
        clusters = clusterContours(image, clusters)

        imgroi = fillClusters(image, clusters)
        cv2.imshow("contours", imgcontours)
        cv2.imshow("image", image)
        cv2.imshow("total", imgroi)
