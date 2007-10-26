//----------------------------------------------------------------------------
// NAME: Geoportail Plugin
// VERSION: 1.1
// DESCRIPTION: Maps provided by IGN at http://geoportail.fr
// DEVELOPER: Guilhem Vellut
// WEBSITE: http://thepochisuperstarmegashow.com
// REFERENCES: 
//----------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using Utility;
using System.Runtime.InteropServices;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using WorldWind;
using WorldWind.Net;
using WorldWind.Renderable;
using WorldWind.Terrain;


namespace TPSMS.Plugins {
	#region GEOPORTAILFORM
	public class GeoportailForm : System.Windows.Forms.Form {
		private System.Windows.Forms.CheckBox checkBoxMetr;
		private System.Windows.Forms.ComboBox comboBoxLayerMetr;
		private System.ComponentModel.Container components = null;

		private WorldWind.WorldWindow m_WorldWindow;
		public WorldWind.WorldWindow WorldWindow {
			get{return m_WorldWindow;}
		}
		
		private GeoportailTilesLayer geoTilesLayer;
		private System.Windows.Forms.Button btnTrimCache;
	
		public GeoportailTilesLayer GeoTilesLayer {
			get{return geoTilesLayer;}
		}

		public int LimitLevel;

		
		
		public bool IsTerrainOn = true;
		private System.Windows.Forms.CheckBox checkBoxIsOn;
		private System.Windows.Forms.CheckBox checkBoxSPM;
		private System.Windows.Forms.ComboBox comboBoxLayerSPM;
		private System.Windows.Forms.CheckBox checkBoxKerg;
		private System.Windows.Forms.ComboBox comboBoxLayerKerg;
		private System.Windows.Forms.CheckBox checkBoxGuad;
		private System.Windows.Forms.CheckBox checkBoxMart;
		private System.Windows.Forms.ComboBox comboBoxLayerGuad;
		private System.Windows.Forms.ComboBox comboBoxLayerMart;
		private System.Windows.Forms.CheckBox checkBoxGuya;
		private System.Windows.Forms.ComboBox comboBoxLayerGuya;
		private System.Windows.Forms.ComboBox comboBoxLayerWafu;
		private System.Windows.Forms.CheckBox checkBoxWafu;
		private System.Windows.Forms.ComboBox comboBoxLayerReun;
		private System.Windows.Forms.CheckBox checkBoxReun;
		private System.Windows.Forms.ComboBox comboBoxLayerMayo;
		private System.Windows.Forms.CheckBox checkBoxMayo;
		private System.Windows.Forms.ComboBox comboBoxLayerCroz;
		private System.Windows.Forms.CheckBox checkBoxCroz;
		private System.Windows.Forms.ComboBox comboBoxLayerNcal;
		private System.Windows.Forms.CheckBox checkBoxNcal;
		private System.Windows.Forms.Button buttonMetrGo;
		private System.Windows.Forms.Button buttonSPMGo;
		private System.Windows.Forms.Button buttonKergGo;
		private System.Windows.Forms.Button buttonGuadGo;
		private System.Windows.Forms.Button buttonMartGo;
		private System.Windows.Forms.Button buttonGuyaGo;
		private System.Windows.Forms.Button buttonWafuGo;
		private System.Windows.Forms.Button buttonReunGo;
		private System.Windows.Forms.Button buttonMayoGo;
		private System.Windows.Forms.Button buttonNcalGo;
		private System.Windows.Forms.Button buttonCrozGo;

		private string cacheDirectory;

		private void InitLayerState(){
			geoTilesLayer.SetLayerForTerritory("metr","bdortho");
			checkBoxMetr.Checked = true;
			comboBoxLayerMetr.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("spm","scan");
			checkBoxSPM.Checked = true;
			comboBoxLayerSPM.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("kerg","scan");
			checkBoxKerg.Checked = true;
			comboBoxLayerKerg.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("guad","bdortho");
			checkBoxGuad.Checked = true;
			comboBoxLayerGuad.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("mart","bdortho");
			checkBoxMart.Checked = true;
			comboBoxLayerMart.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("guya","bdortho");
			checkBoxGuya.Checked = true;
			comboBoxLayerGuya.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("wafu","bdortho");
			checkBoxWafu.Checked = true;
			comboBoxLayerWafu.SelectedIndex = 0;
			
			geoTilesLayer.SetLayerForTerritory("reun","bdortho");
			checkBoxReun.Checked = true;
			comboBoxLayerReun.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("mayo","bdortho");
			checkBoxMayo.Checked = true;
			comboBoxLayerMayo.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("ncal","scan");
			checkBoxNcal.Checked = true;
			comboBoxLayerNcal.SelectedIndex = 0;

			geoTilesLayer.SetLayerForTerritory("croz","scan");
			checkBoxCroz.Checked = true;
			comboBoxLayerCroz.SelectedIndex = 0;
		}

