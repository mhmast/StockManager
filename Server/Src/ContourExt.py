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
        self.moments = [cv2.moments(c) for c in self.contours]
        self.rect = TiltedRect(*cv2.minAreaRect(np.vstack(contours)))
        global no
        self.no = no
        no += 1

    def contourArea(self):
        return np.mean([m["m00"] for m in self.moments])

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

    def drawCentroid(self, img, color):

        Xs = []
        Ys = []
        areas = []
        for M in self.moments:
            area = M["m00"]
            if M["m00"] == 0:
                continue
            # calculate x,y coordinate of center
            cX = int(M["m10"] / area)
            cY = int(M["m01"] / area)
            areas.append(area)
            Xs.append(cX)
            Ys.append(cY)

        (h, w) = img.shape[:2]
        imgArea = h*w
        area = int(np.mean(areas))
        # put text and highlight the center
        cv2.circle(img, (int(np.mean(Xs)), int(np.mean(cY))), int((imgArea / 50) * (area/imgArea)), color, -1)
