import cv2
from TiltedRect import TiltedRect
import numpy as np

no = 0

class ContourExt:

    def __init__(self, contours):
        # contours = np.array(contours)
        # if len(contours.shape) != 3:
        #     raise Exception("contours must be of shape 3")
        self.contours = contours
        self.rect = TiltedRect(*cv2.minAreaRect(np.vstack(contours)))
        global no
        self.no = no
        no += 1

    def __hash__(self):
        return self.rect.__hash__()

    def __cmp__(self, other):
        return object.__cmp__(self.rect, other.rect)

    def __eq__(self, rhs):
        return self.rect.__eq__(rhs.rect)

    def overlaps(self, other):
        return cv2.rotatedRectangleIntersection(self.rect.box(), other.rect.box())[0]

    def drawContour(self, img, color):
        cv2.drawContours(img, self.contours, -1, color)

    def drawBox(self, img, color):
        cv2.drawContours(img, [self.rect.getBoxPoints()], -1, color)
