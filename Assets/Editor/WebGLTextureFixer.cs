// WebGLTextureFixer.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WebGLTextureFixer : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            ApplySafeWebGLTextureSettings(assetImporter as TextureImporter);
        }
    }
    
    [MenuItem("Tools/WebGL/1. Fix All Textures")]
    static void FixAllTextures()
    {
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture");
        List<string> fixedTextures = new List<string>();
        List<string> skippedTextures = new List<string>();
        
        foreach (string guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                if (ApplySafeWebGLTextureSettings(importer))
                {
                    fixedTextures.Add(System.IO.Path.GetFileName(path));
                    EditorUtility.SetDirty(importer);
                }
                else
                {
                    skippedTextures.Add(System.IO.Path.GetFileName(path));
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"=== WebGL Texture Fix Summary ===");
        Debug.Log($"Fixed: {fixedTextures.Count} textures");
        Debug.Log($"Skipped: {skippedTextures.Count} textures");
        
        if (fixedTextures.Count > 0)
        {
            Debug.Log("Fixed textures:");
            foreach (string name in fixedTextures.Take(10)) // 只显示前10个
            {
                Debug.Log($"  - {name}");
            }
            if (fixedTextures.Count > 10)
                Debug.Log($"  ... and {fixedTextures.Count - 10} more");
        }
    }
    
    [MenuItem("Tools/WebGL/2. Check Problematic Textures")]
    static void CheckProblematicTextures()
    {
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture");
        List<string> problematic = new List<string>();
        
        foreach (string guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                // 检查是否为不安全格式
                if (IsUnsafeFormat(importer))
                {
                    problematic.Add($"{System.IO.Path.GetFileName(path)} (Format: {GetCurrentFormat(importer)})");
                }
            }
        }
        
        Debug.Log($"=== Problematic Textures Check ===");
        Debug.Log($"Total textures: {textureGUIDs.Length}");
        Debug.Log($"Potentially problematic: {problematic.Count}");
        
        if (problematic.Count > 0)
        {
            Debug.Log("Problematic textures (may cause WebGL errors):");
            foreach (string problem in problematic)
            {
                Debug.Log($"  - {problem}");
            }
        }
    }
    
    [MenuItem("Tools/WebGL/3. Force Reset All WebGL Settings")]
    static void ForceResetAllWebGLSettings()
    {
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture");
        int resetCount = 0;
        
        foreach (string guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("WebGL");
                settings.overridden = false; // 重置覆盖
                importer.SetPlatformTextureSettings(settings);
                resetCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Reset WebGL settings for {resetCount} textures");
        Debug.Log("Now run 'Fix All Textures' to apply safe settings");
    }
    
    static bool ApplySafeWebGLTextureSettings(TextureImporter importer)
    {
        if (importer == null) return false;
        
        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("WebGL");
        TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
        
        bool needsFix = false;
        
        // 检查是否需要修复的条件
        if (!settings.overridden)
        {
            needsFix = true;
        }
        else if (IsUnsafeFormat(settings.format))
        {
            needsFix = true;
        }
        else if (settings.maxTextureSize > 2048)
        {
            needsFix = true;
        }
        
        if (needsFix)
        {
            settings.overridden = true;
            settings.maxTextureSize = Mathf.Min(importer.maxTextureSize, 2048);
            
            // 根据平台支持选择格式
            if (SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_6x6))
            {
                settings.format = TextureImporterFormat.ASTC_6x6;
            }
            else if (SystemInfo.SupportsTextureFormat(TextureFormat.ETC2_RGBA8))
            {
                settings.format = TextureImporterFormat.ETC2_RGBA8;
            }
            else
            {
                // 回退到安全的未压缩格式
                settings.format = TextureImporterFormat.RGBA32;
            }
            
            // 对于法线贴图，使用专用格式
            if (importer.textureType == TextureImporterType.NormalMap)
            {
                settings.format = TextureImporterFormat.RGBA32; // 法线贴图最好不压缩
            }
            
            // 对于UI纹理，使用无压缩或RGBA32
            if (importer.textureType == TextureImporterType.Sprite && 
                importer.spriteImportMode != SpriteImportMode.None)
            {
                settings.format = TextureImporterFormat.RGBA32;
            }
            
            importer.SetPlatformTextureSettings(settings);
            return true;
        }
        
        return false;
    }
    
    static bool IsUnsafeFormat(TextureImporter importer)
    {
        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("WebGL");
        if (!settings.overridden)
        {
            // 使用默认设置，检查默认格式
            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            return IsUnsafeFormat(defaultSettings.format);
        }
        
        return IsUnsafeFormat(settings.format);
    }
    
    static bool IsUnsafeFormat(TextureImporterFormat format)
    {
        // WebGL 不安全的格式列表
        TextureImporterFormat[] unsafeFormats = {
            TextureImporterFormat.DXT1,
            TextureImporterFormat.DXT1Crunched,
            TextureImporterFormat.DXT5,
            TextureImporterFormat.DXT5Crunched,
            TextureImporterFormat.BC4,
            TextureImporterFormat.BC5,
            TextureImporterFormat.BC6H,
            TextureImporterFormat.BC7,
            TextureImporterFormat.PVRTC_RGB2,
            TextureImporterFormat.PVRTC_RGBA2,
            TextureImporterFormat.PVRTC_RGB4,
            TextureImporterFormat.PVRTC_RGBA4
        };
        
        return unsafeFormats.Contains(format);
    }
    
    static string GetCurrentFormat(TextureImporter importer)
    {
        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("WebGL");
        if (!settings.overridden)
        {
            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            return $"{defaultSettings.format} (Default)";
        }
        
        return settings.format.ToString();
    }
}
#endif