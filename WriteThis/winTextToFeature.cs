using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Runtime.InteropServices;

/// All this tool does is write the user text to a polyline featureclass
/// Map document needs to be in WGS84 .
namespace WriteThis
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class winTextToFeature : UserControl
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Unregister(regKey);

        }

        #endregion
        #endregion
        public winTextToFeature(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private winTextToFeature m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new winTextToFeature(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            // for now map must be wgs84
            if (ArcMap.Document.FocusMap.SpatialReference == null)
            {
                MessageBox.Show("Please put Map Document Data Frame into - Geographic Coordinate System: WGS 1984");
                return;
            }
            if (ArcMap.Document.FocusMap.SpatialReference.FactoryCode != 4326)
            {
                MessageBox.Show("Please put Map Document Data Frame into - Geographic Coordinate System: WGS 1984");
                return;
            }


            // convert text
            if (txtText.Text.Length > 0)
            {
                // convert and collect text info
                code.TextGeomsStruct.TextGeomCollection tgCollection = new code.TextGeomsStruct.TextGeomCollection(txtText.Text);
                ESRI.ArcGIS.Carto.IFeatureLayer2 featLayer = (ESRI.ArcGIS.Carto.IFeatureLayer2)code.Layers.getLayerByName("MyLines");
                if (featLayer == null)
                {
                    //  create new layer to hold features
                    code.CreateLineLayer createLineFeatureClass = new code.CreateLineLayer();
                    featLayer =  createLineFeatureClass.CreateFeatureLayer("MyLines", code.Layers.returnSR(), "in_memory");
                    // featLayer = code.Layers.CreateFeatureLayer("MyLines", code.Layers.returnSR(), "in_memory");
                    ArcMap.Document.FocusMap.AddLayer((ESRI.ArcGIS.Carto.ILayer)featLayer);
                    // actually create the features and add them to the layer
                    bool b = tgCollection.CreateFeatures("MyLines");
                }
                else
                {
                    // TODO: this will cause stacked text if ran more then once. need to add parameter to shift new text
                    bool b = tgCollection.CreateFeatures("MyLines");
                }
                ArcMap.Document.ActiveView.PartialRefresh(ESRI.ArcGIS.Carto.esriViewDrawPhase.esriViewGeography, null, null);
            }
        }
    }
}
