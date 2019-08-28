using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteThis.code
{
    /// <summary>
    /// GeomUtil - utilities for ESRi geometries.
    /// </summary>
    static class GeomUtil
    {
        /// <summary>
        /// PointToPolyLine - Takes a point collection and generates a polyline
        /// </summary>
        /// <param name="pntColl"></param>
        /// <returns>PolyLine</returns>
        public static ESRI.ArcGIS.Geometry.Polyline PointToPolyline(ESRI.ArcGIS.Geometry.IPointCollection pntColl)
        {
            ESRI.ArcGIS.Geometry.Polyline pline = new ESRI.ArcGIS.Geometry.Polyline();
            if (pntColl.PointCount > 0)
            {
                ESRI.ArcGIS.Geometry.ISegmentCollection segColl = PointsToSegmentColl(pntColl);
                if (segColl.SegmentCount >= 0)
                {
                    pline = segColl as ESRI.ArcGIS.Geometry.Polyline;
                }
            }
            return pline;
        }

        /// <summary>
        /// PointToSegmentColl - takes a point collection and returns a collection of segments.
        /// </summary>
        /// <param name="pntColl"></param>
        /// <returns>SegmentCollection</returns>
        internal static ESRI.ArcGIS.Geometry.ISegmentCollection PointsToSegmentColl(ESRI.ArcGIS.Geometry.IPointCollection pntColl)
        {
            ESRI.ArcGIS.Geometry.ISegmentCollection segcoll = new ESRI.ArcGIS.Geometry.PolylineClass() as ESRI.ArcGIS.Geometry.ISegmentCollection;
            ESRI.ArcGIS.Geometry.ISegment segment;
            ESRI.ArcGIS.Geometry.IPoint frompoint = null;
            ESRI.ArcGIS.Geometry.IPoint topoint = null;
             
            for (int i = 0; i < pntColl.PointCount; i++)
            {
                if (frompoint == null)
                {
                    frompoint = pntColl.get_Point(i);
                    if (pntColl.PointCount >= (i + 1))
                    {
                        topoint = pntColl.get_Point((i + 1));
                        segment = PointsToSegment(frompoint, topoint);
                        segcoll.AddSegment(segment, Type.Missing, Type.Missing);
                    }
                }
                else
                {
                    // to point becomes frompoint after each iteration
                    frompoint = topoint;
                    if (pntColl.PointCount > (i + 1))
                    {
                        topoint = pntColl.get_Point((i + 1));
                        segment = PointsToSegment(frompoint, topoint);
                        segcoll.AddSegment(segment, Type.Missing, Type.Missing);
                    }
                }
            }

            return segcoll;
        }

        /// <summary>
        /// PointsToSegment - Takes to/from points to form a line segment
        /// </summary>
        /// <param name="frompoint"></param>
        /// <param name="topoint"></param>
        /// <returns>Segment</returns>
        internal static ESRI.ArcGIS.Geometry.ISegment PointsToSegment(ESRI.ArcGIS.Geometry.IPoint frompoint, ESRI.ArcGIS.Geometry.IPoint topoint)
        {
            ESRI.ArcGIS.Geometry.ISegment segment = null ;
            ESRI.ArcGIS.Geometry.ILine line = new ESRI.ArcGIS.Geometry.Line() ;
            if (frompoint != null && topoint != null)
            {
                
                line.FromPoint = frompoint;
                line.ToPoint = topoint;
                segment = line as ESRI.ArcGIS.Geometry.ISegment;
            }
            return segment;
        }

        internal static ESRI.ArcGIS.Geometry.IPolygon4 PolylineToPolygon(ESRI.ArcGIS.Geometry.IPolyline5 inputline)
        {
            ESRI.ArcGIS.Geometry.IPolygon4 results = new ESRI.ArcGIS.Geometry.PolygonClass();
            
            return results;
        }
    }
}
