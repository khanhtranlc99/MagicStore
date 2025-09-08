#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;

/// <summary>
/// Script để đảm bảo shader được include trong build
/// </summary>
public class ShaderBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Đang chuẩn bị build - Kiểm tra shader references...");
        
        // Tìm tất cả shader cần thiết
        string[] requiredShaders = {
            "Spine/SkeletonGraphic",
            "Spine/SkeletonGraphic - Premultiplied Alpha",
            "Spine/SkeletonGraphic - Straight Alpha",
            "UI/Default",
            "Sprites/Default"
        };

        List<Shader> foundShaders = new List<Shader>();
        
        foreach (string shaderName in requiredShaders)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                foundShaders.Add(shader);
                Debug.Log($"✓ Tìm thấy shader: {shaderName}");
            }
            else
            {
                Debug.LogWarning($"✗ Không tìm thấy shader: {shaderName}");
            }
        }

        // Tạo một GameObject tạm thời để giữ reference shader
        GameObject shaderHolder = new GameObject("ShaderHolder_Temp");
        ShaderLoader loader = shaderHolder.AddComponent<ShaderLoader>();
        
        // Gán shader vào array để Unity không strip
        var shaderArray = new Shader[foundShaders.Count];
        for (int i = 0; i < foundShaders.Count; i++)
        {
            shaderArray[i] = foundShaders[i];
        }
        
        // Sử dụng reflection để set private field
        var field = typeof(ShaderLoader).GetField("requiredShaders", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(loader, shaderArray);
        }

        Debug.Log($"Đã chuẩn bị {foundShaders.Count} shader cho build");
    }
}

/// <summary>
/// Menu item để test shader loading
/// </summary>
public class ShaderTestMenu
{
    [MenuItem("Tools/Test Shader Loading")]
    public static void TestShaderLoading()
    {
        Debug.Log("=== Test Shader Loading ===");
        
        string[] shaderNames = {
            "Spine/SkeletonGraphic",
            "Spine/SkeletonGraphic - Premultiplied Alpha",
            "Spine/SkeletonGraphic - Straight Alpha",
            "UI/Default",
            "Sprites/Default"
        };

        foreach (string shaderName in shaderNames)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                Debug.Log($"✓ {shaderName} - OK");
            }
            else
            {
                Debug.LogError($"✗ {shaderName} - NOT FOUND");
            }
        }
        
        Debug.Log("=== End Test ===");
    }
}
#endif
