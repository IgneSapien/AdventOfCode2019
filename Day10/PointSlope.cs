using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SadRogue.Primitives;

namespace Day10
{
    class PointSlope
    {
        public Point StartP { get; private set; }
        public Point EndP { get; private set; }
        public Point Vector { get; protected set; }

        float? slope;

        public PointSlope(Point start, Point end)
        {
            StartP = start;
            EndP = end;
            Vector = EndP - StartP;

            //Slope = y2 - y1 / x2 - x1
            //This should work with the vector
            //If x is zero then the slope is undefined because a virtical line doesn't have a slope 
            if (Vector.X == 0)
            {
                slope = null;
            }
            else
            {
                slope = (float)Vector.Y / (float)Vector.X;
            }

        }

        public bool PointOnSlope(Point point)
        {
            //If the slope is virtical we just have to check the X matches 
            if(slope == null )
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
            int MinX = Math.Min(StartP.X, EndP.X);
            int MaxX = Math.Max(StartP.X, EndP.X);

            int MinY = Math.Min(StartP.Y, EndP.Y);
            int MaxY = Math.Max(StartP.Y, EndP.Y);

            List<Point> points = new List<Point>();
            for (int x = MinX; x <= MaxX; x++)
            {
                for (int y = MinY; y <= MaxY; y++)
                {
                    Point p = new Point(x, y);
                    if(PointOnSlope(p))
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

