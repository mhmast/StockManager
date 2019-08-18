
class Rect:

    def __init__(self, p1, p2):
        (self.x, self.y) = p1
        (self.max_x, self.max_y) = p2
        self.area = (p2[0] - p1[0]) * (p2[1]-p1[1])
        self.center = (self.max_x - self.x, self.max_y - self.y)
        self.topLeft = p1
        self.bottomLeft = (self.x, self.max_y)
        self.topRight = (self.max_x, self.y)
        self.bottomRight = p2

    def intersect(self, other):

        if self.x >= other.max_x or self.max_x <= other.x:
            return False
        if self.y >= other.max_y or self.max_y <= other.y:
            return False
        return True

    def collideAndDevide(self, other):
        if not self.intersect(other):
            return [self, other]
        leftmost = self if self.x < other.x else other
        rightmost = self if self.max_x > other.max_x else other

        returnVals = []
        intersection = Rect(
            (max(self.x, other.x), max(self.y, other.y)),
            (min(self.max_x, other.max_x), min(self.max_y, other.max_y))
        )
        # append intersection
        returnVals.append(intersection)
        # append topleft
        topLeft = Rect(leftmost.topLeft, (intersection.topLeft))
        returnVals.append(topLeft)
        # append left
        left = Rect(topLeft.bottomLeft, intersection.bottomLeft)
        returnVals.append(left)
        # append bottomLeft
        bottomLeft = Rect(left.bottomLeft, (intersection.x, leftmost.max_y))
        returnVals.append(bottomLeft)
        # append top
        top = Rect(topLeft.topRight, intersection.topRight)
        returnVals.append(top)
        # append bottom
        bottom = Rect(bottomLeft.topRight, intersection.bottomRight)
        returnVals.append(bottom)
        # append topright
        topRight = Rect((intersection.max_x, rightmost.y), (rightmost.max_x, intersection.y))
        returnVals.append(topRight)
        # append right
        right = Rect(intersection.topRight, (rightmost.max_x, intersection.max_y))
        returnVals.append(right)
        # append bottomright
        bottomRight = Rect(intersection.bottomRight, (rightmost.max_x, rightmost.max_y))
        returnVals.append(bottomRight)
        return [r for r in returnVals if r.area > 0]
