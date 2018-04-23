using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoshopFile;
using UnityEngine;
using UnityEditor;

public class PanelNode
{
	private string _name;
	public string Name{get{return _name;}}

	public bool visible;
	public PanelNode Parent = null;


	public  bool isGroup;
	public int idxStart;
	public int idxEnd;

	public int Index;
	public Layer layer;


	public List<PanelNode> childNodeList = new List<PanelNode>();
	public PanelNode(string name)
	{
		this._name = name;
	}
	public void AddChild (PanelNode panelNode)
	{
		panelNode.Parent = this;
		childNodeList.Add (panelNode);
	}
}

namespace subjectnerdagreement.psdexport
{
	public class PSDConvert : Editor
	{
		private const string OutPutDirPath = @"Assets\ResourcesLib\Images\";
		[MenuItem ("Assets/PSD导出/ItemICon")]
		private static void ExportSprite_Pub(){
			Texture2D psdImg = (Texture2D)Selection.objects [0];
			string path = AssetDatabase.GetAssetPath (psdImg);
			if (!Path.GetExtension (path).Equals (".psd")) {
				Debug.LogError ("该文件不是psd文件。");
				return;
			}
			string RootName = Path.GetFileNameWithoutExtension (AssetDatabase.GetAssetPath (psdImg.GetInstanceID ()));
			List<PanelNode> exportLayerList = GetLayer (psdImg);
			if (exportLayerList != null && exportLayerList.Count > 0) {
				ExportSprite (exportLayerList,RootName);
			}
		}

		[MenuItem ("Assets/PSD导出/面板导出")]
		private static void ImportPsdWindow ()
		{
			Texture2D psdImg = (Texture2D)Selection.objects [0];
			if (psdImg == null) {
				Debug.LogError ("请选取一个PSD文件");
				return;
			}

			RectTransform parentCanvas = GameObject.FindObjectOfType<Canvas> ().GetComponent<RectTransform> ();

			string RootName = Path.GetFileNameWithoutExtension (AssetDatabase.GetAssetPath (psdImg.GetInstanceID ()));
			PanelNode panelNodeRoot = GetPanelNodeRoot (psdImg);
			RectTransform layerRoot = GameObjectPanel (panelNodeRoot.Name,parentCanvas);
			CreatePanel (panelNodeRoot,layerRoot);
		}
		private static void ArrangementNode(List<PanelNode> PanelNodeList)
		{
			for (int i = 0; i < PanelNodeList.Count; i++) {
				PanelNode panelNode = PanelNodeList [i];
				List<PanelNode> parentNodeList;
				if (panelNode.isGroup) {
					parentNodeList = PanelNodeList.FindAll (a => (a.idxStart < panelNode.idxStart && a.idxEnd > panelNode.idxEnd));
				} else {
					parentNodeList = PanelNodeList.FindAll (a => (a.idxStart < panelNode.Index && a.idxEnd > panelNode.Index));
				}
				if (parentNodeList.Count > 0) {
					parentNodeList.Sort ((left, right) => left.idxStart.CompareTo (right.idxStart));
					PanelNode parentNode = parentNodeList [parentNodeList.Count-1];
					panelNode.Parent = parentNode;
					parentNode.AddChild (panelNode);
				}
			}
		}

		private static void CreatePanel(PanelNode panelRoot,RectTransform layerRoot){
			for (int i = 0; i < panelRoot.childNodeList.Count; i++) {
				if (panelRoot.childNodeList [i].isGroup) {
					RectTransform panelRectTrans = GameObjectPanel (panelRoot.childNodeList [i].Name,layerRoot);
					CreatePanel (panelRoot.childNodeList [i], panelRectTrans);
				} else {
					CreatePanel (panelRoot.childNodeList [i], layerRoot);
				}
			}
		}


		private static List<PanelNode> GetLayer(Texture2D psdFile, string filter=""){
			List<PanelNode> PanelNodeList = new List<PanelNode> ();
			PsdExportSettings settings = new PsdExportSettings(psdFile);
			PsdFileInfo fileInfo = new PsdFileInfo (settings.Psd);
			bool valid = (settings.Psd != null);
			if (valid) {
				settings.LoadLayers (fileInfo);
				PsdFile psd = settings.Psd;
				int layerIndexMin = 0;
				int layerIndexMax = 0;
				for (int i = 0; i < psd.Layers.Count; i++) {
					PSDLayerGroupInfo groupInfo = fileInfo.GetGroupByLayerIndex (i);
					//Debug.Log (groupInfo.start + "-" + groupInfo.end + ":" + groupInfo.name);
					if (groupInfo != null && !PanelNodeList.Exists(a=>a.Name.Equals(groupInfo.name))) {
						PanelNode panelNode = new PanelNode (groupInfo.name);
						panelNode.visible = groupInfo.visible;
						panelNode.idxStart = groupInfo.start;
						panelNode.idxEnd = groupInfo.end;
						panelNode.isGroup = true;
						PanelNodeList.Add (panelNode);
					}
				}   

				for (int i = 0; i < fileInfo.LayerIndices.Length; i++) {

					int layerIndex = fileInfo.LayerIndices [i];
					Layer layer = psd.Layers [layerIndex];
					PanelNode panelNode = new PanelNode (layer.Name);
					panelNode.visible = layer.Visible;
					panelNode.Index = layerIndex;
					panelNode.isGroup = false;
					panelNode.layer = layer;
					PanelNodeList.Add (panelNode);
				}
				ArrangementNode (PanelNodeList);
			}
			return PanelNodeList;
		}

