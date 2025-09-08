using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý shader references để đảm bảo shader không bị strip khỏi build
/// </summary>
public class ShaderManager : MonoBehaviour
{
    [Header("Shader References - Để Unity không strip shader")]
    [SerializeField] private List<Shader> spineShaders = new List<Shader>();
    [SerializeField] private List<Material> spineMaterials = new List<Material>();
    
    private static ShaderManager _instance;
    public static ShaderManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ShaderManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ShaderManager");
                    _instance = go.AddComponent<ShaderManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeShaders();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeShaders()
    {
        // Thêm các shader cần thiết vào list để Unity không strip
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
            if (shader != null && !spineShaders.Contains(shader))
            {
                spineShaders.Add(shader);
                Debug.Log($"Đã thêm shader vào ShaderManager: {shaderName}");
            }
        }
    }

    /// <summary>
    /// Tìm shader phù hợp cho SkeletonGraphic
    /// </summary>
    public Shader GetSpineShader()
    {
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
                Debug.Log($"ShaderManager tìm thấy shader: {shaderName}");
                return shader;
            }
        }

        Debug.LogError("ShaderManager: Không tìm thấy shader nào phù hợp!");
        return null;
    }

    /// <summary>
    /// Tạo material cho SkeletonGraphic
    /// </summary>
    public Material CreateSpineMaterial()
    {
        Shader shader = GetSpineShader();
        if (shader != null)
        {
            Material material = new Material(shader);
            if (!spineMaterials.Contains(material))
            {
                spineMaterials.Add(material);
            }
            return material;
        }
        return null;
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Shader List")]
    private void RefreshShaderList()
    {
        spineShaders.Clear();
        InitializeShaders();
        Debug.Log("Đã refresh danh sách shader");
    }
#endif
}
