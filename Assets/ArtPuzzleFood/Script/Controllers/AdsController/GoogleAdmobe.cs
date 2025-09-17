using GoogleMobileAds.Api;
using GoogleMobileAds.Common;             // AppStateEventNotifier
using System;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAdmobe : MonoBehaviour
{
    [SerializeField] private string AOAID = "ca-app-pub-9819920607806935/8050158984";

    public bool IsReady { get; private set; }
    public bool IsShowingAds { get; private set; }
    public bool isLoadingAOA = false;

    private AppOpenAd appOpenAd = null;
    private int AOAtryTimes = 0;
    private const int MaxAOARetry = 10;
    private float lastShowTime = -999f;
    private const float MinShowInterval = 3f; // tránh double-show

    bool isFirstGame;
    private void OnEnable()
    {
        // Tự show khi app quay lại foreground
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnDisable()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }

    private void OnDestroy()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
    }

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        Debug.Log("[Admob] Init");
        // Nếu muốn giữ qua scene:
        // DontDestroyOnLoad(gameObject);
        isFirstGame = true;
        MobileAds.Initialize(initStatus =>
        {
            var map = initStatus.getAdapterStatusMap();
            Debug.Log($"[Admob] Init status: {initStatus} - {map.Count}");
            foreach (var kv in map)
            {
                var status = kv.Value;
                if (status.InitializationState == AdapterState.Ready)
                    Debug.Log("[Admob] Adapter: " + kv.Key + " is initialized.");
                else
                    Debug.Log("[Admob] Adapter: " + kv.Key + " not ready.");
            }

            // Bắt đầu load AOA
            LoadAOA();
        });

        // KHÔNG set IsReady ở đây. Chỉ set khi load xong.
        IsReady = false;
    }

    private void LoadAOA()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("[Admob] No internet for AOA.");
            appOpenAd = null;
            isLoadingAOA = false;
            IsReady = false;
            return;
        }

        if (isLoadingAOA)
        {
            Debug.Log("[Admob] AOA is already loading.");
            return;
        }

        AOAtryTimes++;
        isLoadingAOA = true;

        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        Debug.Log("[Admob] Loading App Open Ad...");
        var adRequest = new AdRequest();

        // LƯU Ý: AppOpenAd.Load cần orientation
        AppOpenAd.Load(
            adUnitId: AOAID,
          
            request: adRequest,
            adLoadCallback: (AppOpenAd ad, LoadAdError error) =>
            {
                isLoadingAOA = false;

                if (error != null || ad == null)
                {
                    Debug.LogError("[Admob] AOA failed to load: " + error);
                    IsReady = false;

                    if (AOAtryTimes < MaxAOARetry)
                    {
                        // thử load lại
                        LoadAOA();
                    }
                    return;
                }

                Debug.Log("[Admob] AOA loaded. Response: " + ad.GetResponseInfo());
                AOAtryTimes = 0;
                appOpenAd = ad;
                RegisterAOAEventHandlers(appOpenAd);
                IsReady = true;
                if(isFirstGame && IsReady)
                {
                   TryShowAOAIfAvailable();
                    isFirstGame = false;
                }    
            });
    }

  
    private void RegisterAOAEventHandlers(AppOpenAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"[Admob] AOA paid {adValue.Value} {adValue.CurrencyCode}");
            OnAdRevenuePaidEvent(AOAID, "ADMOB_AOA", ad.GetResponseInfo(), adValue);
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("[Admob] AOA impression recorded.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("[Admob] AOA clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            IsShowingAds = true;
            Debug.Log("[Admob] AOA opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            IsShowingAds = false;
            Debug.Log("[Admob] AOA closed. Loading next.");
            // Sau khi đóng phải load ad mới để lần sau show tiếp
            appOpenAd.Destroy();
            appOpenAd = null;
            IsReady = false;
            LoadAOA();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            IsShowingAds = false;
            Debug.LogError("[Admob] AOA open failed: " + error);
            // Nếu fail mở thì cũng nên hủy và load mới
            appOpenAd?.Destroy();
            appOpenAd = null;
            IsReady = false;
            LoadAOA();
        };

      
    }

    /// <summary>
    /// Gọi hàm này ở thời điểm bạn muốn hiện AOA (ví dụ splash xong, hoặc resume game).
    /// </summary>
    public bool TryShowAOAIfAvailable()
    {
        if (IsShowingAds)
        {
            Debug.Log("[Admob] AOA is already showing.");
            return false;
        }

        if (Time.unscaledTime - lastShowTime < MinShowInterval)
        {
            Debug.Log("[Admob] Throttle show AOA.");
            return false;
        }

        if (appOpenAd != null)
        {
            Debug.Log("[Admob] Showing AOA.");
            lastShowTime = Time.unscaledTime;
            appOpenAd.Show();
            return true;
        }

        Debug.Log("[Admob] AOA not ready, loading...");
        LoadAOA();
        return false;
    }

    /// <summary>
    /// Tự động gọi khi app chuyển trạng thái (background/foreground).
    /// </summary>
    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("[Admob] AppState: " + state);

        // Khi quay về foreground (khi app mở lại), thử show
        if (state == AppState.Foreground)
        {
            // Tùy chiến lược: có thể thêm điều kiện (ví dụ đã qua màn splash, v.v.)
            TryShowAOAIfAvailable();
        }
    }

    private void OnAdRevenuePaidEvent(string adUnitId, string type, ResponseInfo info, AdValue value)
    {
        Debug.Log($"[Admob] Revenue {type}");
        var impressionParameters = new[]
        {
            new Firebase.Analytics.Parameter("ad_platform", "Admob"),
            new Firebase.Analytics.Parameter("ad_source", "Google Admob"),
            new Firebase.Analytics.Parameter("ad_unit_name", adUnitId),
            new Firebase.Analytics.Parameter("ad_format", type),
            new Firebase.Analytics.Parameter("value", value.Value / 1_000_000d),
            new Firebase.Analytics.Parameter("currency", "USD"),
        };

        // FirebaseAnalytics.LogEvent("ad_impression", impressionParameters); // nếu muốn

        var parameters = new Dictionary<string, string>
        {
            { "ad_platform", "Admob" },
            { "ad_source", "Google Admob" },
            { "ad_unit_name", adUnitId },
            { "ad_format", type },
            { "value", $"{value.Value / 1_000_000d}" },
            { "currency", "USD" }
        };
        // Gửi AppsFlyer/Adjust nếu bạn có tích hợp.
    }
}
