using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChange:MonoBehaviour
{
    public static ShaderChange shaderChange;

    public static ShaderChange GetShaderChange() => shaderChange;


    public Material green;
    public Material blue;
    public Material red;
    public Material greenyellow;
    public Material orange;
    public Material purple;
    public Material white;

    public Material yellow;
    public Material gray;
    public Material lightGray;
    public Material AliceBlue;

    public Material black;

    private void Awake()
    {
        shaderChange=this;
    }

    /// <summary>
    /// 更改游戏对象材质的方法
    /// </summary>
    /// <param name="gameObject">要更改的对象</param>
    /// <param name="material">新的材质</param>
    public static void ChangeMaterial(GameObject gameObject,Material material)
    {
        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        materials[0]=material;
        gameObject.GetComponent<Renderer>().materials = materials;
    }
}
