using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WriteThis.code
{
    abstract class CreateFeaturelayer
    {
        public ESRI.ArcGIS.Carto.IFeatureLayer2 CreateFeatureLayer(string name, ESRI.ArcGIS.Geometry.ISpatialReference spatialReference, string workspacename)
        {
            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = CreateFeatureClass(name, spatialReference, workspacename);
            ESRI.ArcGIS.Carto.IFeatureLayer2 fl = new ESRI.ArcGIS.Carto.FeatureLayer() as ESRI.ArcGIS.Carto.IFeatureLayer2;
            fl.FeatureClass = fc;
            ((ESRI.ArcGIS.Carto.ILayer)fl).Name = name;

            return fl;
        }

        public ESRI.ArcGIS.Geodatabase.IFeatureClass CreateFeatureClass(string name, ESRI.ArcGIS.Geometry.ISpatialReference spatialReference, string workspacename)
        {
            ESRI.ArcGIS.Geodatabase.IWorkspace w = WorkSpace.CreateInMemoryWorkspace(workspacename);

            ESRI.ArcGIS.Geodatabase.IFeatureWorkspace fw = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)w;


            ESRI.ArcGIS.Geodatabase.IFields2 fields = CreateFields(spatialReference);
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UID();
            ESRI.ArcGIS.Geodatabase.IFeatureClassDescription fcDesc = new ESRI.ArcGIS.Geodatabase.FeatureClassDescriptionClass();
            ESRI.ArcGIS.Geodatabase.IObjectClassDescription ocDesc = (ESRI.ArcGIS.Geodatabase.IObjectClassDescription)fcDesc;

            ESRI.ArcGIS.Geodatabase.IFeatureClass fc = fw.CreateFeatureClass(name, fields, ocDesc.InstanceCLSID, ocDesc.ClassExtensionCLSID, ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimple, "Shape", "");

            return fc;
        }

        public ESRI.ArcGIS.Geodatabase.IFieldEdit2 CreateField(string name, string alias, ESRI.ArcGIS.Geodatabase.esriFieldType fldtype, int length)
        {
            ESRI.ArcGIS.Geodatabase.IField field = new ESRI.ArcGIS.Geodatabase.FieldClass();
            ESRI.ArcGIS.Geodatabase.IFieldEdit2 fe = (ESRI.ArcGIS.Geodatabase.IFieldEdit2)field;
            fe.Length_2 = length;
            fe.Name_2 = name;
            fe.AliasName_2 = alias;
            fe.Type_2 = fldtype;

            return fe;
        }
        public virtual ESRI.ArcGIS.Geodatabase.IFields2 CreateFields(ESRI.ArcGIS.Geometry.ISpatialReference spatialReference){return null;}

       }

    class WorkSpace
    {
        internal static ESRI.ArcGIS.Geodatabase.IWorkspace CreateInMemoryWorkspace(string workspacename)
        {
            // Create an in-memory workspace factory.
            Type factoryType = Type.GetTypeFromProgID(
              "esriDataSourcesGDB.InMemoryWorkspaceFactory");
            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory workspaceFactory = (ESRI.ArcGIS.Geodatabase.IWorkspaceFactory)
              Activator.CreateInstance(factoryType);


            // Create an in-memory workspace.
            ESRI.ArcGIS.Geodatabase.IWorkspaceName workspaceName = workspaceFactory.Create("", workspacename,
              null, 0);

            // Cast for IName and open a reference to the in-memory workspace through the name object.
            ESRI.ArcGIS.esriSystem.IName name = (ESRI.ArcGIS.esriSystem.IName)workspaceName;

            ESRI.ArcGIS.Geodatabase.IWorkspace workspace = (ESRI.ArcGIS.Geodatabase.IWorkspace)name.Open();
            return workspace;
        }
    }

    /// <summary>
    /// CreateLinelayer - generate line feature layer
    /// </summary>
    class CreateLineLayer : CreateFeaturelayer
    {

        public override ESRI.ArcGIS.Geodatabase.IFields2 CreateFields(ESRI.ArcGIS.Geometry.ISpatialReference spatialReference)
        {
            ESRI.ArcGIS.Geodatabase.IFields2 fields = new ESRI.ArcGIS.Geodatabase.FieldsClass();
            ESRI.ArcGIS.Geodatabase.IFieldsEdit fieldsEdit = (ESRI.ArcGIS.Geodatabase.IFieldsEdit)fields;

            ESRI.ArcGIS.Geodatabase.IFieldEdit2 fieldedit = CreateField("ObjectID", "FID", ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeOID, 0);
            fieldsEdit.AddField(fieldedit);

            //add id
            fieldedit = CreateField("LineID", "LineID", ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGUID, 36);
            fieldedit.DefaultValue_2 = 0; // add default for letter
            fieldsEdit.AddField(fieldedit);

            // place holder for letter
            fieldedit = CreateField("LETTER", "LETTER",
                ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeString, 20);
            fieldedit.DefaultValue_2 = "X"; // add default for letter
            fieldsEdit.AddField(fieldedit);

            // just another field
            fieldedit = CreateField("NOTES", "NOTES",
                ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeString, 225);
            fieldsEdit.AddField(fieldedit);

            // add geomtype
            ESRI.ArcGIS.Geodatabase.IGeometryDef geometryDef = new ESRI.ArcGIS.Geodatabase.GeometryDefClass();
            ESRI.ArcGIS.Geodatabase.IGeometryDefEdit geometryDefEdit = (ESRI.ArcGIS.Geodatabase.IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline;

            ESRI.ArcGIS.Geometry.ISpatialReferenceResolution spatialReferenceResolution = (ESRI.ArcGIS.Geometry.ISpatialReferenceResolution)spatialReference;
            ESRI.ArcGIS.Geometry.ISpatialReferenceTolerance spatialReferenceTolerance = (ESRI.ArcGIS.Geometry.ISpatialReferenceTolerance)spatialReference;
            spatialReferenceTolerance.SetDefaultXYTolerance();
            geometryDefEdit.SpatialReference_2 = spatialReference;

            ESRI.ArcGIS.Geodatabase.IField geometryField = new ESRI.ArcGIS.Geodatabase.FieldClass();
            ESRI.ArcGIS.Geodatabase.IFieldEdit geometryFieldEdit = (ESRI.ArcGIS.Geodatabase.IFieldEdit)geometryField;
            geometryFieldEdit.Name_2 = "Shape";
            geometryFieldEdit.Type_2 = ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry;
            geometryFieldEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(geometryField);

            return fields;
        }

       
    }

   
    /// <summary>
    /// Layers - select/find layers for the map
    /// </summary>
    class Layers
    {
        internal static ESRI.ArcGIS.Carto.IFeatureLayer2 getFeatureLayerByName(string name)
        {
            if (name.Length > 0)
            {
                ESRI.ArcGIS.Carto.ILayer layer = getLayerByName(name);
                if (layer != null)
                {
                    ESRI.ArcGIS.Carto.IFeatureLayer2 featlayer = (ESRI.ArcGIS.Carto.IFeatureLayer2)layer;
                    return featlayer;
                }
            }
            return null;
        }

        internal static ESRI.ArcGIS.Carto.ILayer getLayerByName(string name)
        {
            ESRI.ArcGIS.Carto.IMap map = ArcMap.Document.FocusMap;
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";


            ESRI.ArcGIS.Carto.IEnumLayer enumLayer = map.get_Layers(null, true);
            ESRI.ArcGIS.Carto.ILayer l = enumLayer.Next();
            while (l != null)
            {
                if (l.Name.ToUpper() == name.ToUpper())
                    break;

                l = enumLayer.Next();
            }

            return l;
        }

        /// <summary>
        /// return Spatial Reference
        /// </summary>
        /// <returns>ISpatialReference of wgs84</returns>
        internal static ESRI.ArcGIS.Geometry.ISpatialReference returnSR()
        {
            // wgs84
            ESRI.ArcGIS.Geometry.ISpatialReferenceFactory spatialReferenceFactory = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();
            ESRI.ArcGIS.Geometry.ISpatialReference spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984);
            ESRI.ArcGIS.Geometry.ISpatialReferenceResolution spatialReferenceResolution = (ESRI.ArcGIS.Geometry.ISpatialReferenceResolution)spatialReference;
            return spatialReference;

        }

    }
}
