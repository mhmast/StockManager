from Rect import Rect
import cv2
import numpy as np


class TiltedRect:

    def __init__(self, p1, p2, angle):
        self.rect = Rect(p1, p2)
        self.angle = angle

    def box(self):
        return (self.rect.topLeft, self.rect.bottomRight, self.angle)

    def getBoxPoints(self):
        box = cv2.boxPoints(self.box())
        return np.int0(box)

    def __eq__(self, rhs):
        (tl, br, a) = self.box()
        (otl, obr, oa) = rhs.box()
        return tl[0] == otl[0] and tl[1] == otl[1] and \
            br[0] == obr[0] and br[1] == obr[1] and \
            a == oa
