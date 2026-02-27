// WebGLTextureDiagnostics.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Text;

public class WebGLTextureDiagnostics : EditorWindow
{
    [MenuItem("Tools/WebGL/4. Texture Diagnostics Window")]
    static void ShowWindow()
    {
        GetWindow<WebGLTextureDiagnostics>("Texture Diagnostics");
    }
    
    Vector2 scrollPos;
    
    void OnGUI()
    {
        if (GUILayout.Button("Run Diagnostics", GUILayout.Height(30)))
        {
            RunDiagnostics();
        }
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label(diagnosticsText);
        EditorGUILayout.EndScrollView();
    }
    
    string diagnosticsText = "";
    
    void RunDiagnostics()
    {
        StringBuilder sb = new StringBuilder();
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture");
        
        sb.AppendLine($"=== TEXTURE DIAGNOSTICS REPORT ===");
        sb.AppendLine($"Total textures found: {textureGUIDs.Length}");
        sb.AppendLine();
        
        int count = 0;
        foreach (string guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null && count < 50) // 限制显示数量
            {
                sb.AppendLine($"[{count + 1}] {System.IO.Path.GetFileName(path)}");
                sb.AppendLine($"  Path: {path}");
                sb.AppendLine($"  Type: {importer.textureType}");
                sb.AppendLine($"  Size: {importer.maxTextureSize}");
                
                TextureImporterPlatformSettings webglSettings = importer.GetPlatformTextureSettings("WebGL");
                sb.AppendLine($"  WebGL Overridden: {webglSettings.overridden}");
                sb.AppendLine($"  WebGL Format: {webglSettings.format}");
                sb.AppendLine($"  WebGL Max Size: {webglSettings.maxTextureSize}");
                
                TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                sb.AppendLine($"  Default Format: {defaultSettings.format}");
                
                if (IsUnsafeFormat(importer))
                    sb.AppendLine($"  ⚠️ POTENTIALLY UNSAFE FOR WEBGL!");
                
                sb.AppendLine();
                count++;
            }
        }
        
        if (textureGUIDs.Length > 50)
            sb.AppendLine($"... and {textureGUIDs.Length - 50} more textures");
        
        diagnosticsText = sb.ToString();
        Repaint();
        
        // 自动分析
        AnalyzeForWebGL();
    }
    
    void AnalyzeForWebGL()
    {
        StringBuilder analysis = new StringBuilder();
        string[] textureGUIDs = AssetDatabase.FindAssets("t:Texture");
        
        int unsafeCount = 0;
        int overriddenCount = 0;
        int needsFixCount = 0;
        
        foreach (string guid in textureGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("WebGL");
                
                if (settings.overridden) overriddenCount++;
                if (IsUnsafeFormat(importer)) unsafeCount++;
                
                // 检查是否需要修复
                if (!settings.overridden || 
                    IsUnsafeFormat(settings.format) || 
                    settings.maxTextureSize > 2048)
                {
                    needsFixCount++;
                }
            }
        }
        
        analysis.AppendLine($"\n=== ANALYSIS FOR WEBGL ===");
        analysis.AppendLine($"Textures with WebGL override: {overriddenCount}/{textureGUIDs.Length}");
        analysis.AppendLine($"Potentially unsafe formats: {unsafeCount}");
        analysis.AppendLine($"Textures needing fix: {needsFixCount}");
        
        Debug.Log(analysis.ToString());
    }
    
    bool IsUnsafeFormat(TextureImporter importer)
    {
        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("WebGL");
        if (!settings.overridden)
        {
            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            return IsUnsafeFormat(defaultSettings.format);
        }
        
        return IsUnsafeFormat(settings.format);
    }
    
    bool IsUnsafeFormat(TextureImporterFormat format)
    {
        TextureImporterFormat[] unsafeFormats = {
            TextureImporterFormat.DXT1,
            TextureImporterFormat.DXT1Crunched,
            TextureImporterFormat.DXT5,
            TextureImporterFormat.DXT5Crunched,
            TextureImporterFormat.BC4,
            TextureImporterFormat.BC5,
            TextureImporterFormat.BC6H,
            TextureImporterFormat.BC7
        };
        
        foreach (var unsafeFormat in unsafeFormats)
        {
            if (format == unsafeFormat) return true;
        }
        
        return false;
    }
}
#endif