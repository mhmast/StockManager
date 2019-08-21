
import cv2
import numpy as np
from Matrix import rotate


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

    def area(self):
        return cv2.contourArea(self.getBoxPoints())
        # angle = 360 + self.angle if self.angle <0 else self.angle 
        # M = rotate(360-angle)
        # p1trans = np.array(self.p1) @ M
        # p2trans = np.array(self.p2) @ M
        # return abs(max(p1trans[0],p2trans[0]) - min(p1trans[0],p2trans[0])) * abs(max(p2trans[1] , p1trans[1]) - min(p2trans[1] , p1trans[1]))


    def __eq__(self, rhs):
        return self.hash == rhs.hash

    def __hash__(self):
        return self.hash
