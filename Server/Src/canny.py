#from CustomExtractions import *
from Scikit import extractFeatures
import cv2
import multiprocessing as mp
from skimage import img_as_float
from ImageFunctions import cartoon, canny


def runCode():

    cap = cv2.VideoCapture(0)
    ret, image = cap.read()
    while(True):
        # Capture frame-by-frame
        (h, w) = image.shape[:2]
        factor = max(w, h) / 500
        image = cv2.resize(image, (int(w/factor), int(h/factor)))
        image = cartoon(image)
        #image, cannymask = canny(image)

        #extractFeatures(image, False, False)
        features = extractFeatures(image, False, True)
        boxPoints = [c.rect.getBoxPoints() for c in features]
        contours = [c.contours for c in features]
        cv2.drawContours(image, contours, -1, (255, 0, 0))
        cv2.drawContours(image, boxPoints, -1, (0, 0, 255))
        image = cv2.resize(image, (w, h))
        cv2.imshow("total", image)
        key = cv2.waitKey(0)
        if key == 27:    # Esc key to stop
            break
        else:
            ret, image = cap.read()

    # When everything done, release the capture
    cap.release()
    cv2.destroyAllWindows()


def main():
    runCode()


if __name__ == '__main__':
    mp.freeze_support()
    main()
