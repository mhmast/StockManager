
import cv2
import numpy as np


class TiltedRect:

    def __init__(self, p1, p2, angle):
        self.p1 = p1
        self.p2 = p2
        self.angle = angle
        self.hash = hash((p1, p2, angle))

    def box(self):
        return (self.p1, self.p2, self.angle)

    def getBoxPoints(self):
        box = cv2.boxPoints(self.box())
        return np.int0(box)

    def __eq__(self, rhs):
        return self.hash == rhs.hash

    def __hash__(self):
        return self.hash
