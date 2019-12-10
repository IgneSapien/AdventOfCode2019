using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using SadRogue.Primitives;

namespace Day10
{
    class AngleSlope
    {
        public Point StartP { get; private set; }
        public double Degree { get; private set; }

        double? slope;

        Point MapSize;

        public AngleSlope(Point start, double degree, Point MapSize)
        {
            StartP = start;
            Degree = degree;

            
           slope = Math.Tan(Degree); 
            
          
            
        }

        public bool PointOnSlope(Point point)
        {
            //If the slope is virtical we just have to check the X matches 
            if (slope == null)
            {
                return point.X == StartP.X;
            }

            //y − y1 = m(x −x1)
            //Sub m with slope and either start/end point for x1, y1
            //Again we can just use a vector 
            Point v = point - StartP;

            return v.Y == slope * v.X;
        }

        //Returns an list of the whole points on this slope between the start and end point
        //List will be ordered from nearest to start i.e. top of the list is visiable, anything after is not. 
        public List<Point> WholePointsOnSlope()
        {
            int MinX = Math.Min(StartP.X, MapSize.X);
            int MaxX = Math.Max(StartP.X, MapSize.X);

            int MinY = Math.Min(StartP.Y, MapSize.Y);
            int MaxY = Math.Max(StartP.Y, MapSize.Y);

            List<Point> points = new List<Point>();
            for (int x = MinX; x <= MaxX; x++)
            {
                for (int y = MinY; y <= MaxY; y++)
                {
                    Point p = new Point(x, y);
                    if (PointOnSlope(p))
                    {
                        points.Add(p);
                    }
                }
            }
            points.Remove(StartP);

            return points;
        }
    }
}
