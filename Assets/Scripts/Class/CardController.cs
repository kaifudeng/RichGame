using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{

    public bool state;
    public int useMaterialIndex;
    public char x;
    public int y;

    public Material material;

    /// <summary>
    /// 所属公司
    /// </summary>
    public Company Company;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Material>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 更新材质
    /// </summary>
    /// 
    public void UpdateMaterial(int index)
    {
        useMaterialIndex = index;
        if(material == null)
        {
            material = GetComponent<Material>();
        }
        switch (index)
        {
            case 0:
                {
                    material = ShaderChange.GetShaderChange().gray;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 1:
                {
                    material = ShaderChange.GetShaderChange().lightGray;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 2:
                {
                    material = ShaderChange.GetShaderChange().yellow;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 3:
                {
                    material = ShaderChange.GetShaderChange().red;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 4:
                {
                    material = ShaderChange.GetShaderChange().blue;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 5:
                {
                    material = ShaderChange.GetShaderChange().green;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 6:
                {
                    material = ShaderChange.GetShaderChange().orange;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 7:
                {
                    material = ShaderChange.GetShaderChange().purple;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 8:
                {
                    material = ShaderChange.GetShaderChange().greenyellow;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 9:
                {
                    material = ShaderChange.GetShaderChange().white;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
                case 10:
                {
                    material = ShaderChange.GetShaderChange().black;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            case 11:
                {
                    material = ShaderChange.GetShaderChange().AliceBlue;
                    ShaderChange.ChangeMaterial(gameObject, material); break;
                }
            default: break;
        }
    }

    public void RefeshMaterial()
    {
        UpdateMaterial(useMaterialIndex);
    }

    public int getMaterial()
    {
        return useMaterialIndex;
    }
    public override string ToString()
    {
        string str = x+y.ToString();
        return str;
    }
}
