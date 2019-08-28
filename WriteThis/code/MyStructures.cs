using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteThis.code
{
  
    public class TextGeomsStruct
    {
        // constant offset for letters to prevent stacking.
        internal const float _letteroffset = 30;

        public class TextGeomCollection
        {
            private List<TextGeomItem> _textgeomlist = new List<TextGeomItem>();

            public TextGeomCollection()
            {
                _textgeomlist = new List<TextGeomItem>();
            }

            public TextGeomCollection(string input)
            {
                _textgeomlist = new List<TextGeomItem>();
                Extract(input);
            }

            public List<TextGeomItem> TextGeometries
            {
                get { return _textgeomlist; }
                set { _textgeomlist = value; }
            }

            public double Width
            {
                get
                {
                    double d = 0.0;
                    foreach (TextGeomItem item in this._textgeomlist)
                    {
                        d = d + item.Geometry.Envelope.Width;
                    }
                    return d;
                }
            }

            public double Height
            {
                get
                {
                    double d = 0.0;
                    foreach (TextGeomItem item in this._textgeomlist)
                    {
                        d = d + item.Geometry.Envelope.Height;
                    }
                    return d;
                }
            }

            public int Append(TextGeomItem input)
            {
                this._textgeomlist.Add(input);
                return this._textgeomlist.Count;
            }

            public int Append(ESRI.ArcGIS.Geometry.IPolyline5 inputgeom, string inputtext)
            {
                TextGeomItem tgitem = new TextGeomItem(inputgeom, inputtext);
                int result = this.Append(tgitem);
                return result;
            }

            /// <summary>
            /// CreateFeatures - make ESRI features based on text
            /// </summary>
            /// <param name="layername">layer to add data to</param>
            /// <returns></returns>
            internal bool CreateFeatures(string layername)
            {
                bool result = true;

                try
                {
                    ESRI.ArcGIS.Carto.IFeatureLayer2 featlayer = code.Layers.getFeatureLayerByName("MyLines");
                    if (featlayer != null)
                    {
                        ESRI.ArcGIS.Geodatabase.IFeatureBuffer newFeatBuff = featlayer.FeatureClass.CreateFeatureBuffer();

                        ESRI.ArcGIS.Geodatabase.IFeatureCursor featureCursor = featlayer.FeatureClass.Insert(true);
                        foreach (TextGeomItem item in this.TextGeometries)
                        {
                           
                            //        // ID = unique id for gis line feature
                            newFeatBuff.set_Value(featlayer.FeatureClass.FindField("LineID"), Guid.NewGuid().ToString("B"));
                            newFeatBuff.set_Value(featlayer.FeatureClass.FindField("LETTER"), item.Text);

                            newFeatBuff.Shape = item.Geometry;
                            featureCursor.InsertFeature(newFeatBuff);
                        }
                        featureCursor.Flush();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
                    }
                    return result;
                }
                catch (System.Exception ex)
                {
                    return false;
                }
          
                
            }

            /// <summary>
            /// Extract - take letters and make geometries
            /// </summary>
            /// <param name="input"></param>
            private void Extract(string input)
            {
                if (input.Length > 0)
                {
                    ESRI.ArcGIS.Geometry.Polyline pLine;
                    float offset = _letteroffset; 
                    for (int i=(input.Length - 1);i > -1;i--)
                    {
                        char c = input[i];
                        List<ESRI.ArcGIS.Geometry.IPointCollection> pointList = TextTo.TextToPoints(c.ToString(),offset);
                        if (pointList.Count > 0)
                        {
                            foreach (ESRI.ArcGIS.Geometry.IPointCollection pntColl in pointList)
                            {
                                pLine = GeomUtil.PointToPolyline(pntColl);
                                // add geom and attributes to this
                                this.Append((ESRI.ArcGIS.Geometry.IPolyline5)pLine, c.ToString());
                            }
                        }
                        // change offset for each letter
                        offset = offset + TextGeomsStruct._letteroffset;
                    }
                }
            }


        }

        /// <summary>
        /// TextGeomItem - holds single character items
        /// </summary>
        public class TextGeomItem
        {
            private ESRI.ArcGIS.Geometry.IPolyline5 _textgeom = null;
            private string _text = string.Empty;

            public TextGeomItem() { }

            public TextGeomItem(ESRI.ArcGIS.Geometry.IPolyline5 inputgeom, string inputtext)
            {
                if (!inputgeom.IsEmpty)
                {
                    _textgeom = inputgeom;
                }

                if (inputtext.Length > 0)
                {
                    _text = inputtext;
                }
            }

            public ESRI.ArcGIS.Geometry.IPolyline5 Geometry
            {
                get { return _textgeom; }
                set { _textgeom = value; }
            }

            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }
        }
    }
}
