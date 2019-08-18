import cv2
import numpy as np


class ClusterRef:

    # @property
    # def rect(self):
    #     return self.__rect

    # @property
    # def contours(self):
    #     return self.__contours

    # def appendContours(self, contours):
    #     if len(contours) == 0:
    #         return
    #     if not isinstance(contours, list):
    #         contours = [contours]
    #     points = []

    #     for c in contours:
    #         self.__contours.append(c)
    #     for contour in self.__contours:
    #         (x, y, w, h) = cv2.boundingRect(contour)
    #         points.append([x, y])
    #         points.append([x+w, y+h])
    #     rect = cv2.boundingRect(np.array(points).astype(int))

    #     self.__rect = Rect(*rect)
    @staticmethod
    def merge(existing, subjects):
        returnrefs = []
        delta = 4
        for r in subjects:
            merged = False
            for e in existing:
                if e.clusterNumber == r.clusterNumber:
                    break
                if abs(int(existing.clusterColor) - int(r.clusterColor)) < delta:
                    returnrefs.remove(existing)
                    winner = existing if existing.rect.area > r.rect.area else r
                    winner.rects = np.concatenate((existing.rects, r.rects), axis=None)
                    returnrefs.append(winner)
                    merged = True
                    break
            if not merged:
                returnrefs.append(r)
        return returnrefs

    def collideAndDevide(self, img, other):
        if not self.rect.intersect(other.rect):
            return [self, other]

        def getClusterRef(img, rect):
            (X, Y) = rect.center
            return ClusterRef(img[Y, X], rect)
        return ClusterRef.merge([getClusterRef(img, r) for r in self.rect.collideAndDevide(other.rect)], [other])

    def getComputedContours(self):
        points = []
        for rect in self.rects:
            points.append(rect.topLeft)
            points.append(rect.bottomRight)
        minRect = cv2.minAreaRect(np.array(points).astype(int))
        box = cv2.boxPoints(minRect)
        return [np.int0(box)]

    # @property
    # def clusterColor(self):
    #     return self.__clusterColor

    # @clusterColor.setter
    # def clusterColor(self, val):
    #     self.__clusterColor = val

    # @property
    # def clusterNumber(self):
    #     return self.__clusterNumber

    def intersect(self, other):
        return self.rect.intersect(other.rect)

    def dist(self, other):

        compareColor = not hasattr(self.clusterColor, "__len__")

        if (compareColor and self.clusterColor != other.clusterColor) or not self.intersect(other):
            return sys.maxsize
        return max(self.rect.area - other.rect.area, 0)

    # @clusterNumber.setter
    # def clusterNumber(self, val):
    #     self.__clusterNumber = val

    def __init__(self, clusterColor, boundingRect=None):
        self.clusterColor = clusterColor
        self.clusterNumber = (clusterColor, boundingRect)
        # self.__contours = []
        # self.appendContours(contours)
        # if boundingRect is not None:
        self.rect = boundingRect
        self.rects = [boundingRect]