		public GeoportailForm(MainApplication parentApplication) {
			InitializeComponent();

			checkBoxIsOn.Checked = true;

			try {
				m_WorldWindow = parentApplication.WorldWindow;

				//Set the PROJ_LIB env var
				

				//Initialize the metadata for territories
				TerritoryMetadata metadata = new TerritoryMetadata(parentApplication);

				//Create the GeoTilesLayer
				geoTilesLayer = new GeoportailTilesLayer("Geoportail Tiles",metadata, parentApplication, this);
				geoTilesLayer.IsOn = true;
				
				//Initialize the territories to display initially
				InitLayerState();

				lock(m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects.SyncRoot) {
					foreach(WorldWind.Renderable.RenderableObject ro in m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects) {
						if(ro is WorldWind.Renderable.RenderableObjectList && ro.Name.IndexOf("Images") >= 0) {
							WorldWind.Renderable.RenderableObjectList imagesList = ro as WorldWind.Renderable.RenderableObjectList;
							//insert it at the end of the list
							imagesList.ChildObjects.Insert(imagesList.ChildObjects.Count - 1, geoTilesLayer);
							break;
						}
					}
				}

				cacheDirectory = String.Format("{0}\\Geoportail", m_WorldWindow.Cache.CacheDirectory);
				if(Directory.Exists(cacheDirectory) == true) {
					DirectoryInfo diCache = new DirectoryInfo(cacheDirectory);
					//for debug, delete the entire cache
					//diCache.Delete(true);
				}
				

				//check that proj.dll is installed correctly, else set plugin to off
				string projDllPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\proj.dll";
				if(! File.Exists(projDllPath)) {
					//TODO turned off for debugging
					geoTilesLayer.IsOn = false;
					throw new Exception("'proj.dll' needs to be in the same directory where WorldWind.exe is installed");
				}
				
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
				throw;
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnClosing(CancelEventArgs e) {
			e.Cancel = true;
			this.Visible = false;
			base.OnClosing (e);
		}

		private void btnTrimCache_Click(object sender, System.EventArgs e) {
			try {
				//possibly iter the dirs and delete old tiles
				if(Directory.Exists(cacheDirectory) == true) {
					DirectoryInfo diCache = new DirectoryInfo(cacheDirectory);
					int numDays = 7; //Make this configurable?
					DateTime cutOffDate = DateTime.Now.AddDays(numDays);
					RecurseDeleteOldFiles(diCache, cutOffDate);
				}
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
			}
		}

		public void RecurseDeleteOldFiles(DirectoryInfo di, DateTime cutOffDate) {
			foreach(FileInfo fi in di.GetFiles("*.jpg")) {
				if(fi.CreationTime < cutOffDate) {
					fi.Delete();
				}
			}
			foreach(FileInfo fi in di.GetFiles("*.jpeg")) {
				if(fi.CreationTime < cutOffDate) {
					fi.Delete();
				}
			}
			foreach(DirectoryInfo tempDi in di.GetDirectories()) {
				RecurseDeleteOldFiles(tempDi, cutOffDate);
			}
		}

		#region Code généré par le Concepteur Windows Form
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent() {
			this.checkBoxMetr = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerMetr = new System.Windows.Forms.ComboBox();
			this.btnTrimCache = new System.Windows.Forms.Button();
			this.checkBoxIsOn = new System.Windows.Forms.CheckBox();
			this.checkBoxSPM = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerSPM = new System.Windows.Forms.ComboBox();
			this.checkBoxKerg = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerKerg = new System.Windows.Forms.ComboBox();
			this.checkBoxGuad = new System.Windows.Forms.CheckBox();
			this.checkBoxMart = new System.Windows.Forms.CheckBox();
			this.checkBoxGuya = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerGuad = new System.Windows.Forms.ComboBox();
			this.comboBoxLayerMart = new System.Windows.Forms.ComboBox();
			this.comboBoxLayerGuya = new System.Windows.Forms.ComboBox();
			this.comboBoxLayerWafu = new System.Windows.Forms.ComboBox();
			this.checkBoxWafu = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerReun = new System.Windows.Forms.ComboBox();
			this.checkBoxReun = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerMayo = new System.Windows.Forms.ComboBox();
			this.checkBoxMayo = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerCroz = new System.Windows.Forms.ComboBox();
			this.checkBoxCroz = new System.Windows.Forms.CheckBox();
			this.comboBoxLayerNcal = new System.Windows.Forms.ComboBox();
			this.checkBoxNcal = new System.Windows.Forms.CheckBox();
			this.buttonMetrGo = new System.Windows.Forms.Button();
			this.buttonSPMGo = new System.Windows.Forms.Button();
			this.buttonKergGo = new System.Windows.Forms.Button();
			this.buttonGuadGo = new System.Windows.Forms.Button();
			this.buttonMartGo = new System.Windows.Forms.Button();
			this.buttonGuyaGo = new System.Windows.Forms.Button();
			this.buttonWafuGo = new System.Windows.Forms.Button();
			this.buttonReunGo = new System.Windows.Forms.Button();
			this.buttonMayoGo = new System.Windows.Forms.Button();
			this.buttonNcalGo = new System.Windows.Forms.Button();
			this.buttonCrozGo = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkBoxMetr
			// 
			this.checkBoxMetr.Location = new System.Drawing.Point(8, 16);
			this.checkBoxMetr.Name = "checkBoxMetr";
			this.checkBoxMetr.Size = new System.Drawing.Size(136, 24);
			this.checkBoxMetr.TabIndex = 0;
			this.checkBoxMetr.Text = "Continental France";
			this.checkBoxMetr.CheckedChanged += new System.EventHandler(this.checkBoxMetr_CheckedChanged);
			// 
			// comboBoxLayerMetr
			// 
			this.comboBoxLayerMetr.Items.AddRange(new object[] {
																   "Aerial",
																   "Street",
																   "Scan",
																   "Altitude"});
			this.comboBoxLayerMetr.Location = new System.Drawing.Point(160, 16);
			this.comboBoxLayerMetr.Name = "comboBoxLayerMetr";
			this.comboBoxLayerMetr.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerMetr.TabIndex = 1;
			this.comboBoxLayerMetr.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerMetr_SelectedIndexChanged);
			// 
			// btnTrimCache
			// 
			this.btnTrimCache.Location = new System.Drawing.Point(8, 376);
			this.btnTrimCache.Name = "btnTrimCache";
			this.btnTrimCache.Size = new System.Drawing.Size(112, 23);
			this.btnTrimCache.TabIndex = 3;
			this.btnTrimCache.Text = "Trim Cache";
			this.btnTrimCache.Click += new System.EventHandler(this.btnTrimCache_Click);
			// 
			// checkBoxIsOn
			// 
			this.checkBoxIsOn.Location = new System.Drawing.Point(160, 376);
			this.checkBoxIsOn.Name = "checkBoxIsOn";
			this.checkBoxIsOn.TabIndex = 4;
			this.checkBoxIsOn.Text = "Terrain On";
			this.checkBoxIsOn.CheckedChanged += new System.EventHandler(this.checkBoxIsOn_CheckedChanged);
			// 
			// checkBoxSPM
			// 
			this.checkBoxSPM.Location = new System.Drawing.Point(8, 48);
			this.checkBoxSPM.Name = "checkBoxSPM";
			this.checkBoxSPM.Size = new System.Drawing.Size(136, 24);
			this.checkBoxSPM.TabIndex = 5;
			this.checkBoxSPM.Text = "St-Pierre-et-Miquelon";
			this.checkBoxSPM.CheckedChanged += new System.EventHandler(this.checkBoxSPM_CheckedChanged);
			// 
			// comboBoxLayerSPM
			// 
			this.comboBoxLayerSPM.Items.AddRange(new object[] {
																  "Scan"});
			this.comboBoxLayerSPM.Location = new System.Drawing.Point(160, 48);
			this.comboBoxLayerSPM.Name = "comboBoxLayerSPM";
			this.comboBoxLayerSPM.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerSPM.TabIndex = 6;
			this.comboBoxLayerSPM.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerSPM_SelectedIndexChanged);
			// 
			// checkBoxKerg
			// 
			this.checkBoxKerg.Location = new System.Drawing.Point(8, 80);
			this.checkBoxKerg.Name = "checkBoxKerg";
			this.checkBoxKerg.Size = new System.Drawing.Size(136, 24);
			this.checkBoxKerg.TabIndex = 7;
			this.checkBoxKerg.Text = "Kerguelen";
			this.checkBoxKerg.CheckedChanged += new System.EventHandler(this.checkBoxKerg_CheckedChanged);
			// 
			// comboBoxLayerKerg
			// 
			this.comboBoxLayerKerg.Items.AddRange(new object[] {
																   "Scan"});
			this.comboBoxLayerKerg.Location = new System.Drawing.Point(160, 80);
			this.comboBoxLayerKerg.Name = "comboBoxLayerKerg";
			this.comboBoxLayerKerg.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerKerg.TabIndex = 8;
			this.comboBoxLayerKerg.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerKerg_SelectedIndexChanged);
			// 
			// checkBoxGuad
			// 
			this.checkBoxGuad.Location = new System.Drawing.Point(8, 112);
			this.checkBoxGuad.Name = "checkBoxGuad";
			this.checkBoxGuad.Size = new System.Drawing.Size(136, 24);
			this.checkBoxGuad.TabIndex = 9;
			this.checkBoxGuad.Text = "Guadeloupe";
			this.checkBoxGuad.CheckedChanged += new System.EventHandler(this.checkBoxGuad_CheckedChanged);
			// 
			// checkBoxMart
			// 
			this.checkBoxMart.Location = new System.Drawing.Point(8, 144);
			this.checkBoxMart.Name = "checkBoxMart";
			this.checkBoxMart.TabIndex = 10;
			this.checkBoxMart.Text = "Martinique";
			this.checkBoxMart.CheckedChanged += new System.EventHandler(this.checkBoxMart_CheckedChanged);
			// 
			// checkBoxGuya
			// 
			this.checkBoxGuya.Location = new System.Drawing.Point(8, 176);
			this.checkBoxGuya.Name = "checkBoxGuya";
			this.checkBoxGuya.TabIndex = 11;
			this.checkBoxGuya.Text = "Guyane";
			this.checkBoxGuya.CheckedChanged += new System.EventHandler(this.checkBoxGuya_CheckedChanged);
			// 
			// comboBoxLayerGuad
			// 
			this.comboBoxLayerGuad.Items.AddRange(new object[] {
																   "Aerial",
																   "Scan",
																   "Altitude"});
			this.comboBoxLayerGuad.Location = new System.Drawing.Point(160, 112);
			this.comboBoxLayerGuad.Name = "comboBoxLayerGuad";
			this.comboBoxLayerGuad.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerGuad.TabIndex = 12;
			this.comboBoxLayerGuad.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerGuad_SelectedIndexChanged);
			// 
			// comboBoxLayerMart
			// 
			this.comboBoxLayerMart.Items.AddRange(new object[] {
																   "Aerial",
																   "Scan",
																   "Altitude"});
			this.comboBoxLayerMart.Location = new System.Drawing.Point(160, 144);
			this.comboBoxLayerMart.Name = "comboBoxLayerMart";
			this.comboBoxLayerMart.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerMart.TabIndex = 13;
			this.comboBoxLayerMart.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerMart_SelectedIndexChanged);
			// 
			// comboBoxLayerGuya
			// 
			this.comboBoxLayerGuya.Items.AddRange(new object[] {
																   "Aerial",
																   "Scan",
																   "Altitude"});
			this.comboBoxLayerGuya.Location = new System.Drawing.Point(160, 176);
			this.comboBoxLayerGuya.Name = "comboBoxLayerGuya";
			this.comboBoxLayerGuya.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerGuya.TabIndex = 14;
			this.comboBoxLayerGuya.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerGuya_SelectedIndexChanged);
			// 
			// comboBoxLayerWafu
			// 
			this.comboBoxLayerWafu.Items.AddRange(new object[] {
																   "Aerial",
																   "Scan"});
			this.comboBoxLayerWafu.Location = new System.Drawing.Point(160, 208);
			this.comboBoxLayerWafu.Name = "comboBoxLayerWafu";
			this.comboBoxLayerWafu.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerWafu.TabIndex = 16;
			this.comboBoxLayerWafu.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerWafu_SelectedIndexChanged);
			// 
			// checkBoxWafu
			// 
			this.checkBoxWafu.Location = new System.Drawing.Point(8, 208);
			this.checkBoxWafu.Name = "checkBoxWafu";
			this.checkBoxWafu.TabIndex = 15;
			this.checkBoxWafu.Text = "Wallis / Futuna";
			this.checkBoxWafu.CheckedChanged += new System.EventHandler(this.checkBoxWafu_CheckedChanged);
			// 
			// comboBoxLayerReun
			// 
			this.comboBoxLayerReun.Items.AddRange(new object[] {
																   "Aerial",
																   "Scan",
																   "Altitude"});
			this.comboBoxLayerReun.Location = new System.Drawing.Point(160, 240);
			this.comboBoxLayerReun.Name = "comboBoxLayerReun";
			this.comboBoxLayerReun.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerReun.TabIndex = 18;
			this.comboBoxLayerReun.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerReun_SelectedIndexChanged);
			// 
			// checkBoxReun
			// 
			this.checkBoxReun.Location = new System.Drawing.Point(8, 240);
			this.checkBoxReun.Name = "checkBoxReun";
			this.checkBoxReun.TabIndex = 17;
			this.checkBoxReun.Text = "Réunion";
			this.checkBoxReun.CheckedChanged += new System.EventHandler(this.checkBoxReun_CheckedChanged);
			// 
			// comboBoxLayerMayo
			// 
			this.comboBoxLayerMayo.Items.AddRange(new object[] {
																   "Aerial",
																   "Scan",
																   "Altitude"});
			this.comboBoxLayerMayo.Location = new System.Drawing.Point(160, 272);
			this.comboBoxLayerMayo.Name = "comboBoxLayerMayo";
			this.comboBoxLayerMayo.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerMayo.TabIndex = 20;
			this.comboBoxLayerMayo.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerMayo_SelectedIndexChanged);
			// 
			// checkBoxMayo
			// 
			this.checkBoxMayo.Location = new System.Drawing.Point(8, 272);
			this.checkBoxMayo.Name = "checkBoxMayo";
			this.checkBoxMayo.TabIndex = 19;
			this.checkBoxMayo.Text = "Mayotte";
			this.checkBoxMayo.CheckedChanged += new System.EventHandler(this.checkBoxMayo_CheckedChanged);
			// 
			// comboBoxLayerCroz
			// 
			this.comboBoxLayerCroz.Items.AddRange(new object[] {
																   "Scan"});
			this.comboBoxLayerCroz.Location = new System.Drawing.Point(160, 336);
			this.comboBoxLayerCroz.Name = "comboBoxLayerCroz";
			this.comboBoxLayerCroz.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerCroz.TabIndex = 24;
			this.comboBoxLayerCroz.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerCroz_SelectedIndexChanged);
			// 
			// checkBoxCroz
			// 
			this.checkBoxCroz.Location = new System.Drawing.Point(8, 336);
			this.checkBoxCroz.Name = "checkBoxCroz";
			this.checkBoxCroz.TabIndex = 23;
			this.checkBoxCroz.Text = "Crozet";
			this.checkBoxCroz.CheckedChanged += new System.EventHandler(this.checkBoxCroz_CheckedChanged);
			// 
			// comboBoxLayerNcal
			// 
			this.comboBoxLayerNcal.Items.AddRange(new object[] {
																   "Scan"});
			this.comboBoxLayerNcal.Location = new System.Drawing.Point(160, 304);
			this.comboBoxLayerNcal.Name = "comboBoxLayerNcal";
			this.comboBoxLayerNcal.Size = new System.Drawing.Size(121, 21);
			this.comboBoxLayerNcal.TabIndex = 22;
			this.comboBoxLayerNcal.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerNcal_SelectedIndexChanged);
			// 
			// checkBoxNcal
			// 
			this.checkBoxNcal.Location = new System.Drawing.Point(8, 304);
			this.checkBoxNcal.Name = "checkBoxNcal";
			this.checkBoxNcal.Size = new System.Drawing.Size(136, 24);
			this.checkBoxNcal.TabIndex = 21;
			this.checkBoxNcal.Text = "Nouvelle-Calédonie";
			this.checkBoxNcal.CheckedChanged += new System.EventHandler(this.checkBoxNcal_CheckedChanged);
			// 
			// buttonMetrGo
			// 
			this.buttonMetrGo.Location = new System.Drawing.Point(288, 16);
			this.buttonMetrGo.Name = "buttonMetrGo";
			this.buttonMetrGo.Size = new System.Drawing.Size(32, 23);
			this.buttonMetrGo.TabIndex = 25;
			this.buttonMetrGo.Text = "Go";
			this.buttonMetrGo.Click += new System.EventHandler(this.buttonMetrGo_Click);
			// 
			// buttonSPMGo
			// 
			this.buttonSPMGo.Location = new System.Drawing.Point(288, 48);
			this.buttonSPMGo.Name = "buttonSPMGo";
			this.buttonSPMGo.Size = new System.Drawing.Size(32, 23);
			this.buttonSPMGo.TabIndex = 26;
			this.buttonSPMGo.Text = "Go";
			this.buttonSPMGo.Click += new System.EventHandler(this.buttonSPMGo_Click);
			// 
			// buttonKergGo
			// 
			this.buttonKergGo.Location = new System.Drawing.Point(288, 80);
			this.buttonKergGo.Name = "buttonKergGo";
			this.buttonKergGo.Size = new System.Drawing.Size(32, 23);
			this.buttonKergGo.TabIndex = 27;
			this.buttonKergGo.Text = "Go";
			this.buttonKergGo.Click += new System.EventHandler(this.buttonKergGo_Click);
			// 
			// buttonGuadGo
			// 
			this.buttonGuadGo.Location = new System.Drawing.Point(288, 112);
			this.buttonGuadGo.Name = "buttonGuadGo";
			this.buttonGuadGo.Size = new System.Drawing.Size(32, 23);
			this.buttonGuadGo.TabIndex = 28;
			this.buttonGuadGo.Text = "Go";
			this.buttonGuadGo.Click += new System.EventHandler(this.buttonGuadGo_Click);
			// 
			// buttonMartGo
			// 
			this.buttonMartGo.Location = new System.Drawing.Point(288, 144);
			this.buttonMartGo.Name = "buttonMartGo";
			this.buttonMartGo.Size = new System.Drawing.Size(32, 23);
			this.buttonMartGo.TabIndex = 29;
			this.buttonMartGo.Text = "Go";
			this.buttonMartGo.Click += new System.EventHandler(this.buttonMartGo_Click);
			// 
			// buttonGuyaGo
			// 
			this.buttonGuyaGo.Location = new System.Drawing.Point(288, 176);
			this.buttonGuyaGo.Name = "buttonGuyaGo";
			this.buttonGuyaGo.Size = new System.Drawing.Size(32, 23);
			this.buttonGuyaGo.TabIndex = 30;
			this.buttonGuyaGo.Text = "Go";
			this.buttonGuyaGo.Click += new System.EventHandler(this.buttonGuyaGo_Click);
			// 
			// buttonWafuGo
			// 
			this.buttonWafuGo.Location = new System.Drawing.Point(288, 208);
			this.buttonWafuGo.Name = "buttonWafuGo";
			this.buttonWafuGo.Size = new System.Drawing.Size(32, 23);
			this.buttonWafuGo.TabIndex = 31;
			this.buttonWafuGo.Text = "Go";
			this.buttonWafuGo.Click += new System.EventHandler(this.buttonWafuGo_Click);
			// 
			// buttonReunGo
			// 
			this.buttonReunGo.Location = new System.Drawing.Point(288, 240);
			this.buttonReunGo.Name = "buttonReunGo";
			this.buttonReunGo.Size = new System.Drawing.Size(32, 23);
			this.buttonReunGo.TabIndex = 32;
			this.buttonReunGo.Text = "Go";
			this.buttonReunGo.Click += new System.EventHandler(this.buttonReunGo_Click);
			// 
			// buttonMayoGo
			// 
			this.buttonMayoGo.Location = new System.Drawing.Point(288, 272);
			this.buttonMayoGo.Name = "buttonMayoGo";
			this.buttonMayoGo.Size = new System.Drawing.Size(32, 23);
			this.buttonMayoGo.TabIndex = 33;
			this.buttonMayoGo.Text = "Go";
			this.buttonMayoGo.Click += new System.EventHandler(this.buttonMayoGo_Click);
			// 
			// buttonNcalGo
			// 
			this.buttonNcalGo.Location = new System.Drawing.Point(288, 304);
			this.buttonNcalGo.Name = "buttonNcalGo";
			this.buttonNcalGo.Size = new System.Drawing.Size(32, 23);
			this.buttonNcalGo.TabIndex = 34;
			this.buttonNcalGo.Text = "Go";
			this.buttonNcalGo.Click += new System.EventHandler(this.buttonNcalGo_Click);
			// 
			// buttonCrozGo
			// 
			this.buttonCrozGo.Location = new System.Drawing.Point(288, 336);
			this.buttonCrozGo.Name = "buttonCrozGo";
			this.buttonCrozGo.Size = new System.Drawing.Size(32, 23);
			this.buttonCrozGo.TabIndex = 35;
			this.buttonCrozGo.Text = "Go";
			this.buttonCrozGo.Click += new System.EventHandler(this.buttonCrozGo_Click);
			// 
			// GeoportailForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 405);
			this.Controls.Add(this.buttonCrozGo);
			this.Controls.Add(this.buttonNcalGo);
			this.Controls.Add(this.buttonMayoGo);
			this.Controls.Add(this.buttonReunGo);
			this.Controls.Add(this.buttonWafuGo);
			this.Controls.Add(this.buttonGuyaGo);
			this.Controls.Add(this.buttonMartGo);
			this.Controls.Add(this.buttonGuadGo);
			this.Controls.Add(this.buttonKergGo);
			this.Controls.Add(this.buttonSPMGo);
			this.Controls.Add(this.buttonMetrGo);
			this.Controls.Add(this.comboBoxLayerCroz);
			this.Controls.Add(this.checkBoxCroz);
			this.Controls.Add(this.comboBoxLayerNcal);
			this.Controls.Add(this.checkBoxNcal);
			this.Controls.Add(this.comboBoxLayerMayo);
			this.Controls.Add(this.checkBoxMayo);
			this.Controls.Add(this.comboBoxLayerReun);
			this.Controls.Add(this.checkBoxReun);
			this.Controls.Add(this.comboBoxLayerWafu);
			this.Controls.Add(this.checkBoxWafu);
			this.Controls.Add(this.comboBoxLayerGuya);
			this.Controls.Add(this.comboBoxLayerMart);
			this.Controls.Add(this.comboBoxLayerGuad);
			this.Controls.Add(this.checkBoxGuya);
			this.Controls.Add(this.checkBoxMart);
			this.Controls.Add(this.checkBoxGuad);
			this.Controls.Add(this.comboBoxLayerKerg);
			this.Controls.Add(this.checkBoxKerg);
			this.Controls.Add(this.comboBoxLayerSPM);
			this.Controls.Add(this.checkBoxSPM);
			this.Controls.Add(this.checkBoxIsOn);
			this.Controls.Add(this.btnTrimCache);
			this.Controls.Add(this.comboBoxLayerMetr);
			this.Controls.Add(this.checkBoxMetr);
			this.Name = "GeoportailForm";
			this.Text = "Geoportail Plugin";
			this.ResumeLayout(false);

		}
		#endregion


		private void checkBoxIsOn_CheckedChanged(object sender, System.EventArgs e) {
			if(geoTilesLayer != null){
				if(checkBoxIsOn.Checked){
					geoTilesLayer.IsOn = true;
				}else{
					geoTilesLayer.IsOn = false;
					geoTilesLayer.RemoveAllTiles();
				}
			}
		}

		private void checkBoxMetr_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxMetr.Checked){
				comboBoxLayerMetr.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("metr",getMetrLayer(comboBoxLayerMetr.SelectedIndex));	
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("metr");
				comboBoxLayerMetr.Enabled = false;
			}
		}

		
		private void checkBoxSPM_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxSPM.Checked){
				comboBoxLayerSPM.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("spm",getSPMLayer(comboBoxLayerSPM.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("spm");
				comboBoxLayerSPM.Enabled = false;
			}
		}

		private void checkBoxKerg_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxKerg.Checked){
				comboBoxLayerKerg.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("kerg",getKergLayer(comboBoxLayerKerg.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("kerg");
				comboBoxLayerKerg.Enabled = false;
			}
		}

		//Should refactor these...
		private void checkBoxGuad_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxGuad.Checked){
				comboBoxLayerGuad.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("guad",getGuadLayer(comboBoxLayerGuad.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("guad");
				comboBoxLayerGuad.Enabled = false;
			}
		}

		private void checkBoxMart_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxMart.Checked){
				comboBoxLayerMart.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("mart",getMartLayer(comboBoxLayerMart.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("mart");
				comboBoxLayerMart.Enabled = false;
			}
		}

		private void checkBoxGuya_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxGuya.Checked){
				comboBoxLayerGuya.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("guya",getGuyaLayer(comboBoxLayerGuya.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("guya");
				comboBoxLayerGuya.Enabled = false;
			}
		}

		private void checkBoxWafu_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxWafu.Checked){
				comboBoxLayerWafu.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("wafu",getWafuLayer(comboBoxLayerWafu.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("wafu");
				comboBoxLayerWafu.Enabled = false;
			}
		}

		private void checkBoxReun_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxReun.Checked){
				comboBoxLayerReun.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("reun",getReunLayer(comboBoxLayerReun.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("reun");
				comboBoxLayerReun.Enabled = false;
			}
		}

		private void checkBoxMayo_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxMayo.Checked){
				comboBoxLayerMayo.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("mayo",getMayoLayer(comboBoxLayerMayo.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("mayo");
				comboBoxLayerMayo.Enabled = false;
			}
		}

		private void checkBoxNcal_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxNcal.Checked){
				comboBoxLayerNcal.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("ncal",getNcalLayer(comboBoxLayerNcal.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("ncal");
				comboBoxLayerNcal.Enabled = false;
			}
		}

		private void checkBoxCroz_CheckedChanged(object sender, System.EventArgs e) {
			if(checkBoxCroz.Checked){
				comboBoxLayerCroz.Enabled = true;
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("croz",getCrozLayer(comboBoxLayerCroz.SelectedIndex));
			}else{
				if(geoTilesLayer != null)
					geoTilesLayer.removeTerritory("croz");
				comboBoxLayerCroz.Enabled = false;
			}
		}

		private string getMetrLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "grk";
				case 2:
					return "scan";
				case 3:
					return "alti";
				default:
					return "";
			}
		}

		private string getSPMLayer(int index){
			switch(index){
				case 0:
					return "scan";
				default:
					return "";
			}
		}

		private string getKergLayer(int index){
			switch(index){
				case 0:
					return "scan";
				default:
					return "";
			}
		}

		private string getGuadLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "scan";
				case 2:
					return "alti";
				default:
					return "";
			}
		}

		private string getMartLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "scan";
				case 2:
					return "alti";
				default:
					return "";
			}
		}

		private string getGuyaLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "scan";
				case 2:
					return "alti";
				default:
					return "";
			}
		}

		private string getWafuLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "scan";
				default:
					return "";
			}
		}

		private string getReunLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "scan";
				case 2:
					return "alti";
				default:
					return "";
			}
		}	

		private string getMayoLayer(int index){
			switch(index){
				case 0:
					return "bdortho";
				case 1:
					return "scan";
				case 2:
					return "alti";
				default:
					return "";
			}
		}

		private string getNcalLayer(int index){
			switch(index){
				case 0:
					return "scan";
				default:
					return "";
			}
		}

		private string getCrozLayer(int index){
			switch(index){
				case 0:
					return "scan";
				default:
					return "";
			}
		}

		private void comboBoxLayerMetr_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxMetr.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("metr",getMetrLayer(comboBoxLayerMetr.SelectedIndex));
			}
		}

		private void comboBoxLayerSPM_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxSPM.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("spm",getSPMLayer(comboBoxLayerSPM.SelectedIndex));
			}
		}

		private void comboBoxLayerKerg_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxKerg.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("kerg",getKergLayer(comboBoxLayerKerg.SelectedIndex));
			}
		}

		private void comboBoxLayerGuad_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxGuad.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("guad",getGuadLayer(comboBoxLayerGuad.SelectedIndex));
			}
		}

		private void comboBoxLayerMart_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxMart.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("mart",getMartLayer(comboBoxLayerMart.SelectedIndex));
			}
		}

		private void comboBoxLayerGuya_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxGuya.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("guya",getGuyaLayer(comboBoxLayerGuya.SelectedIndex));
			}
		}

		private void comboBoxLayerWafu_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxWafu.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("wafu",getWafuLayer(comboBoxLayerWafu.SelectedIndex));
			}
		}

		private void comboBoxLayerReun_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxReun.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("reun",getReunLayer(comboBoxLayerReun.SelectedIndex));
			}
		}

		private void comboBoxLayerMayo_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxMayo.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("mayo",getMayoLayer(comboBoxLayerMayo.SelectedIndex));
			}
		}

		private void comboBoxLayerNcal_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxNcal.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("ncal",getNcalLayer(comboBoxLayerNcal.SelectedIndex));
			}
		}

		private void comboBoxLayerCroz_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(checkBoxCroz.Checked){
				if(geoTilesLayer != null)
					geoTilesLayer.SetLayerForTerritory("croz",getCrozLayer(comboBoxLayerCroz.SelectedIndex));
			}
		}

		private void buttonMetrGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(48.859899,2.3478743,0,55000,0,0);
		}

		private void buttonSPMGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(46.783356,-56.174347,0,8500,0,0);
		}

		private void buttonKergGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(-49.337941,69.161535,0,47000,0,0);
		}

		private void buttonGuadGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(16.225129,-61.512027,0,3000,0,0);
		}

		private void buttonMartGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(14.613404,-61.075239,0,2000,0,0);
		}

		private void buttonGuyaGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(4.915331,-52.289359,0,10000,0,0);
		}

		private void buttonWafuGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(-13.2838,-176.178029,0,5000,0,0);
		}

		private void buttonReunGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(-21.33159,55.46760,0,11000,0,0);
		}

		private void buttonMayoGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(-12.89272,45.142549,0,3900,0,0);
		}

		private void buttonNcalGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(-22.25110,166.45958,0,20000,0,0);
		}

		private void buttonCrozGo_Click(object sender, System.EventArgs e) {
			WorldWindow.DrawArgs.WorldCamera.SetPosition(-46.404783,51.767925,0,29000,0,0);
		}


		
	}

	#endregion

	#region GEOPORTAILPLUGIN
	public class GeoportailPlugin : WorldWind.PluginEngine.Plugin {
		GeoportailForm m_Form = null;
		MenuItem m_MenuItem;
		WorldWind.WindowsControlMenuButton m_ToolbarItem;

		[DllImport("Kernel32.dll")] 
		public static extern int SetEnvironmentVariable( string name , string value ) ;
		
		public override void Load() {
			try {
				SetEnvironmentVariable("PROJ_LIB",Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\GeoportailPlugin\\NAD");

				if(ParentApplication.WorldWindow.CurrentWorld.Name.IndexOf("Earth") >= 0) {
					m_Form = new GeoportailForm(ParentApplication);
					m_Form.Owner = ParentApplication;

					m_MenuItem = new MenuItem("Geoportail");
					m_MenuItem.Click += new EventHandler(menuItemClicked);
					ParentApplication.PluginsMenu.MenuItems.Add( m_MenuItem );
			
					
					string imgPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\GeoportailPlugin\\GeoportailPlugin.png";
					
					if(!File.Exists(imgPath)) {
						Utility.Log.Write("imgPath not found " + imgPath);
					}
					m_ToolbarItem = new WorldWind.WindowsControlMenuButton(
						"Geoportail",
						imgPath,
						m_Form);
			
					ParentApplication.WorldWindow.MenuBar.AddToolsMenuButton(m_ToolbarItem);
			
					base.Load ();
				}
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
				throw;
			}
		}

		public override void Unload() {
			try {
				//remove from renderable objects
				m_Form.GeoTilesLayer.IsOn = false;
				ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Remove(m_Form.GeoTilesLayer);
				m_Form.GeoTilesLayer.Dispose();

				if(m_Form != null) {
					m_Form.Dispose();
					m_Form = null;
					ParentApplication.PluginsMenu.MenuItems.Remove( m_MenuItem );
					ParentApplication.WorldWindow.MenuBar.RemoveToolsMenuButton(m_ToolbarItem);
				}

				base.Unload ();
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
				throw;
			}
		}

		private void menuItemClicked(object sender, System.EventArgs e) {
			if(m_Form.Visible) {
				m_Form.Visible = false;
				m_MenuItem.Checked = false;
			}
			else {
				m_Form.Visible = true;
				m_MenuItem.Checked = true;
			}
		}
	}
	#endregion

	#region GEOPORTAILMETADATA
	public class Extent{
		public UV TopLeftUV, BottomRightUV;
		public Extent(UV tlUV, UV brUV){
			this.TopLeftUV = tlUV;
			this.BottomRightUV = brUV;
		}
		public Extent(double l,double t, double r, double b){
			this.TopLeftUV = new UV(l,t);
			this.BottomRightUV = new UV(r,b);
		}
															
		public bool Intersect(Extent extent){
			return ( TopLeftUV.U < extent.BottomRightUV.U
					&& BottomRightUV.U > extent.TopLeftUV.U 
					&& TopLeftUV.V > extent.BottomRightUV.V 
					&& BottomRightUV.V < extent.TopLeftUV.V 
					);
		}

		public bool Contains(UV point){
			return (point.U > TopLeftUV.U
					&& point.U < BottomRightUV.U
					&& point.V > BottomRightUV.V
					&& point.V < TopLeftUV.V);
		}
	}

	public class GeoportailTerritory{
		public String Name;
		public Hashtable Layers;
		public int ScaleMin, ScaleMax; //To know when to stop and start displaying the data for the territory

		public GeoportailTerritory(String name, int scaleMin, int scaleMax){
			this.Name = name;
			this.ScaleMin = scaleMin;
			this.ScaleMax = scaleMax;
			this.Layers = new Hashtable();
		}
		public void AddLayer(GeoportailLayer layer){
			Layers[layer.Name] = layer;
			layer.Territory = this;
		}
		public GeoportailLayer GetLayer(String name){
			return (GeoportailLayer) Layers[name];
		}
	}

	public class GeoportailLayer{
		public String Name;
		public Proj Projection;
		public Extent LayerExtent;
		public UV CellSize;
		public int Width, Height;
		public ArrayList Components;
		public ArrayList Levels;
		public String CopyrightNoticeUrl; //Error is returned if the copyright notice is not requested to the geoportail
		public int PrevRow = -1;
		public int PrevCol = -1;
		public GeoportailLayerLevel PrevLevel = null;
		public ArrayList GeoTiles;
		public GeoportailTerritory Territory;
		
		public GeoportailLayer(String name,Proj projection, Extent extent,UV cellSize, int width, int height, String copyrightNoticeUrl){
			this.Name = name;
			this.Projection = projection;
			this.LayerExtent = extent;
			this.CellSize = cellSize;
			this.Width = width;
			this.Height = height;
			this.CopyrightNoticeUrl = copyrightNoticeUrl;
			
			this.Components = new ArrayList();
			this.Levels = new ArrayList();

			//Determinate the number of levels and create an Level object for each one
			int size = Math.Min(width,height);
			while(size > GeoportailTilesLayer.PixelsPerTile) {
				Levels.Add(new GeoportailLayerLevel(this, Levels.Count));
				size /= 2;
			}

			this.GeoTiles = new ArrayList();
		}

		public void ForceRefresh(){
			PrevRow = -1;
			PrevCol = -1;
			PrevLevel = null;
		}

		public void AddComponent(GeoportailLayerComponent component){
			Components.Add(component);
		}

		public ArrayList FindComponentsForScaleAndExtent(double scale,Extent extent){
			ArrayList results = new ArrayList(20);
			scale /= 8;
			while(results.Count == 0){
				foreach(Object objComponent in Components){
					GeoportailLayerComponent component = objComponent as GeoportailLayerComponent;
					if(component.IsValidForScale(scale) && component.Intersect(extent)){
						results.Add(component);
					}
				}
				scale *= 2;
			}
			return results;
		}

		public GeoportailLayerLevel GetLevelByArcDistance(double arcDistance){
			arcDistance /= 8;
			for(int iLevel = 0 ; iLevel < Levels.Count; iLevel++) {
				GeoportailLayerLevel level = Levels[iLevel] as GeoportailLayerLevel;
				
				if(arcDistance < level.WorldPerTile.U) {
					return level;
				}
			}
			return (GeoportailLayerLevel) Levels[Levels.Count - 1];
		}
	}

	public class GeoportailLayerLevel{
		public GeoportailLayer Layer;
		public int Level;
		public UV WorldPerTile;
		public UV WorldPerPixel;

		public GeoportailLayerLevel(GeoportailLayer layer, int level){
			this.Layer = layer;
			this.Level = level;

			//Determinate the worldPerTile for the level
			double wPPU = Layer.CellSize.U * (1 << Level) ;
			double wPPV = Layer.CellSize.V * (1 << Level) ;
			this.WorldPerPixel = new UV(wPPU,wPPV);

			double wPTU = GeoportailTilesLayer.PixelsPerTile * wPPU;
			double wPTV = GeoportailTilesLayer.PixelsPerTile * wPPV;
			this.WorldPerTile = new UV(wPTU, wPTV);
		}

	}

	public class GeoportailLayerComponent{
		public GeoportailLayer Layer;
		public String url;
		public Extent extent;
		public int scaleMin, scaleMax;

		public GeoportailLayerComponent(GeoportailLayer layer,String url,Extent extent,int scaleMin, int scaleMax){
			this.Layer = layer;
			this.url = url;
			this.extent = extent;
			this.scaleMin = scaleMin;
			this.scaleMax = scaleMax;
		}

		public bool Intersect(Extent extent){
			return extent.Intersect(extent);
		}

		public bool IsValidForScale(double scale){
			return scale >= scaleMin && scale <= scaleMax;
		}
	}
	#endregion

	#region TERRITORYMETADATA
	public struct TerritoryMetadata{
		private Hashtable tmd;

		public TerritoryMetadata(MainApplication application){
			tmd = new Hashtable();

			//Lambert2e projection
			Proj lambert2e = new Proj(new string[]{"proj=lcc","towgs84=-168,-60,320,0,0,0,0","a=6378249.1449999996","es=0.0068035113","f=293.4650000000","lat_0=46.80000","lat_1=45.8989194400","lat_2=47.6960138900","lon_0=2.3372291670","x_0=600000","y_0=2200000", "no.defs"});
			
			//Harcode the metadata but it could also be gotten dynamically through calls to a geoportail service
			//May be done in later versions

			//Metr france
			GeoportailTerritory metr = new GeoportailTerritory("metr",1500,1000000);
			tmd[metr.Name] = metr;
			GeoportailLayer layer = new GeoportailLayer("bdortho",lambert2e, new Extent(new UV(45000,2700000),new UV(1200000,1616000)) , new UV(0.5,0.5), 2310000, 2168000,"/c/dmybdobr.jp2,/c/dmybdotl2.jp2");
			metr.AddLayer(layer);

			//Add the components of the layer. Pain in the ass...
			GeoportailLayerComponent component;
			component = new GeoportailLayerComponent(layer,"/c/10.ecw",new Extent(245000, 2600000, 345000, 2500000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/11.ecw",new Extent(345000, 1800000, 445000, 1700000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/12.ecw",new Extent(345000, 1900000, 445000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/13.ecw",new Extent(345000, 2000000, 445000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/14.ecw",new Extent(345000, 2100000, 445000, 2000000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/15.ecw",new Extent(345000, 2200000, 445000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/16.ecw",new Extent(345000, 2300000, 445000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/17.ecw",new Extent(345000, 2400000, 445000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/18.ecw",new Extent(345000, 2500000, 445000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/19.ecw",new Extent(345000, 2600000, 445000, 2500000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/c/1a.ecw",new Extent(445000, 1800000, 545000, 1700000),1500,14000);
			layer.AddComponent(component);
					
			component = new GeoportailLayerComponent(layer,"/c/1b.ecw",new Extent(445000, 1900000, 545000, 1800000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/c/1c.ecw" ,new Extent(445000, 2000000, 545000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/1d.ecw",new Extent(445000, 2100000, 545000, 2000000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/2.ecw",new Extent(45000, 2400000, 145000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/3.ecw",new Extent(45000, 2500000, 145000, 2400000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/c/4.ecw",new Extent(145000, 2200000, 245000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer, "/c/5.ecw",new Extent(145000, 2300000, 245000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/6.ecw",new Extent(145000, 2400000, 245000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/7.ecw",new Extent(145000, 2500000, 245000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/8.ecw",new Extent(245000, 1800000, 345000, 1700000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/9.ecw",new Extent(245000, 1900000, 345000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/a.ecw",new Extent(245000, 2000000, 345000, 1900000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/c/b.ecw",new Extent(245000, 2100000, 345000, 2000000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/c.ecw",new Extent(245000, 2200000, 345000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/d.ecw",new Extent(245000, 2300000, 345000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/e.ecw",new Extent(245000, 2400000, 345000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/f.ecw",new Extent(245000, 2500000, 345000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/1e.ecw",new Extent(445000, 2200000, 545000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/1f.ecw",new Extent(445000, 2300000, 545000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/20.ecw",new Extent(445000, 2400000, 545000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/21.ecw",new Extent(445000, 2500000, 545000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/22.ecw",new Extent(445000, 2600000, 545000, 2500000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/23.ecw",new Extent(445000, 2700000, 545000, 2600000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/d/24.ecw",new Extent(545000, 1800000, 645000, 1700000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/25.ecw",new Extent(545000, 1900000, 645000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/26.ecw",new Extent(545000, 2000000, 645000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/27.ecw",new Extent(545000, 2100000, 645000, 2000000),1500,14000);
			layer.AddComponent(component);
			
			component = new GeoportailLayerComponent(layer,"/d/28.ecw",new Extent(545000, 2200000, 645000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/29.ecw",new Extent(545000, 2300000, 645000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/2a.ecw",new Extent(545000, 2400000, 645000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/2b.ecw",new Extent(545000, 2500000, 645000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/2c.ecw",new Extent(545000, 2600000, 645000, 2500000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/2d.ecw",new Extent(545000, 2700000, 645000, 2600000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/2e.ecw",new Extent(645000, 1800000, 745000, 1700000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/2f.ecw",new Extent(645000, 1900000, 745000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/30.ecw",new Extent(645000, 2000000, 745000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/31.ecw",new Extent(645000, 2100000, 745000, 2000000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/32.ecw",new Extent(645000, 2200000, 745000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/33.ecw",new Extent(645000, 2300000, 745000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/34.ecw",new Extent(645000, 2400000, 745000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/35.ecw",new Extent(645000, 2500000, 745000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/d/36.ecw",new Extent(645000, 2600000, 745000, 2500000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/37.ecw",new Extent(645000, 2700000, 745000, 2600000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/38.ecw",new Extent(745000, 1900000, 845000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/39.ecw",new Extent(745000, 2000000, 845000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/3a.ecw",new Extent(745000, 2100000, 845000, 2000000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/3b.ecw",new Extent(745000, 2200000, 845000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/3c.ecw",new Extent(745000, 2300000, 845000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/3d.ecw",new Extent(745000, 2400000, 845000, 2300000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/e/3e.ecw",new Extent(745000, 2500000, 845000, 2400000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/e/3f.ecw",new Extent(745000, 2600000, 845000, 2500000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/40.ecw",new Extent(845000, 1800000, 945000, 1700000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/41.ecw",new Extent(845000, 1900000, 945000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/42.ecw",new Extent(845000, 2000000, 945000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/43.ecw",new Extent(845000, 2100000, 945000, 2000000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/44.ecw",new Extent(845000, 2200000, 945000, 2100000),1500,14000);
			layer.AddComponent(component);
					
			component = new GeoportailLayerComponent(layer,"/e/45.ecw",new Extent(845000, 2300000, 945000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/46.ecw",new Extent(845000, 2400000, 945000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/47.ecw",new Extent(845000, 2500000, 945000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/48.ecw",new Extent(845000, 2600000, 945000, 2500000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/49.ecw",new Extent(945000, 1900000, 1045000, 1800000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/4a.ecw",new Extent(945000, 2000000, 1045000, 1900000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/4b.ecw",new Extent(945000, 2100000, 1045000, 2000000),1500,14000);
			layer.AddComponent(component);
						
			component = new GeoportailLayerComponent(layer,"/e/4c.ecw",new Extent(945000, 2200000, 1045000, 2100000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/4d.ecw",new Extent(945000, 2300000, 1045000, 2200000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/4e.ecw",new Extent(945000, 2400000, 1045000, 2300000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/4f.ecw",new Extent(945000, 2500000, 1045000, 2400000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/e/56.ecw",new Extent(1110000, 1808000, 1200000, 1616000),1500,14000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/c/0.ecw",new Extent(40000, 2680000, 1200000, 1610000),14000,1000000);
			layer.AddComponent(component);

			//GRK
			layer = new GeoportailLayer("grk",lambert2e, new Extent(new UV(-140.0,2702605.10),new UV(1250000.0,1529970.46875)) , new UV(1.32291666666667,1.32291666666667), 944987, 886400,"/b/dmygrbr.jp2,/b/dmygrtl.jp2");
			metr.AddLayer(layer);

			//Add the components of the layer. Pain in the ass...
			component = new GeoportailLayerComponent(layer,"/b/7.ecw",new Extent(45991, 2678892, 1200995, 1618888),1500, 6250);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/b/2.ecw",new Extent(42487, 2682597, 1222500, 1615382),6250, 12500);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/b/6.ecw",new Extent(24985, 2680015, 1200014, 1604985),12500, 25000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/b/3.ecw",new Extent(-140, 2700000, 1222994, 1599855),50000, 100000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/b/5.ecw",new Extent(22526, 2702572, 1222940, 1595356),100000, 200000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/b/0.ecw",new Extent(24259, 2700865, 1221234, 1597023),200000, 1000000);
			layer.AddComponent(component);

			//Scan
			layer = new GeoportailLayer("scan",lambert2e, new Extent(new UV(0.0000000,2700000.000),new UV(1300100.0,1549900.000)) , new UV(2.50000,2.5000), 520040, 460040,"/f/dmyctometrobr.jp2,/f/dmyctometrotl.jp2");
			metr.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/f/a.ecw",new Extent(40000, 2680000, 1200000, 1610000),1500, 12500);
			layer.AddComponent(component);
																												component = new GeoportailLayerComponent(layer,"/f/1a.ecw",new Extent(40000, 2680000, 1200000, 1610000),1500, 12500);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/15.ecw",new Extent(25000, 2680000, 1200000, 1605000),12500, 25000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/18.ecw",new Extent(30000, 2700000, 1230100, 1589900),25000, 75000);
			layer.AddComponent(component);
			
			//Can scaleMin == scaleMax? This component will probably never be displayed
			component = new GeoportailLayerComponent(layer,"/f/2.ecw",new Extent(0, 2700000, 1250000, 1600000),25000, 25000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/19.ecw",new Extent(0, 2700000, 1300100, 1549900),75000, 150000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/0.ecw",new Extent(0, 2700000, 1215000, 1595000),150000, 1000000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("alti",lambert2e, new Extent(new UV(-1.0347789e+5,2.809548e+6),new UV(1.256872e+6,1.6e+6)) , new UV(25.0000,25.000), 54014, 50200,"/a/dmydtmbr.jp2,/a/dmydtmtl.jp2");
			metr.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/a/0.ecw",new Extent(-1.0347789e+5,2.809548e+6, 1.256872e+6,1.6e+6),1500, 1000000);
			layer.AddComponent(component);

			//Saint-Pierre-et-Miquelon
			Proj spm_utm21n = new Proj(new string[]{"proj=utm","zone=21","units=m", "no.defs"});

			GeoportailTerritory spm = new GeoportailTerritory("spm",1500,300000);
			tmd[spm.Name] = spm;
			
			layer = new GeoportailLayer("scan",spm_utm21n, new Extent(new UV(5.430e+5,5.22500e+6),new UV(5.73e+5,5.1750e+6)) , new UV(2.5,2.5), 12000, 20000,"");
			spm.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/f/d.ecw",new Extent(543000, 5225000, 573000, 5175000),1500, 300000);
			layer.AddComponent(component);

			//Kerguelen
			Proj kerg_utm42s = new Proj(new string[]{"proj=utm","zone=42","south","units=m", "no.defs"});
			GeoportailTerritory kerg = new GeoportailTerritory("kerg",5000, 300000);

			tmd[kerg.Name] = kerg;
			
			layer = new GeoportailLayer("scan",kerg_utm42s, new Extent(new UV(4.2935e+5,4.6467e+6),new UV(6.4571e+5,4.4523e+6)) , new UV(10,10), 21636, 19440,"/f/dmyctokerbr.jp2,/f/dmyctokertl.jp2");
			kerg.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/f/6.ecw",new Extent(451750, 4628550, 632200, 4470700),1500, 50000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/8.ecw",new Extent(429350, 4646700, 645710, 4452300),50000, 300000);
			layer.AddComponent(component);

			//Guadeloupe
			Proj guad_mart_utm20n = new Proj(new string[]{"proj=utm","zone=20","units=m", "no.defs"});
			GeoportailTerritory guad = new GeoportailTerritory("guad",1500, 300000);

			tmd[guad.Name] = guad;
			
			layer = new GeoportailLayer("bdortho",guad_mart_utm20n, new Extent(new UV(4.820e+5,2.0060e+6),new UV(7.150e+5,1.7500e+6)) , new UV(0.5,0.5), 466000, 512000,"");
			guad.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/e/50.ecw",new Extent(482000, 2006000, 715000, 1750000),1500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("scan",guad_mart_utm20n, new Extent(new UV(4.8e+5,2.01e+6),new UV(7.2022e+5,1.74306e+6)) , new UV(2.5,2.5), 96088, 106776,"/f/dmyctoguabr.jp2,/f/dmyctoguatl.jp2");
			guad.AddLayer(layer);
			
			component = new GeoportailLayerComponent(layer,"/f/b.ecw",new Extent(480000, 2010000, 720000, 1750000),1500, 300000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/3.ecw",new Extent(481480, 2005370, 720220, 1743060),12500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("alti",guad_mart_utm20n, new Extent(new UV(4.797342201845560e+5,2.011538517009150e+6),new UV(7.476842201845560e+5,1.733438517009150e+6)) , new UV(25,25),10718, 11124,"");
			guad.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/a/2.ecw",new Extent(1500, 1000000, 1500, 50000),1500, 300000);
			layer.AddComponent(component);

			//Martinique
			GeoportailTerritory mart = new GeoportailTerritory("mart",1500, 300000);

			tmd[mart.Name] = mart;

			layer = new GeoportailLayer("bdortho",guad_mart_utm20n, new Extent(new UV(6.890e+5,1.6470e+6),new UV(7.380e+5,1.590e+6)) , new UV(0.5,0.5), 98000, 114000,"");
			mart.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/e/51.ecw",new Extent(689000, 1647000, 738000, 1590000),1500, 300000);
			layer.AddComponent(component);
			
			layer = new GeoportailLayer("scan",guad_mart_utm20n, new Extent(new UV(6.871e+5,1.6540e+6),new UV(7.53e+5,1.58250e+6)) , new UV(2.5,2.5), 26360, 28600,"/f/dmyctomarbr.jp2,/f/dmyctomartl.jp2");
			mart.AddLayer(layer);
			
			component = new GeoportailLayerComponent(layer,"/f/c.ecw",new Extent(690000, 1650000, 740000, 1590000),1500, 300000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/4.ecw",new Extent(687100, 1654000, 753000, 1582500),12500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("alti",guad_mart_utm20n, new Extent(new UV(6.818436794941037e+5,1.657912564942177e+6),new UV(7.453422509622261e+5,1.583639361437386e+6)) , new UV(25,25),2540, 2971,"");
			mart.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/a/1.ecw",new Extent(690000, 1650000, 740000, 1590000),1500, 300000);
			layer.AddComponent(component);

			//Guyane
			Proj guya_utm22n = new Proj(new string[]{"proj=utm","zone=22","units=m", "no.defs"});

			GeoportailTerritory guya = new GeoportailTerritory("guya",1500, 500000);

			tmd[guya.Name] = guya;

			layer = new GeoportailLayer("bdortho",guya_utm22n, new Extent(new UV(1.51e+5,6.38e+5),new UV(3.65e+5,4.98e+5)) , new UV(0.5,0.5), 427999, 280000,"");
			guya.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/e/52.ecw",new Extent(151000, 638000, 365000, 498000),1500, 500000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("scan",guya_utm22n, new Extent(new UV(8.325192613901058e+4,6.783609006634455e+005),new UV(4.832519261390106e+005,1.783609000000000e+005)) , new UV(2.5,2.5), 160000, 200000,"/f/dmyctoguybr2.jp2,/f/dmyctoguytl2.jp2");
			guya.AddLayer(layer);
			
			component = new GeoportailLayerComponent(layer,"/f/e.ecw",new Extent(150000, 640000, 420000, 460000),1500, 500000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/1.ecw",new Extent(100000, 643000, 450000, 343000),12500, 50000);
			layer.AddComponent(component);
																											component = new GeoportailLayerComponent(layer,"/f/12.ecw",new Extent(83250, 678475, 483250, 178475),50000, 500000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("alti",guya_utm22n, new Extent(new UV(1.46e+5,6.40e+5),new UV(4.46e+5,4.50e+5)) , new UV(25,25),12000, 8000,"");
			guya.AddLayer(layer);

			component = new GeoportailLayerComponent(layer, "/a/3.ecw",new Extent(150000, 640000, 420000, 460000),1500, 500000);
			layer.AddComponent(component);

			//Wallis & Futuna
			Proj wafu_utm1s = new Proj(new string[]{"proj=utm","zone=1","south","units=m", "no.defs"});

			GeoportailTerritory wafu = new GeoportailTerritory("wafu",1500, 300000);

			tmd[wafu.Name] = wafu;

			layer = new GeoportailLayer("bdortho",wafu_utm1s, new Extent(new UV(3.7200e+5,8.5440e+6),new UV(8.41100e+6,8.4110e+6)) , new UV(0.5,0.5), 448000, 266000,"");
			wafu.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/e/55.ecw",new Extent(372000, 8544000, 596000, 8411000),1500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("scan",wafu_utm1s, new Extent(new UV(3.7119125e+5,8.543685e+6),new UV(5.9679125e+5,8.410245e+6)) , new UV(2.5,2.5), 90240, 53376,"");
			wafu.AddLayer(layer);
		
			component = new GeoportailLayerComponent(layer,"/f/f.ecw",new Extent(371191, 8426565, 392951, 8410245),1500, 300000);
			layer.AddComponent(component);

			//Réunion
			Proj reun_utm40s = new Proj(new string[]{"proj=utm","zone=40","south","units=m", "no.defs"});

			GeoportailTerritory reun = new GeoportailTerritory("reun",1500, 300000);

			tmd[reun.Name] = reun;

			layer = new GeoportailLayer("bdortho",reun_utm40s, new Extent(new UV(3.13e+5,7.693e+6),new UV(3.810e+5,7.6330+6)) , new UV(0.5,0.5), 136000, 120000,"");
			reun.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/e/53.ecw",new Extent(13000, 7693000, 381000, 7633000),1500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("scan",reun_utm40s, new Extent(new UV(3.08377e+5,7.6979890e+6),new UV(3.853770e+5,7.626489e+6)) , new UV(2.5,2.5), 30800, 28600,"/f/dmyctoreubr.jp2,/f/dmyctoreutl.jp2");
			reun.AddLayer(layer);
		
			component = new GeoportailLayerComponent(layer,"/f/11.ecw",new Extent(312000, 7692000, 382000, 7632000),1500, 300000);
			layer.AddComponent(component);
																											component = new GeoportailLayerComponent(layer,"/f/5.ecw",new Extent(308377, 7697989, 385377, 7626489),12500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("alti",reun_utm40s, new Extent(new UV(3.05e+5,7.70e+6),new UV(3.95e+5,7.625e+6)) , new UV(25,25), 3600, 3000,"");
			reun.AddLayer(layer);
		
			component = new GeoportailLayerComponent(layer,"/a/5.ecw",new Extent(312000, 7692000, 382000, 7632000),1500, 300000);
			layer.AddComponent(component);

			//Mayotte
			Proj mayo_utm38s = new Proj(new string[]{"proj=utm","zone=38","south","units=m", "no.defs"});

			GeoportailTerritory mayo = new GeoportailTerritory("mayo",1500, 300000);

			tmd[mayo.Name] = mayo;

		
			layer = new GeoportailLayer("bdortho",mayo_utm38s, new Extent(new UV(4.93e+5,8.61200e+6),new UV(5.340e+5,8.5520e+6)) , new UV(0.5,0.5), 82000, 120000,"");
			mayo.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/e/54.ecw",new Extent(493000, 8612000, 534000, 8552000),1500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("scan",mayo_utm38s, new Extent(new UV(5.00e+5,8.610e+6),new UV(5.400e+5,8.560e+6)) , new UV(2.5,2.5), 16000, 20000,"");
			mayo.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/f/10.ecw",new Extent(500000, 8610000, 540000, 8560000),1500, 300000);
			layer.AddComponent(component);

			layer = new GeoportailLayer("alti",mayo_utm38s, new Extent(new UV(5.00e+5,8.605e+6),new UV(5.40e+5,8.56e+6)) , new UV(25,25),1600, 1800,"");
			mayo.AddLayer(layer);
	
			component = new GeoportailLayerComponent(layer,"/a/4.ecw",new Extent(500000, 8610000, 540000, 8560000),1500, 300000);
			layer.AddComponent(component);

			//Nouvelle-Calédonie
			Proj ncal_utm58s = new Proj(new string[]{"proj=utm","zone=58","south","units=m", "no.defs"});

			GeoportailTerritory ncal = new GeoportailTerritory("ncal",5000, 500000);

			tmd[ncal.Name] = ncal;
		
			layer = new GeoportailLayer("scan",ncal_utm58s, new Extent(new UV(3.40e+5,7.86e+6),new UV(8.40e+5,7.46e+6)) , new UV(5,5), 100000, 80000,"/f/dmyctoncabr.jp2,/f/dmyctoncatl.jp2");
			ncal.AddLayer(layer);

			component = new GeoportailLayerComponent(layer,"/f/17.ecw",new Extent(340000, 7850000, 840000, 7470000),5000, 500000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/9.ecw",new Extent(40000, 7860000, 840000, 7460000),25000, 500000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/14.ecw",new Extent(340000, 7860000, 840000, 7460000),150000, 500000);
			layer.AddComponent(component);

			//Crozet
			Proj croz_utm39s = new Proj(new string[]{"proj=utm","zone=39","south","units=m", "no.defs"});

			GeoportailTerritory croz = new GeoportailTerritory("croz",5000, 300000);

			tmd[croz.Name] = croz;
		
			layer = new GeoportailLayer("scan",croz_utm39s, new Extent(new UV(4.152e+5,4.94255e+6),new UV(6.21e+5,4.81405e+6)) , new UV(5,5), 41160, 25700,"/f/dmyctocrobr.jp2,/f/dmyctocrotl.jp2");
			croz.AddLayer(layer);


			component = new GeoportailLayerComponent(layer,"/f/16.ecw",new Extent(547850, 4870000, 596000, 4851000),5000, 300000);
			layer.AddComponent(component);

			component = new GeoportailLayerComponent(layer,"/f/7.ecw",new Extent(421000, 4936000, 615000, 4835000),25000, 300000);
			layer.AddComponent(component);

		}
	
	
		public GeoportailTerritory getTerritory(String territory){
			return (GeoportailTerritory) tmd[territory];
		}
	}
	#endregion

	#region GEOPORTAILTILESLAYER
	public class GeoportailTilesLayer : RenderableObject {
		private GeoportailForm geoForm;
		private TerritoryMetadata metadata;
		MainApplication parentApplication;

		public static int PixelsPerTile = 384;
		public double EarthRadius;
		public double EarthCircum;
		public double EarthHalfCirc;

		private float prevVe = -1;

		private Hashtable dispTerritories;

		public GeoportailTilesLayer(string name, TerritoryMetadata metadata, MainApplication parentApplication, GeoportailForm geoForm) : base (name) {
			this.name = name;
			this.metadata = metadata;
			this.parentApplication = parentApplication;
			this.geoForm = geoForm;
			dispTerritories = new Hashtable();

			EarthRadius = parentApplication.WorldWindow.CurrentWorld.EquatorialRadius;
			EarthHalfCirc = EarthRadius  * Math.PI;
			EarthCircum = EarthHalfCirc * 2.0;
		}

		public void SetLayerForTerritory(String territory, String layerName){
			GeoportailLayer layer = metadata.getTerritory(territory).GetLayer(layerName);
			if(layer != null){
				dispTerritories[territory] = layer;
			}
		}

		public void removeTerritory(String territory){
			dispTerritories.Remove(territory);
		}

		public override void Initialize(DrawArgs drawArgs) {
			try {
				if(this.isInitialized ) {
					return;
				}

				GeoTile.Init(parentApplication.WorldWindow.CurrentWorld.TerrainAccessor, parentApplication.WorldWindow.CurrentWorld.EquatorialRadius, geoForm);

				prevVe = World.Settings.VerticalExaggeration;

				this.isInitialized = true;
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
				throw;
			}
		}

		public void RemoveAllTiles(GeoportailLayer layer) {
			ArrayList geoTiles = layer.GeoTiles;
			lock(geoTiles.SyncRoot) {
				for(int i=0; i<geoTiles.Count; i++) {
					GeoTile geoTile = (GeoTile) geoTiles[i];
					geoTile.Dispose();
					geoTiles.RemoveAt(i);
				}
				geoTiles.Clear();
			}
		}

		public void RemoveAllTiles(){
			foreach(string key in dispTerritories.Keys){
				GeoportailLayer layer = (GeoportailLayer) dispTerritories[key];
				RemoveAllTiles(layer);
			}
		}

		/// <summary>
		/// Update layer (called from worker thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs) {
			try {
				if(!this.isOn) {
					return;
				}

				if(!this.isInitialized) {
					this.Initialize(drawArgs);
					return;
				}

				//get lat, lon from WW
				double lat = drawArgs.WorldCamera.Latitude.Degrees;
				double lon = drawArgs.WorldCamera.Longitude.Degrees;

				foreach(string key in dispTerritories.Keys){
					GeoportailTerritory territory = metadata.getTerritory(key);
					GeoportailLayer layer = dispTerritories[key] as GeoportailLayer;
					ArrayList geoTiles = layer.GeoTiles;
			
					//determine zoom level
					double alt = drawArgs.WorldCamera.Altitude;

					Angle tvr = drawArgs.WorldCamera.TrueViewRange; //off of altitude
					
					double arcDistance = tvr.Radians * EarthRadius;
					double scale = Scale(arcDistance);

					//dont start VE tiles until a certain zoom level
					if(scale > territory.ScaleMax * 4) { //Arbitrary factor 4
						this.RemoveAllTiles(layer);
						continue;
					}

					GeoportailLayerLevel level = layer.GetLevelByArcDistance(arcDistance);
					
					//this correctly keeps me on the current tile that is being viewed
					UV uvCurrent = new UV(DegToRad(lon), DegToRad(lat));
					uvCurrent = layer.Projection.Forward(uvCurrent); //in our projection 
                    
					//Check if there exists layer components for the current scale and extent of the 
					//Check if inside the extent of the layer
					if(! layer.LayerExtent.Contains(uvCurrent)){
						continue;
					}

					//Get the tile for the current position:
					UV relativeToTL = new UV(uvCurrent.U - layer.LayerExtent.TopLeftUV.U,
											 layer.LayerExtent.TopLeftUV.V - uvCurrent.V); //Tile Y grows southbound
					
					int col = (int) Math.Floor(relativeToTL.U / level.WorldPerTile.U);
					int row = (int) Math.Floor(relativeToTL.V / level.WorldPerTile.V);

					//if within previous bounds and same zoom level, then exit
					if(row == layer.PrevRow && col == layer.PrevCol && level == layer.PrevLevel) {
						continue;
					}

					//update mesh if VertEx changes
					if(prevVe != World.Settings.VerticalExaggeration) {
						lock(geoTiles.SyncRoot) {
							foreach(GeoTile geoTile in geoTiles) {
								if(geoTile.VertEx != World.Settings.VerticalExaggeration) {
									geoTile.CreateMesh(this.Opacity, World.Settings.VerticalExaggeration);
								}
							}
						}
					}
					prevVe = World.Settings.VerticalExaggeration;

					lock(geoTiles.SyncRoot) {
						foreach(GeoTile geoTile in geoTiles) {
							geoTile.IsNeeded = false;
						}
					}

					//add current tiles first
					AddGeoTile(drawArgs, layer, row, col, level);
					//then add other tiles outwards in surrounding circles
					AddNeighborTiles(drawArgs, layer, row, col, level, 1);
					AddNeighborTiles(drawArgs, layer, row, col, level, 2);
					//AddNeighborTiles(drawArgs, layer, row, col, level, 3);
			

					lock(geoTiles.SyncRoot) {
						GeoTile geoTile;
						for(int i=0; i<geoTiles.Count; i++) {
							geoTile = (GeoTile) geoTiles[i];
							if(!geoTile.IsNeeded) {
								geoTile.Dispose();
								geoTiles.RemoveAt(i);
							}
						}
					}
					
					layer.PrevRow = row;
					layer.PrevCol = col;
					layer.PrevLevel = level;
				}
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
			}
		}

		private void AddNeighborTiles(DrawArgs drawArgs, GeoportailLayer layer, int row, int col, GeoportailLayerLevel zoomLevel, int range) {
			int minRow = row - range;
			int maxRow = row + range;
			int minCol = col - range;
			int maxCol = col + range;
			for(int i=minRow; i<=maxRow; i++) {
				for(int j=minCol; j<=maxCol; j++) {
					//only outer edges, inner tiles should already be added
					if(i == minRow || i == maxRow || j == minCol || j == maxCol) {
						AddGeoTile(drawArgs, layer, i, j, zoomLevel);
					}
				}
			}
		}

		private void AddGeoTile(DrawArgs drawArgs, GeoportailLayer layer, int row, int col, GeoportailLayerLevel zoomLevel) {
			ArrayList geoTiles = layer.GeoTiles;
			bool tileFound = false;
			lock(geoTiles.SyncRoot) {
				foreach(GeoTile geoTile in geoTiles) {
					if(geoTile.IsNeeded == true) {
						continue; //we don't want to do the next test
					}
					if(geoTile.IsEqual(layer,row, col, zoomLevel) == true) {
						geoTile.IsNeeded = true;
						tileFound = true;
						break;
					}
				}
			}
			if(tileFound == false) {
				//exit if zoom level has changed
				double arcDistance = drawArgs.WorldCamera.TrueViewRange.Radians * EarthRadius;
				double scale = Scale(arcDistance);

				GeoportailLayerLevel curZoomLevel = layer.GetLevelByArcDistance(arcDistance);

				if(curZoomLevel != zoomLevel) {
					return;
				}
				
				//Calculate the extent of the current tile so we can know if it intersects any components of the layer
				double xTL = col * zoomLevel.WorldPerTile.U + layer.LayerExtent.TopLeftUV.U;
				double yTL = layer.LayerExtent.TopLeftUV.V - (row * zoomLevel.WorldPerTile.V);
				double xBR = xTL + zoomLevel.WorldPerTile.U;
				double yBR = yTL - zoomLevel.WorldPerTile.V;
				Extent extent = new Extent(xTL,yTL,xBR,yBR);
				
				ArrayList components = layer.FindComponentsForScaleAndExtent(scale,extent);

				if(components.Count == 0)
					return;

				GeoTile newGeoTile = new GeoTile(layer, components, row, col, zoomLevel);

				//thread to download new tile(s) or just load from cache
				newGeoTile.GetTexture(drawArgs);

				newGeoTile.UL = new UV(xTL,yTL);
				newGeoTile.UR = new UV(xBR,yTL);
				newGeoTile.LL = new UV(xTL,yBR);
				newGeoTile.LR = new UV(xBR,yBR);

				//create mesh
				byte opacity = this.Opacity; //from RenderableObject
				float verticalExaggeration = World.Settings.VerticalExaggeration;
				newGeoTile.CreateMesh(opacity, verticalExaggeration);
				newGeoTile.CreateDownloadRectangle(drawArgs, World.Settings.DownloadProgressColor.ToArgb());
				
				newGeoTile.IsNeeded = true;
				lock(geoTiles.SyncRoot) {
					geoTiles.Add(newGeoTile);
				}
			}
		}
		
		private double Scale(double arcDistance){
			//I have no idea how this formula is obtained...
			//arcDistance is the size in meters of a WW tile at hte current level of zoom. Gives an approximate scale
			return 9600 * arcDistance / (GeoportailTilesLayer.PixelsPerTile * 2.54);
		}

		private static double DegToRad(double d) {
			return d * Math.PI / 180.0;
		}

		private static double RadToDeg(double d) {
			return d * 180/Math.PI;
		}

		/// <summary>
		/// Draws the layer
		/// </summary>
		public override void Render(DrawArgs drawArgs) {
			try {
				if(this.isOn == false) {
					return;
				}

				if(this.isInitialized == false) {
					return;
				}

				if(drawArgs.device == null)
					return;

				foreach(string key in dispTerritories.Keys){
					GeoportailLayer layer = dispTerritories[key] as GeoportailLayer;
					ArrayList geoTiles = layer.GeoTiles;
					if(geoTiles != null && geoTiles.Count > 0) {
						//render mesh and tile(s)
						bool disableZBuffer = false;
						GeoTile.Render(drawArgs, disableZBuffer, geoTiles);
					}
				}
			}
			catch(Exception ex) {
				Utility.Log.Write(ex);
			}
		}


		public override void Dispose() {
			foreach(string key in dispTerritories.Keys){
				GeoportailLayer layer = dispTerritories[key] as GeoportailLayer;
				RemoveAllTiles(layer);	
			}
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs) {
			return false;
		}
	}
	#endregion
	
	#region GEOTILE
	public class GeoTile : IDisposable {
		//these are the coordinate extents for the tile
		public UV UL, UR, LL, LR;
		public UV geoUL, geoUR, geoLL, geoLR;

		//store the Vertical Exaggeration for when the mesh was created
		//so when the VerticalExaggeration setting changes, it know which meshes to recreate
		private float vertEx;
		public float VertEx {
			get{return vertEx;}
		}

		private static double layerRadius;
		private static TerrainAccessor terrainAccessor;
		private static System.Drawing.Font font;
		private static Brush brush;
		private static GeoportailForm geoForm;

		public static void Init(TerrainAccessor _terrainAccessor, double _layerRadius, GeoportailForm _geoForm) {
			terrainAccessor = _terrainAccessor;
			layerRadius = _layerRadius;
			geoForm = _geoForm;
			font = new System.Drawing.Font("Verdana", 15, FontStyle.Bold);
			brush = new SolidBrush(Color.Green);
		}

		//flag for if the tile should be disposed
		private bool isNeeded = true;
		public bool IsNeeded {
			get{return isNeeded;}
			set{isNeeded = value;}
		}

		public bool IsEqual(GeoportailLayer layer, int row, int col, GeoportailLayerLevel level) {
			bool retVal = false;
			if(this.layer == layer && this.row == row && this.col == col && this.level == level) {
				retVal = true;
			}
			return retVal;
		}

		private bool debug = false;
		private int row;
		private int col;
		private GeoportailLayerLevel level;
		private GeoportailLayer layer;
		private ArrayList layerComponents;

		public GeoTile(GeoportailLayer layer, ArrayList layerComponents, int row, int col, GeoportailLayerLevel level){
			this.layer = layer;
			this.layerComponents = layerComponents;
			this.row = row;
			this.col = col;
			this.level = level;
		}

		private Texture texture = null;
		public Texture Texture {
			get{return texture;}
			set{texture = value;}
		}

		private ArrayList alMetaData = new ArrayList();
		private WebDownloadWithReferer download;
		public float ProgressPercent;
		private string textureName;
		private DrawArgs drawArgs;

		public void GetTexture(DrawArgs drawArgs) {
			this.drawArgs = drawArgs;

			/*if(debug) {
				//generate a DEBUG tile with metadata
				MemoryStream ms;
				//debug
				Bitmap b = new Bitmap(GeoportailTilesLayer.PixelsPerTile, 
									  GeoportailTilesLayer.PixelsPerTile);
				System.Drawing.Imaging.ImageFormat imageFormat;
								
				alMetaData.Add("ww rowXcol : " + row.ToString() + "x" + col.ToString());			
				alMetaData.Add("geoLevel : " + level.ToString());
				
				imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
				b = DecorateBitmap(b, font, brush, alMetaData);
				
				ms = new MemoryStream();
				b.Save(ms, imageFormat);
				ms.Position = 0;
				this.texture = TextureLoader.FromStream(drawArgs.device, ms);
				ms.Close();
				ms = null;
				b.Dispose();
				b = null;
			}
			else {*/
				//load a tile from file OR download it if not cached
				//Change that to have a territory/Layer/level tree instead
				string territoryDir = CreateTerritoryDir(geoForm.WorldWindow.Cache.CacheDirectory,layer.Territory.Name);
				string layerDir = CreateLayerDir(territoryDir, layer.Name);
				string levelDir = CreateLevelDir(layerDir,level.Level);
				
				textureName = GetTextureName(levelDir, row, col);
				
				if(File.Exists(textureName) == true) {
					this.texture = TextureLoader.FromFile(drawArgs.device, textureName);
				}
				else { 
					string textureUrl = "http://cimg.geoportail.fr/ImageX/ImageX.dll?image?cache=true&transparent=true&type=jpg&l=" + level.Level.ToString() + "&tx=" + col.ToString() + "&ty=" + row.ToString() + "&ts=384&fill=ffffff&quality=60&layers=" + formatComponents() + layer.CopyrightNoticeUrl;

					download = new WebDownloadWithReferer(textureUrl);
					download.DownloadType = DownloadType.Unspecified;
					download.SavedFilePath = textureName + ".tmp"; //?
					download.CompleteCallback += new DownloadCompleteHandler(DownloadComplete);
					download.BackgroundDownloadFile();
				}
			//}			
		}

		private string formatComponents(){
			string url = "";
			foreach(GeoportailLayerComponent component in layerComponents){
				url += component.url + ",";
			}
			return url;
		}

		private void DownloadComplete(WebDownloadWithReferer downloadInfo) {
			try {
				downloadInfo.Verify();

				// Rename temp file to real name
				File.Delete(textureName);
				File.Move(downloadInfo.SavedFilePath, textureName);

				this.texture = TextureLoader.FromFile(drawArgs.device, textureName);
			}
			catch(System.Net.WebException caught) {
				System.Net.HttpWebResponse response = caught.Response as System.Net.HttpWebResponse;
				if(response!=null && response.StatusCode==System.Net.HttpStatusCode.NotFound) {
					using(File.Create(textureName + ".txt")) {
					 }
					return;
				}
				
			}
			catch {
				using(File.Create(textureName + ".txt")) {
				 }
				if(File.Exists(downloadInfo.SavedFilePath))
					File.Delete(downloadInfo.SavedFilePath);
			}
			finally {
				download.IsComplete = true;
			}
		}

		public void AddMetaData(string metadata) {
			alMetaData.Add(metadata);
		}

		public static string CreateTerritoryDir(string cacheDirectoryRoot, string territoryName) {
			string cacheDirectory = String.Format("{0}\\Geoportail", cacheDirectoryRoot);
			if(Directory.Exists(cacheDirectory) == false) {
				Directory.CreateDirectory(cacheDirectory);
			}
			string territoryDir = cacheDirectory + @"\" + territoryName;
			if(Directory.Exists(territoryDir) == false) {
				Directory.CreateDirectory(territoryDir);
			}
			return territoryDir;
		}

		public static string CreateLayerDir(string territoryDir, string layerName) {
			string layerDir = territoryDir + @"\" + layerName;
			if(Directory.Exists(layerDir) == false) {
				Directory.CreateDirectory(layerDir);
			}
			return layerDir;
		}

		public static string CreateLevelDir(string layerDir, int level) {
			string levelDir = layerDir + @"\" + level.ToString("00");
			if(Directory.Exists(levelDir) == false) {
				Directory.CreateDirectory(levelDir);
			}
			return levelDir;
		}

		public static string GetTextureName(string cacheDirectory,string territoryName, string layerName, int level, int row, int col){
			string territoryDir = CreateTerritoryDir(cacheDirectory,territoryName);
			string layerDir = CreateLayerDir(territoryDir,layerName);
			string levelDir = CreateLevelDir(layerDir,level);

			return GetTextureName(levelDir,row,col);
		}

		public static string GetTextureName(string levelDir, int row, int col) {
			return levelDir + @"\" + row.ToString("00000") + "_" + col.ToString("00000") + ".jpg";
		}

		protected CustomVertex.PositionColoredTextured[] vertices;
		public CustomVertex.PositionColoredTextured[] Vertices {
			get{return vertices;}
		}
		protected short[] indices;
		public short[] Indices {
			get{return indices;}
		}
		protected int meshPointCount = 64;

		//NOTE this is a mix from Mashi's Reproject and WW for terrain
		public void CreateMesh(byte opacity, float verticalExaggeration) {
			this.vertEx = verticalExaggeration;

			int opacityColor = System.Drawing.Color.FromArgb(opacity,0,0,0).ToArgb();

			meshPointCount = 32; //64; //96 // How many vertices for each direction in mesh (total: n^2)
			vertices = new CustomVertex.PositionColoredTextured[meshPointCount * meshPointCount];

			int upperBound = meshPointCount - 1;
			float scaleFactor = (float)1/upperBound;
			
			double uRange = Math.Sqrt(Math.Pow(UL.U-UR.U,2)+Math.Pow(UL.V-UR.V,2)) ;
			double vRange = Math.Sqrt(Math.Pow(UL.U-LL.U,2)+Math.Pow(UL.V-LL.V,2));
			double uStep = uRange/upperBound;
			double vStep = vRange/upperBound;
			UV curUnprojected = new UV(UL.U,UL.V);

			// figure out latrange (for terrain detail)
			geoUL = layer.Projection.Inverse(UL);
			geoLR = layer.Projection.Inverse(LR);
			geoUR = layer.Projection.Inverse(UR);
			geoLL = layer.Projection.Inverse(LL);
			double latRange = (geoUL.V - geoLL.V) * 180/Math.PI;
		

			UV geo;
			Vector3 pos;
			double height = 0;
			for(int i = 0; i < meshPointCount; i++) {
				for(int j = 0; j < meshPointCount; j++) {	
					geo = layer.Projection.Inverse(curUnprojected);
							
					// Radians -> Degrees
					geo.U *= 180/Math.PI;
					geo.V *= 180/Math.PI;

					//original
					height = verticalExaggeration * terrainAccessor.GetElevationAt(geo.V, geo.U,((double)meshPointCount)/latRange);

					pos = MathEngine.SphericalToCartesian( 
						geo.V,
						geo.U, 
						layerRadius + height);

					vertices[i*meshPointCount + j].X = pos.X;
					vertices[i*meshPointCount + j].Y = pos.Y;
					vertices[i*meshPointCount + j].Z = pos.Z;
										
					vertices[i*meshPointCount + j].Tu = j*scaleFactor;
					vertices[i*meshPointCount + j].Tv = i*scaleFactor;
					vertices[i*meshPointCount + j].Color = opacityColor;
					curUnprojected.U += uStep;
				}
				curUnprojected.U = UL.U;
				curUnprojected.V -= vStep;
			}
			//}

			indices = new short[2 * upperBound * upperBound * 3];
			for(int i = 0; i < upperBound; i++) {
				for(int j = 0; j < upperBound; j++) {
					indices[(2*3*i*upperBound) + 6*j] = (short)(i*meshPointCount + j);
					indices[(2*3*i*upperBound) + 6*j + 1] = (short)((i+1)*meshPointCount + j);
					indices[(2*3*i*upperBound) + 6*j + 2] = (short)(i*meshPointCount + j+1);
	
					indices[(2*3*i*upperBound) + 6*j + 3] = (short)(i*meshPointCount + j+1);
					indices[(2*3*i*upperBound) + 6*j + 4] = (short)((i+1)*meshPointCount + j);
					indices[(2*3*i*upperBound) + 6*j + 5] = (short)((i+1)*meshPointCount + j+1);
				}
			}
		}

		public void Dispose() {
			if(texture != null) {
				texture.Dispose();
				texture = null;
			}
			if(download != null) {
				download.Dispose();
				download = null;
			}
			if(vertices != null) {
				vertices = null;
			}
			if(indices != null) {
				indices = null;
			}
			if(downloadRectangle != null) {
				downloadRectangle = null;
			}
			GC.SuppressFinalize(this);
		}

		CustomVertex.PositionColored[] downloadRectangle = new CustomVertex.PositionColored[5];

		public static void Render(DrawArgs drawArgs, bool disableZbuffer, ArrayList alGeoTiles) {
			try {
				if(alGeoTiles.Count <= 0)
					return;

				lock(alGeoTiles.SyncRoot) {
					//setup device to render textures
					if(disableZbuffer) {
						if(drawArgs.device.RenderState.ZBufferEnable)
							drawArgs.device.RenderState.ZBufferEnable = false;
					}
					else {
						if(!drawArgs.device.RenderState.ZBufferEnable)
							drawArgs.device.RenderState.ZBufferEnable = true;
					}
					
					drawArgs.device.VertexFormat = CustomVertex.PositionColoredTextured.Format;
					drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
					drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Add;
					drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;

					//save index to tiles not downloaded yet
					int notDownloadedIter = 0;
					int [] notDownloaded = new int[alGeoTiles.Count];

					//render tiles that are downloaded
					

					GeoTile geoTile;
					for(int i=0; i<alGeoTiles.Count; i++) {
						geoTile = (GeoTile) alGeoTiles[i];
						if(geoTile.Texture == null) { //not downloaded yet
							notDownloaded[notDownloadedIter] = i;
							notDownloadedIter++;
							continue;
						}
						else {
							//NOTE to stop ripping?
							drawArgs.device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
							drawArgs.device.SetTexture(0, geoTile.Texture);
							
							drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, 
							geoTile.Vertices.Length, geoTile.Indices.Length / 3, geoTile.Indices, true, geoTile.Vertices);
						}
					}

					//now render the downloading tiles
					drawArgs.device.RenderState.ZBufferEnable = false;
					drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
					drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;

					int tileIndex;
					for(int i=0; i<notDownloadedIter; i++) {
						tileIndex = notDownloaded[i];
						geoTile = (GeoTile) alGeoTiles[tileIndex];
						//TODO render progress bar indicator too?
						geoTile.RenderDownloadRectangle(drawArgs);
					}

					drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
					drawArgs.device.VertexFormat = CustomVertex.PositionTextured.Format;
					drawArgs.device.RenderState.ZBufferEnable = true;
				}
			}
			catch(Exception ex) {
				string sex = ex.ToString();
				Utility.Log.Write(ex);
			}
			finally {
				if(disableZbuffer)
					drawArgs.device.RenderState.ZBufferEnable = true;
			}
		}

		public void CreateDownloadRectangle(DrawArgs drawArgs, int color) {
			// Render terrain download rectangle
			Vector3 northWestV = MathEngine.SphericalToCartesian((float)geoUL.V *180/Math.PI, (float)geoUL.U *180/Math.PI, layerRadius);
			Vector3 southWestV = MathEngine.SphericalToCartesian((float)geoLL.V *180/Math.PI, (float)geoLL.U *180/Math.PI, layerRadius);
			Vector3 northEastV = MathEngine.SphericalToCartesian((float)geoUR.V*180/Math.PI, (float)geoUR.U*180/Math.PI, layerRadius);
			Vector3 southEastV = MathEngine.SphericalToCartesian((float)geoLR.V*180/Math.PI, (float)geoLR.U*180/Math.PI, layerRadius);
			
			downloadRectangle[0].X = northWestV.X;
			downloadRectangle[0].Y = northWestV.Y;
			downloadRectangle[0].Z = northWestV.Z;
			downloadRectangle[0].Color = color;

			downloadRectangle[1].X = southWestV.X;
			downloadRectangle[1].Y = southWestV.Y;
			downloadRectangle[1].Z = southWestV.Z;
			downloadRectangle[1].Color = color;

			downloadRectangle[2].X = southEastV.X;
			downloadRectangle[2].Y = southEastV.Y;
			downloadRectangle[2].Z = southEastV.Z;
			downloadRectangle[2].Color = color;

			downloadRectangle[3].X = northEastV.X;
			downloadRectangle[3].Y = northEastV.Y;
			downloadRectangle[3].Z = northEastV.Z;
			downloadRectangle[3].Color = color;

			downloadRectangle[4].X = downloadRectangle[0].X;
			downloadRectangle[4].Y = downloadRectangle[0].Y;
			downloadRectangle[4].Z = downloadRectangle[0].Z;
			downloadRectangle[4].Color = color;
		}

		public void RenderDownloadRectangle(DrawArgs drawArgs) {
			drawArgs.device.DrawUserPrimitives(PrimitiveType.LineStrip, 4, downloadRectangle);
		}
	}
	#endregion

	#region PROJ.4
	[StructLayout(LayoutKind.Sequential)]
	public struct UV { 
		public double U;
		public double V; 

		public UV(double u, double v) {
			this.U = u;
			this.V = v;
		}
	}

	/// <summary>
	/// C# wrapper for proj.4 projection filter. By Mashu.com.
	/// http://proj.maptools.org/
	/// </summary>
	public class Proj : IDisposable {
		IntPtr projPJ;

		[DllImport("proj.dll")]
		static extern IntPtr pj_init(int argc, string[] args);

		[DllImport("proj.dll")]
		static extern string pj_free(IntPtr projPJ);

		//Lat/Lon to XY
		[DllImport("proj.dll")]
		static extern UV pj_fwd(UV uv, IntPtr projPJ);
	
		///XY to Lat/lon
		[DllImport("proj.dll")]
		static extern UV pj_inv(UV uv, IntPtr projPJ);
		
		public Proj(string[] initParameters) {
			projPJ = pj_init( initParameters.Length, initParameters);
			if(projPJ==IntPtr.Zero)
				throw new ApplicationException("Projection initialization failed."); 
		}

		/// Lat/Lon in WGS84 sphere to XY
		public UV Forward(UV uv) {
			return pj_fwd(uv, projPJ); 
		}

		//XY to Lat/Lon in WGS84 sphere
		public UV Inverse(UV uv) {
			return pj_inv(uv, projPJ);
		}

		public void Dispose() {
			if(projPJ!=IntPtr.Zero) {
				pj_free(projPJ);
				projPJ = IntPtr.Zero;
			}
		}
	}
	#endregion

	#region DOWNLOAD
	public delegate void DownloadCompleteHandler(WebDownloadWithReferer wd);

	public class WebDownloadWithReferer : IDisposable{
		#region Static proxy properties

		static public bool Log404Errors = false;
		static public bool useWindowsDefaultProxy = true;
		static public string proxyUrl = "";
		static public bool useDynamicProxy;
		static public string proxyUserName = "";
		static public string proxyPassword = "";
		static public string refererURL = "http://visu.geoportail.fr/";

		#endregion
		
		public static string UserAgent = String.Format(
			CultureInfo.InvariantCulture,
			"World Wind v{0} ({1}, {2})",
			System.Windows.Forms.Application.ProductVersion,
			Environment.OSVersion.ToString(),
			CultureInfo.CurrentCulture.Name);

		public string Url;

		/// <summary>
		/// Memory downloads fills this stream
		/// </summary>
		public Stream ContentStream; 

		public string SavedFilePath;
		public bool IsComplete;

		/// <summary>
		/// Called when data is being received.  
		/// Note that totalBytes will be zero if the server does not respond with content-length.
		/// </summary>
		public DownloadProgressHandler ProgressCallback;

		/// <summary>
		/// Called to update debug window.
		/// </summary>
		public static DownloadCompleteHandler DebugCallback;

		/// <summary>
		/// Called when a download has ended with success or failure
		/// </summary>
		public static DownloadCompleteHandler DownloadEnded;

		/// <summary>
		/// Called when download is completed.  Call Verify from event handler to throw any exception.
		/// </summary>
		public DownloadCompleteHandler CompleteCallback;
		
		public DownloadType DownloadType = DownloadType.Unspecified;
		public string ContentType;
		public int BytesProcessed;
		public int ContentLength;

		/// <summary>
		/// The download start time (or MinValue if not yet started)
		/// </summary>
		public DateTime DownloadStartTime = DateTime.MinValue;

		internal HttpWebRequest request;
		internal HttpWebResponse response;

		protected Exception downloadException;

		protected bool isMemoryDownload;
		protected Thread dlThread;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebDownload"/> class.
		/// </summary>
		/// <param name="url">The URL to download from.</param>
		public WebDownloadWithReferer( string url ) {
			this.Url = url;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:WorldWind.Net.WebDownload"/> class.
		/// </summary>
		public WebDownloadWithReferer() {
		}

		/// <summary>
		/// Whether the download is currently being processed (active).
		/// </summary>
		public bool IsDownloadInProgress {
			get {
				return dlThread != null && dlThread.IsAlive;
			}
		}

		/// <summary>
		/// Contains the exception that occurred during download, or null if successful.
		/// </summary>
		public Exception Exception {
			get {
				return downloadException;
			}
		}

		/// <summary>
		/// Asynchronous download of HTTP data to file. 
		/// </summary>
		public void BackgroundDownloadFile() {
			if (CompleteCallback==null)
				throw new ArgumentException("No download complete callback specified.");

			dlThread = new Thread(new ThreadStart(Download));
			dlThread.Name = "WebDownload.dlThread";
			dlThread.IsBackground = true;
			dlThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			dlThread.Start();
		}
	
		/// <summary>
		/// Asynchronous download of HTTP data to file.
		/// </summary>
		public void BackgroundDownloadFile( DownloadCompleteHandler completeCallback ) {
			CompleteCallback += completeCallback;
			BackgroundDownloadFile();
		}
	
		/// <summary>
		/// Download image of specified type. (handles server errors for wms type)
		/// </summary>
		public void BackgroundDownloadFile( DownloadType dlType ) {
			DownloadType = dlType;
			BackgroundDownloadFile();
		}

		/// <summary>
		/// Asynchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void BackgroundDownloadMemory() {
			if (CompleteCallback==null)
				throw new ArgumentException("No download complete callback specified.");

			isMemoryDownload = true;
			dlThread = new Thread(new ThreadStart(Download));
			dlThread.Name = "WebDownload.dlThread(2)";
			dlThread.IsBackground = true;
			dlThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			dlThread.Start();
		}
	
		/// <summary>
		/// Asynchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void BackgroundDownloadMemory( DownloadCompleteHandler completeCallback ) {
			CompleteCallback += completeCallback;
			BackgroundDownloadMemory();
		}
	
		/// <summary>
		/// Download image of specified type. (handles server errors for WMS type)
		/// </summary>
		/// <param name="dlType">Type of download.</param>
		public void BackgroundDownloadMemory( DownloadType dlType ) {
			DownloadType = dlType;
			BackgroundDownloadMemory();
		}

		/// <summary>
		/// Synchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void DownloadMemory() {
			isMemoryDownload = true;
			Download();
		}

		/// <summary>
		/// Download image of specified type. (handles server errors for WMS type)
		/// </summary>
		public void DownloadMemory( DownloadType dlType ) {
			DownloadType = dlType;
			DownloadMemory();
		}

		/// <summary>
		/// HTTP downloads to memory.
		/// </summary>
		/// <param name="progressCallback">The progress callback.</param>
		public void DownloadMemory( DownloadProgressHandler progressCallback ) {
			ProgressCallback += progressCallback;
			DownloadMemory();
		}

		/// <summary>
		/// Synchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void DownloadFile( string destinationFile ) {
			SavedFilePath = destinationFile;

			Download();
		}

		/// <summary>
		/// Download image of specified type to a file. (handles server errors for WMS type)
		/// </summary>
		public void DownloadFile( string destinationFile, DownloadType dlType ) {
			DownloadType = dlType;
			DownloadFile(destinationFile);
		}

		/// <summary>
		/// Saves a http in-memory download to file.
		/// </summary>
		/// <param name="destinationFilePath">File to save the downloaded data to.</param>
		public void SaveMemoryDownloadToFile(string destinationFilePath ) {
			if(ContentStream==null)
				throw new InvalidOperationException("No data available.");

			// Cache the capabilities on file system
			ContentStream.Seek(0,SeekOrigin.Begin);
			using(Stream fileStream = File.Create(destinationFilePath)) {
				if(ContentStream is MemoryStream) {
					// Write the MemoryStream buffer directly (2GB limit)
					MemoryStream ms = (MemoryStream)ContentStream;
					fileStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
				}
				else {
					// Block copy
					byte[] buffer = new byte[4096];
					while(true) {
						int numRead = ContentStream.Read(buffer, 0, buffer.Length);
						if(numRead<=0)
							break;
						fileStream.Write(buffer,0,numRead);
					}
				}
			}
			ContentStream.Seek(0,SeekOrigin.Begin);
		}

		/// <summary>
		/// Aborts the current download. 
		/// </summary>
		public void Cancel() {
			CompleteCallback = null;
			ProgressCallback = null;
			if (dlThread!=null && dlThread != Thread.CurrentThread) {
				if(dlThread.IsAlive)
					dlThread.Abort();
				dlThread = null;
			}		
		}


		/// <summary>
		/// Called when downloading has ended.
		/// </summary>
		/// <param name="wd">The download.</param>
		private static void OnDownloadEnded(WebDownloadWithReferer wd) {
			if (DownloadEnded != null) {
				DownloadEnded(wd);
			}
		}

		/// <summary>
		/// Synchronous HTTP download
		/// </summary>
		protected void Download() {
			Debug.Assert(Url.StartsWith("http://"));
			DownloadStartTime = DateTime.Now;
			try {
				try {
					
					// create content stream from memory or file
					if (isMemoryDownload && ContentStream == null) {
						ContentStream = new MemoryStream();
					}
					else {
						// Download to file
						string targetDirectory = Path.GetDirectoryName(SavedFilePath);
						if(targetDirectory.Length > 0)
							Directory.CreateDirectory(targetDirectory);
						ContentStream = new FileStream(SavedFilePath, FileMode.Create);
					}

					// Create the request object.
					request = (HttpWebRequest) WebRequest.Create(Url);
					request.UserAgent = UserAgent;
					request.Referer = refererURL;

					request.Proxy = ProxyHelper.DetermineProxyForUrl(
						Url, 
						useWindowsDefaultProxy, 
						useDynamicProxy, 
						proxyUrl, 
						proxyUserName, 
						proxyPassword);

					using (response = request.GetResponse() as HttpWebResponse) {
						// only if server responds 200 OK
						if (response.StatusCode == HttpStatusCode.OK) {
							ContentType = response.ContentType;

							// Find the data size from the headers.
							string strContentLength = response.Headers["Content-Length"];
							if (strContentLength != null) {
								ContentLength = int.Parse(strContentLength, CultureInfo.InvariantCulture);
							}

							byte[] readBuffer = new byte[1500];
							using (Stream responseStream = response.GetResponseStream()) {
								while (true) {
									//  Pass do.readBuffer to BeginRead.
									int bytesRead = responseStream.Read(readBuffer, 0, readBuffer.Length);
									if (bytesRead <= 0)
										break;

									ContentStream.Write(readBuffer, 0, bytesRead);
									BytesProcessed += bytesRead;

								}
							}
						}
					}

					HandleErrors();
				}
				catch (System.Configuration.ConfigurationException) {
					// is thrown by WebRequest.Create if App.config is not in the correct format
					// TODO: don't know what to do with it
					throw;
				}
				catch (Exception caught) {
					try {
						// Remove broken file download
						if (ContentStream != null) {
							ContentStream.Close();
							ContentStream = null;
						}
						if (SavedFilePath != null && SavedFilePath.Length > 0) {
							File.Delete(SavedFilePath);
						}
					} 
					catch(Exception) {
					}
					SaveException(caught);
				}

				

				if (ContentStream is MemoryStream) {
					ContentStream.Seek(0, SeekOrigin.Begin);
				}
				else if (ContentStream != null) {
					ContentStream.Close();
					ContentStream = null;
				}
			
				
				if (CompleteCallback == null) {
					Verify();
				}
				else {
					CompleteCallback(this);
				}
			}
			catch (ThreadAbortException) {
			 }
			finally {
				IsComplete = true;
			}

			OnDownloadEnded(this);
		}

		/// <summary>
		/// Handle server errors that don't get trapped by the web request itself.
		/// </summary>
		private void HandleErrors() {
			// HACK: Workaround for TerraServer failing to return 404 on not found
			if(ContentStream.Length == 15) {
				// a true 404 error is a System.Net.WebException, so use the same text here
				Exception ex = new FileNotFoundException("The remote server returned an error: (404) Not Found.", SavedFilePath );
				SaveException(ex);
			}

			// TODO: WMS 1.1 content-type != xml
			// TODO: Move WMS logic to WmsDownload
			if (DownloadType == DownloadType.Wms && (
				ContentType.StartsWith("text/xml") ||
				ContentType.StartsWith("application/vnd.ogc.se"))) {
				// WMS request failure
				SetMapServerError();
			}
		}

		/// <summary>
		/// If exceptions occurred they will be thrown by calling this function.
		/// </summary>
		public void Verify() {
			if(Exception!=null)
				throw Exception;
		}

		/// <summary>
		/// Log download error to log file
		/// </summary>
		/// <param name="exception"></param>
		private void SaveException( Exception exception ) {
			// Save the exception 
			downloadException = exception;

			if(Exception is ThreadAbortException)
				// Don't log canceled downloads
				return;

			if(Log404Errors) {
				Log.Write( "HTTP", "Error: " + Url );
				Log.Write( "HTTP", "     : " + exception.Message );
			}
		}

		/// <summary>
		/// Reads the xml response from the server and throws an error with the message.
		/// </summary>
		private void SetMapServerError() {
			try {
				XmlDocument errorDoc = new XmlDocument();
				ContentStream.Seek(0,SeekOrigin.Begin);
				errorDoc.Load(ContentStream);
				string msg = "";
				foreach( XmlNode node in errorDoc.GetElementsByTagName("ServiceException"))
					msg += node.InnerText.Trim()+Environment.NewLine;
				SaveException( new WebException(msg.Trim()) );
			}
			catch(XmlException) {
				SaveException( new WebException("An error occurred while trying to download " + request.RequestUri.ToString()+".") );
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			if (dlThread!=null && dlThread != Thread.CurrentThread) {
				if(dlThread.IsAlive)
					dlThread.Abort();
				dlThread = null;
			}

			if(request!=null) {
				request.Abort();
				request = null;
			}

			if (ContentStream != null) {
				ContentStream.Close();
				ContentStream=null;
			}

			GC.SuppressFinalize(this);
		}
		#endregion

	}
	#endregion

}
