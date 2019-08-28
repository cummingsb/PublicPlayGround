using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteThis.code
{
    public static class TextTo
    {
        public static ESRI.ArcGIS.Geometry.IPolyline5 TextToPoly(string input)
        {
            ESRI.ArcGIS.Geometry.IPolyline5 pLine = new ESRI.ArcGIS.Geometry.Polyline() as ESRI.ArcGIS.Geometry.IPolyline5;
            if (input.Length > 0)
            {
                float offset = (-1 * (input.Length / 2)) * TextGeomsStruct._letteroffset;
                List<ESRI.ArcGIS.Geometry.IPointCollection> pointList = TextToPoints(input, offset);
                if (pointList.Count > 0)
                {
                    foreach (ESRI.ArcGIS.Geometry.IPointCollection pntColl in pointList)
                    {
                        pLine = pntColl as ESRI.ArcGIS.Geometry.IPolyline5;
                    }
                }
            }

            return pLine;
        }

        internal static List<ESRI.ArcGIS.Geometry.IPointCollection> TextToPoints(string input, float offset)
        {
            System.Drawing.FontConverter fc = new System.Drawing.FontConverter();
            float hpos = 0.0F;
            float vpos = 0.0F;
            float angle = 0.0F; // 225.0F;
            List<ESRI.ArcGIS.Geometry.IPointCollection> apointList = new List<ESRI.ArcGIS.Geometry.IPointCollection>();
            foreach (char c in input)
            {
                System.Drawing.Drawing2D.GraphicsPath p = GetPath(c.ToString(), hpos + offset, vpos, angle);
                apointList.Add(PathToPoint(p.PathPoints));

            }

            return apointList;
        }

        private static ESRI.ArcGIS.Geometry.IPointCollection PathToPoint(System.Drawing.PointF[] arrPoints)
        {
            ESRI.ArcGIS.Geometry.IPointCollection pointColl = new ESRI.ArcGIS.Geometry.MultipointClass();
            ESRI.ArcGIS.Geometry.IPoint point = new ESRI.ArcGIS.Geometry.PointClass();

            foreach (System.Drawing.PointF pointf in arrPoints)
            {
                // switching coords - ESRI and Microsoft use a different 'paper space'.
                point.X = pointf.Y;
                point.Y = pointf.X;

                pointColl.AddPoint(point);
                point = new ESRI.ArcGIS.Geometry.PointClass();
            }

            return pointColl;
        }
        
        private static System.Drawing.Drawing2D.GraphicsPath GetPath(string text, float hpos, float vpos, float angle)
        {
            // from Microsoft
            // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.drawing2d.matrix.rotateat?view=netframework-4.8

            // Create a GraphicsPath.
            System.Drawing.Drawing2D.GraphicsPath path =
                new System.Drawing.Drawing2D.GraphicsPath();

            // Add the string to the path; declare the font, font style, size, and
            // vertical format for the string.
            System.Drawing.FontFamily ff = new System.Drawing.FontFamily("Times New Roman");  // System.Drawing.FontFamily("Arial");
            
            path.AddString(text, ff, 2, 30,
                new System.Drawing.PointF(hpos, vpos),
                new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.DirectionVertical));

            // closing line
            path.AddLine(path.PathPoints[path.PathPoints.Count() -1], path.PathPoints[0]);

            // TODO: need some clean up if we wanted polygons instead of lines

            return path;
        }

    }
}
