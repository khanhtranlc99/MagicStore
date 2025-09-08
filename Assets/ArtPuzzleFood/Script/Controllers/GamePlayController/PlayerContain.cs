using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class PlayerContain : MonoBehaviour
{
    public LevelData levelData;
    public Transform postLevel;
    public HScrollController hScrollController;
    public Pieces pieces;
    public Transform postScroll;
    public Material _shaderChange;
    public Material _colorChange;
    public BoosterHint boosterHint;

    public ScrollRect scrollView;
    public RectTransform viewPort;
    public RectTransform content;
    public void Init(Action callBack)
    {
        StartCoroutine(LoadLevelFromAssetBundle(callBack));
    }

    private IEnumerator LoadLevelFromAssetBundle(Action callBack)
    {
        string bundleName = string.Format("level_{0}", UseProfile.LevelEggChest);
        string prefabName = string.Format("Level_{0}", UseProfile.LevelEggChest);
        
        // Tạo đường dẫn asset bundle theo platform
        string bundlePath = "";
        #if UNITY_ANDROID
            bundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);
        #elif UNITY_IOS
            bundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);
        #else
            bundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, bundleName);
        #endif
        
        Debug.Log($"bundlePath: {bundlePath}");
        
        AssetBundle bundle = null;
        
        #if UNITY_ANDROID || UNITY_IOS
            // Trên thiết bị thực, sử dụng UnityWebRequest để load asset bundle
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundlePath))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    bundle = DownloadHandlerAssetBundle.GetContent(www);
                    Debug.Log($"Đã load asset bundle thành công từ: {bundlePath}");
                }
                else
                {
                    Debug.LogError($"Lỗi load asset bundle: {www.error}");
                }
            }
        #else
            // Trên Editor, sử dụng File.Exists để kiểm tra
            if (System.IO.File.Exists(bundlePath))
            {
                Debug.Log($"File tồn tại: {bundlePath}");
                bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle == null)
                {
                    Debug.LogError($"Không thể load asset bundle: {bundlePath}");
                }
            }
            else
            {
                Debug.LogError($"File không tồn tại: {bundlePath}");
            }
        #endif
        
        // Xử lý bundle nếu load thành công
        if (bundle != null)
        {
            Debug.Log($"Load level {UseProfile.LevelEggChest} từ asset bundle: {bundlePath}");
            // Load prefab từ bundle
            GameObject levelPrefab = bundle.LoadAsset<GameObject>(prefabName);
            if (levelPrefab != null)
            {
                // Instantiate prefab và lấy component LevelData
                GameObject levelObj = Instantiate(levelPrefab);
                levelData = levelObj.GetComponent<LevelData>();
                
                if (levelData != null)
                {
                    levelData.transform.SetParent(postLevel, false);
                    levelData.Init(this);
                    boosterHint.Init();
                    bundle.Unload(false);
                    Debug.Log($"Đã load level {UseProfile.LevelEggChest} từ asset bundle: {bundlePath}");
                    callBack?.Invoke();
                    yield break;
                }
                else
                {
                    Debug.LogError($"Prefab không có component LevelData: {prefabName}");
                    DestroyImmediate(levelObj);
                }
            }
            else
            {
                Debug.LogError($"Không thể load prefab từ asset bundle: {bundleName}, prefab: {prefabName}");
            }
            bundle.Unload(true);
        }
        
        // Fallback về Resources nếu không load được từ asset bundle
        Debug.Log("Fallback về Resources...");
        // string pathLevel = StringHelper.PATH_CONFIG_LEVEL_TEST;
        // levelData = Instantiate(Resources.Load<LevelData>(string.Format(pathLevel, UseProfile.LevelEggChest)));
        // levelData.transform.SetParent(postLevel, false);
        // levelData.Init(this);
        // boosterHint.Init();
    }
}
