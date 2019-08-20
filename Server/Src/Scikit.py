# __pool__ = None
__showIntermediate__ = True
__drawContourBox__ = True
__drawContours__ = False
import uuid
import matplotlib.pyplot as plt
import numpy as np
from skimage.measure import find_contours, approximate_polygon, \
    subdivide_polygon
import cv2
import multiprocessing as mp
from multiprocessing.pool import ThreadPool
import time
from ImageFunctions import getContours, cartoon
from skimage import img_as_ubyte, img_as_int, img_as_float
from ImageFunctions import getChannel
from ContourExt import ContourExt


def displayContoursSciKit(image, contours):
    # Display the image and plot all contours found
    fig, ax = plt.subplots()
    ax.imshow(image, interpolation='nearest', cmap=plt.cm.gray)

    for n, contour in enumerate(contours):
        ax.plot(contour[:, 1], contour[:, 0], linewidth=2)

    ax.axis('image')
    ax.set_xticks([])
    ax.set_yticks([])
    plt.show()


def displayContoursCV(image, contourexts):
    # cvim, cvcont = getContours(img_as_ubyte(image), (255, 0, 0))
    contour = 0
    contours = [c.contours for c in contourexts]
    for r in contourexts:
        color = (255, 255, 255)
        global __drawContourBox__
        if __drawContourBox__:
            r.drawBox(image, color)
        global __drawContours__
        if __drawContours__:
            r.drawContour(image, (100, 100, 100))
            contour += 1

    cv2.imshow(str(uuid.uuid4()), image)


def findContours(args):

    return [ContourExt([np.array(np.flip(contour, 1)).astype(int)]) for contour in find_contours(*args)]
    # contours = find_contours(*args)
    # newContours = []
    # for contour in contours:
    #     # for _ in range(5):
    #     #contour = subdivide_polygon(contour, degree=2, preserve_ends=True)
    #     # approximate subdivided polygon with Douglas-Peucker algorithm
    #     #contour = approximate_polygon(contour, tolerance=0.02)
    #     contour = np.array(np.flip(contour, 1)).astype(int)  # convert to cv contour
    #     newContours.append(ContourExt(contour))
    # return newContours


def extractFeatures(img, asynch=False, multiChannel=False):

    img = cartoon(img)
    imgColor = img
    images = [img]

    channels = 1
    if multiChannel:
        images = [getChannel(img, c) for c in range(0, 3)]
        channels = 3

    result = []
    contours = []
    # if asynch:
    #     pool = mp.pool.ThreadPool(channels)
    #     result = pool.map_async(processSingleImage_ansync, [(i, asynch) for i in images])
    #     while not result.ready():
    #         print('Running')
    #         time.sleep(0.5)
    #     result = result.get()
    #     for r in result:
    #         contours.append(r)
    # else:
    for image in images:
        contours.append(processSingleImage(image, asynch))
    if len(contours) == 1:
        return contours[0]
    otherContours = contours[1:]
    overlap = [c for c in contours[0] if inAtLeast(c, otherContours, len(otherContours)-1)]
    return overlap


def cluster(contourExts):
    origlen = len(contourExts)
    itters = 0
    for c in contourExts:
        for c2 in contourExts:
            if c != c2 and c.overlaps(c2):
                contourExts.remove(c2)
                contourExts.remove(c)
                mergedContours = []
                mergedContours.extend(c.contours)
                mergedContours.extend(c2.contours)
                contourExts.append(ContourExt(mergedContours))
                break
        itters += 1

    return contourExts
    # return set([ContourExt(list(zip(c.contours, c2.contours))[0])
    #             if c != c2 and c.overlaps(c2) else c for c in contourExts for c2 in contourExts])


def inAtLeast(c, itters, num):
    if len(itters) < num:
        return False
    foundIn = 0
    for i in range(0, len(itters)):
        if c in itters[i]:
            foundIn += 1
    return foundIn >= num


def processSingleImage_ansync(args):
    return processSingleImage(*args)


def processSingleImage(image, asynch):
    original = image
    min = 0
    max = 10
    if len(image.shape) == 3:
        image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    image = img_as_float(image)

    contours = []
    result = None
    if asynch:
        pool = mp.Pool(max - min)
        result = pool.map_async(findContours, [(image, no/10) for no in range(min, max)])
        while not result.ready():
            print('Running')
            time.sleep(0.5)
        result = result.get()
        for r in result:
            contours.extend(r)
    else:
        for i in range(min, max):
            contours.extend(findContours((image, i/10)))
    global __showIntermediate__
    if __showIntermediate__:
        displayContoursCV(original, contours)
    return contours
