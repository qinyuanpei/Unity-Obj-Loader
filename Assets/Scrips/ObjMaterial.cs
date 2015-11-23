
/*
 * .mtl文件材质描述类
 * 作者：秦元培
 * 时间：2015年11月11日
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class ObjMaterial : MonoBehaviour
{
    /// <summary>
    /// 全局变量
    /// </summary>
    private Texture globalTexture;

	/// <summary>
	/// 材质名称列表
	/// </summary>
	private ArrayList materialNames;

	/// <summary>
	/// 漫反射颜色列表
	/// </summary>
	private List<Vector3> diffuseColors;

	/// <summary>
	/// 漫反射贴图列表
	/// </summary>
	private ArrayList diffuseTextures;

    /// <summary>
    /// 当前实例
    /// </summary>
    private static ObjMaterial instance;
    public static ObjMaterial Instance
    {
        get 
        {
            if(instance == null)
                instance = GameObject.FindObjectOfType<ObjMaterial>();
            return instance; 
        }
    }

	void Awake()
	{
		this.diffuseColors = new List<Vector3>();
		this.diffuseTextures = new ArrayList();
		this.materialNames = new ArrayList();
	}

	/// <summary>
	/// 从一个文本化后的mtl文件加载一组材质
	/// </summary>
	/// <param name="mtlText">文本化的mtl文件</param>
    /// <param name="texturePath">贴图文件夹路径</param>
	public Material[] LoadFormMtl(string mtlText,string texturePath)
	{
		if(mtlText=="")
			return null;

		//将文本化后的内容按行分割
		string[] allLines = mtlText.Split('\n');
		foreach(string line in allLines)
		{
			//按照空格分割每一行的内容
			string[] chars = line.Split(' ');
			switch(chars[0])
			{
				case "newmtl":
				//处理材质名
				materialNames.Add(chars[1]);
					break;
				case "Ka":
				//暂时仅考虑漫反射
					break;
				case "Kd": 
				//处理漫反射
				diffuseColors.Add(new Vector3(
					ConvertToFloat(chars[1]),
					ConvertToFloat(chars[2]),
					ConvertToFloat(chars[3])
					));
					break;
				case "Ks":
                //暂时仅考虑漫反射
					break;
				case "Ke":
				//Todo
					break;
				case "Ni":
				//Todo
					break;
				case "e": 
				//Todo
					break;
				case "illum": 
				//Todo
					break;
				case "map_Ka":
				//暂时仅考虑漫反射
					break;
				case "map_Kd":
				//处理漫反射贴图
				//因为mtl文件中的贴图使用的是绝对路径
				//所以这里需要截取它的文件名来和材质相对应起来
                string textureName = chars[1].Substring(chars[1].LastIndexOf("\\") + 1,chars[1].Length - chars[1].LastIndexOf("\\") - 1);
                //默认贴图格式为.png
                textureName = textureName.Replace(".dds",".png");
				diffuseTextures.Add(textureName);
					break;
				case "map_Ks":
                //暂时仅考虑漫反射
					break;

			}
		}

        //准备一个数组来存储材质
		Material[] materials=new Material[materialNames.Count];

        for(int i = 0; i < materialNames.Count; i++)
        {
            //创建一个内置的Diffuse材质
            Material mat = new Material(Shader.Find("Diffuse"));
            //设置材质名称
            mat.name = materialNames[i].ToString();
            //加载贴图
            StartCoroutine(LoadTexture(texturePath + "/" + diffuseTextures[i]));
            //设置贴图
            mat.mainTexture = globalTexture;
            materials[i] = mat;
        }

        return materials;
	}


    /// <summary>
    /// 将一个字符串转换为浮点类型
    /// </summary>
    /// <param name="s">待转换的字符串</param>
    /// <returns></returns>
    private float ConvertToFloat(string s)
	{
		return System.Convert.ToSingle(s, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// 加载指定路径的贴图	
    /// </summary>
	/// <returns>The texture.</returns>
	/// <param name="fileName">贴图路径</param>
	IEnumerator  LoadTexture(string fileName)
	{
        //根据路径名称自动判断是从网络加载还是从本地加载
		if(!fileName.StartsWith("http://"))
            fileName  = "file://" + fileName;

        //使用WWW下载贴图
        WWW www = new WWW(fileName);
        yield return www;

        if(www != null && !string.IsNullOrEmpty(www.error)){
            if(www.isDone)
               globalTexture = www.texture;
        }else{
            globalTexture = null;
        } 
	}
}
