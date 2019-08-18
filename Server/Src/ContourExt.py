import cv2
from TiltedRect import TiltedRect


class ContourExt:

    def __init__(self, contours):
        self.contours = contours
        self.rect = TiltedRect(*cv2.minAreaRect(contours))

    def __hash__(self):
        return self.rect.__hash__()

    def __cmp__(self, other):
        return object.__cmp__(self.rect, other.rect)

    def __eq__(self, rhs):
        return self.rect.__eq__(rhs.rect)
