using System.Collections;
using UnityEngine;

/// <summary>
/// Script để load shader từ Resources và đảm bảo không bị strip
/// </summary>
public class ShaderLoader : MonoBehaviour
{
    [Header("Shader Resources")]
    [SerializeField] private Shader[] requiredShaders;
    [SerializeField] private Material[] requiredMaterials;

    private static ShaderLoader _instance;
    public static ShaderLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ShaderLoader>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ShaderLoader");
                    _instance = go.AddComponent<ShaderLoader>();
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
            LoadShadersFromResources();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void LoadShadersFromResources()
    {
        // Load shader references từ Resources
        var shaderReferences = Resources.Load<ShaderManager>("ShaderReferences");
        if (shaderReferences != null)
        {
            Debug.Log("Đã load ShaderReferences từ Resources");
        }

        // Đảm bảo các shader cần thiết được load
        StartCoroutine(EnsureShadersLoaded());
    }

    private IEnumerator EnsureShadersLoaded()
    {
        yield return new WaitForEndOfFrame();
        
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
                Debug.Log($"Shader đã sẵn sàng: {shaderName}");
            }
            else
            {
                Debug.LogWarning($"Shader không tìm thấy: {shaderName}");
            }
        }
    }

    /// <summary>
    /// Tìm shader phù hợp cho SkeletonGraphic với fallback
    /// </summary>
    public Shader FindSpineShader()
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
                return shader;
            }
        }

        return null;
    }

    /// <summary>
    /// Tạo material cho SkeletonGraphic với shader phù hợp
    /// </summary>
    public Material CreateSpineMaterial()
    {
        Shader shader = FindSpineShader();
        if (shader != null)
        {
            Material material = new Material(shader);
            Debug.Log($"Đã tạo material với shader: {shader.name}");
            return material;
        }
        
        Debug.LogError("Không thể tạo material cho SkeletonGraphic - không tìm thấy shader phù hợp");
        return null;
    }
}