		private static PanelNode GetPanelNodeRoot(Texture2D psdFile, string filter="")
		{
			return GetLayer (psdFile,filter).Find(a=>a.Parent == null);
		}

		private static void ExportSprite(List<PanelNode> layerList, string psdRootPath)
		{
			for (int i = 0; i < layerList.Count; i++) {
				if (layerList [i].isGroup || !layerList[i].visible || !layerList[i].Parent.visible)
					continue;

				Texture2D tex = CreateTexture (layerList[i].layer);
				if (tex != null) {
					string fileName = layerList[i].Name.Trim() + ".png";
					string savePath = OutPutDirPath + psdRootPath + "/" + fileName;
					SaveAsset (tex,savePath);
				}
			}
		}

		private static RectTransform GameObjectPanel (string panelName, Transform parent)
		{
			GameObject go = new GameObject (panelName, new System.Type[]{ typeof(RectTransform) });
			RectTransform panelRoot = go.GetComponent<RectTransform> ();
			panelRoot.SetParent (parent);

			panelRoot.localPosition = Vector3.zero;
			panelRoot.localScale = Vector3.one;

			panelRoot.anchorMin = Vector2.zero;
			panelRoot.anchorMax = Vector2.one;

			panelRoot.sizeDelta = Vector2.zero;
			panelRoot.anchoredPosition = Vector2.zero;
			return panelRoot;
		}

		private static Texture2D CreateTexture(Layer layer)
		{
			if ((int)layer.Rect.width == 0 || (int)layer.Rect.height == 0)
				return null;
			// For possible clip to document functionality
			//int fileWidth = psd.ColumnCount;
			//int fileHeight = psd.RowCount;

			//int textureWidth = (int) layer.Rect.width;
			//int textureHeight = (int) layer.Rect.height;

			Texture2D tex = new Texture2D((int)layer.Rect.width, (int)layer.Rect.height, TextureFormat.RGBA32, true);
			Color32[] pixels = new Color32[tex.width * tex.height];

			Channel red = (from l in layer.Channels where l.ID == 0 select l).First();
			Channel green = (from l in layer.Channels where l.ID == 1 select l).First();
			Channel blue = (from l in layer.Channels where l.ID == 2 select l).First();
			Channel alpha = layer.AlphaChannel;

			for (int i = 0; i < pixels.Length; i++)
			{
				byte r = red.ImageData[i];
				byte g = green.ImageData[i];
				byte b = blue.ImageData[i];
				byte a = 255;

				if (alpha != null)
					a = alpha.ImageData[i];

				int mod = i % tex.width;
				int n = ((tex.width - mod - 1) + i) - mod;
				pixels[pixels.Length - n - 1] = new Color32(r, g, b, a);
			}

			tex.SetPixels32(pixels);
			tex.Apply();

			return tex;
		}

		private static void SaveAsset (Texture2D tex, string savePath)
		{
			byte[] buf = tex.EncodeToPNG ();
			File.WriteAllBytes (savePath, buf);
			AssetDatabase.Refresh();

			TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(savePath);
			textureImporter.textureType = TextureImporterType.Sprite;
			// Read out the texture import settings so import pivot point can be changed
			TextureImporterSettings importSetting = new TextureImporterSettings();
			textureImporter.ReadTextureSettings(importSetting);

			// Pivot settings are the same but custom, set the vector
			//else if (settings.Pivot == SpriteAlignment.Custom)

			// Set the rest of the texture settings
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spriteImportMode = SpriteImportMode.Single;
			// Write in the texture import settings
			textureImporter.SetTextureSettings(importSetting);

			EditorUtility.SetDirty(tex);
			AssetDatabase.WriteImportSettingsIfDirty(savePath);
			AssetDatabase.ImportAsset(savePath, ImportAssetOptions.ForceUpdate);
		}
	}
}