import math

def rotate(deg):
    rad = deg * 0.0174532925
    return [
        [math.cos(rad) , -math.sin(rad)],
        [math.sin(rad), math.cos(rad)]
    ]