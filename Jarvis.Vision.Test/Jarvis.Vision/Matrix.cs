﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.Vision
{

    public static class Matrix
    {
        public static double[,] Laplacian3x3 => new double[,]
        { { -1, -1, -1,  },
                { -1,  8, -1,  },
                { -1, -1, -1,  }, };

        public static double[,] Laplacian5x5 => new double[,]
        { { -1, -1, -1, -1, -1, },
                { -1, -1, -1, -1, -1, },
                { -1, -1, 24, -1, -1, },
                { -1, -1, -1, -1, -1, },
                { -1, -1, -1, -1, -1  }, };

        public static double[,] LaplacianOfGaussian => new double[,]
        { {  0,   0, -1,  0,  0 },
                {  0,  -1, -2, -1,  0 },
                { -1,  -2, 16, -2, -1 },
                {  0,  -1, -2, -1,  0 },
                {  0,   0, -1,  0,  0 }, };

        public static double[,] Gaussian3x3 => new double[,]
        { { 1, 2, 1, },
                { 2, 4, 2, },
                { 1, 2, 1, }, };

        public static double[,] Gaussian5x5 => new double[,]
        { { 2, 04, 05, 04, 2 },
            { 4, 09, 12, 09, 4 },
            { 5, 12, 15, 12, 5 },
            { 4, 09, 12, 09, 4 },
            { 2, 04, 05, 04, 2 }, };

        public static double[,] Gaussian7x7 => new double[,]
        { { 1,  1,  2,  2,  2,  1,  1, },
            { 1,  2,  2,  4,  2,  2,  1, },
            { 2,  2,  4,  8,  4,  2,  2, },
            { 2,  4,  8, 16,  8,  4,  2, },
            { 2,  2,  4,  8,  4,  2,  2, },
            { 1,  2,  2,  4,  2,  2,  1, },
            { 1,  1,  2,  2,  2,  1,  1, }, };

        public static double[,] Mean3x3 => new double[,]
        { { 1, 1, 1, },
            { 1, 1, 1, },
            { 1, 1, 1, }, };

        public static double[,] Mean5x5 => new double[,]
        { { 1, 1, 1, 1, 1,},
            { 1, 1, 1, 1, 1,},
            { 1, 1, 1, 1, 1,},
            { 1, 1, 1, 1, 1,},
            { 1, 1, 1, 1, 1,}, };

        public static double[,] LowPass3x3 => new double[,]
        { { 1, 2, 1, },
            { 2, 4, 2, },
            { 1, 2, 1, }, };

        public static double[,] LowPass5x5 => new double[,]
        { { 1, 1,  1, 1, 1,},
            { 1, 4,  4, 4, 1,},
            { 1, 4, 12, 4, 1,},
            { 1, 4,  4, 4, 1,},
            { 1, 1,  1, 1, 1,}, };

        public static double[,] Sharpen3x3 => new double[,]
        { { -1, -2, -1, },
            {  2,  4,  2, },
            {  1,  2,  1, }, };

        public static double[,] Gaussian5x5Type1 => new double[,]
        { { 2, 04, 05, 04, 2 },
                { 4, 09, 12, 09, 4 },
                { 5, 12, 15, 12, 5 },
                { 4, 09, 12, 09, 4 },
                { 2, 04, 05, 04, 2 }, };

        public static double[,] Gaussian5x5Type2 => new double[,]
        { {  1,   4,  6,  4,  1 },
                {  4,  16, 24, 16,  4 },
                {  6,  24, 36, 24,  6 },
                {  4,  16, 24, 16,  4 },
                {  1,   4,  6,  4,  1 }, };

        public static double[,] Sobel3x3Horizontal => new double[,]
        { { -1,  0,  1, },
                { -2,  0,  2, },
                { -1,  0,  1, }, };

        public static double[,] Sobel3x3Vertical => new double[,]
        { {  1,  2,  1, },
                {  0,  0,  0, },
                { -1, -2, -1, }, };

        public static double[,] Prewitt3x3Horizontal => new double[,]
        { { -1,  0,  1, },
                { -1,  0,  1, },
                { -1,  0,  1, }, };

        public static double[,] Prewitt3x3Vertical => new double[,]
        { {  1,  1,  1, },
                {  0,  0,  0, },
                { -1, -1, -1, }, };


        public static double[,] Kirsch3x3Horizontal => new double[,]
        { {  5,  5,  5, },
                { -3,  0, -3, },
                { -3, -3, -3, }, };

        public static double[,] Kirsch3x3Vertical => new double[,]
        { {  5, -3, -3, },
                {  5,  0, -3, },
                {  5, -3, -3, }, };
    }

}

