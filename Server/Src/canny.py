# from CustomExtractions import *
from Scikit import extractFeatures, cluster
import cv2
import multiprocessing as mp
from skimage import img_as_float
from ImageFunctions import cartoon, canny


def runCode():

    cap = cv2.VideoCapture(0)
    ret, image = cap.read()
    while(True):
        cv2.imshow("original",image)
        # Capture frame-by-frame
        (h, w) = image.shape[:2]
        factor = max(w, h) / 500
        image = cv2.resize(image, (int(w/factor), int(h/factor)))
        # image = cartoon(image)
        # image, cannymask = canny(image)

        # extractFeatures(image, False, False)
        features = extractFeatures(image, False, True)
        beforeImg = image.copy()
        # for f in features:
        #     f.drawBox(beforeImg, (0, 0, 255))
        #     #f.drawContour(beforeImg, (255, 0, 0))
        # cv2.imshow("beforeCluster", beforeImg)
        
        # features = cluster(features)

        # for f in features:
        #   f.drawBox(image, (0, 0, 255))
    #       f.drawContour(beforeImg, (255, 0, 0))
        
        
        imarea = w* h
        counter = 0
        fmap = [(f.rect.area(),f) for f in features]
        fmap.sort(key=lambda f:f[0],reverse=True)
        for f in fmap:
            if counter >0:
                break
            counter += 1
            f =f[1]
            f.drawBox(image, (0,0, 255))
            f.drawContour(image,(0,255,0))
                

        image = cv2.resize(image, (w, h))
        cv2.imshow("afterCluster", image)
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
