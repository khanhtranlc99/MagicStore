using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.Field;

public class GameScene : BaseScene
{
    
    public Text tvLevel;
    public Button settinBtn;
    public Button nextBtn;
    public Button homeBtn;
    public Transform canvas;
    public GameObject blockRaycast;
    public Button btnRemoveAds;
    public BarPercent barPercent;
    public CanvasGroup canvasGroupMain;
    public int count = 1;

    public void Init( LevelData param )
    {
        nextBtn.onClick.AddListener(HandleButtonNext);
        nextBtn.gameObject.transform.localScale = Vector3.zero;
        nextBtn.gameObject.SetActive(false);
     
   
        settinBtn.onClick.AddListener(HandleBtnSetting);


       
      
        barPercent.Init(param);
        homeBtn.onClick.AddListener(HandleButtonOnClick);

        btnRemoveAds.onClick.AddListener(ButtonRemoveAds);
        count = 1;

    }
    public void ButtonRemoveAds()
    {
        GameController.Instance.musicManager.PlayClickSound();
        GameController.Instance.iapController.BuyProduct(TypePackIAP.RemoveAds);
    }



    private void HandleBtnSetting()
    {
        Debug.LogError("HandleBtnSetting");
        GameController.Instance.musicManager.PlayClickSound();
        SettingBox.Setup(true).Show();
    }
    public void HandleButtonOnClick()
    {
        Debug.LogError("HandleButtonOnClick");
        GameController.Instance.musicManager.PlayClickSound();
        Initiate.Fade(SceneName.HOME_SCENE, Color.black, 2f);
    }
    public void HandleButtonNext()
    {
        if(count > 0)
        {
            count--;
            GameController.Instance.musicManager.PlayClickSound();
            GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "InterWinBox");
            void Next()
            {

                var temp = JsonConvert.DeserializeObject<List<int>>(UseProfile.ListSave);

                if (temp == null)
                {
                    var Newdata = new List<int>() { 1, 2 };
                    UseProfile.ListSave = JsonConvert.SerializeObject(Newdata);
                }
                else
                {
                    temp.Add(UseProfile.LevelEggChest + 1);
                    UseProfile.ListSave = JsonConvert.SerializeObject(temp);
                }
                UseProfile.LevelEggChest += 1;
                Initiate.Fade(SceneName.GAME_PLAY, Color.black, 2f);
            }

        }


    }
    public IEnumerator WaitFadeCanvas( )
    {

        yield return canvasGroupMain.DOFade(0,0.85f).WaitForCompletion();
         
    }
    public void HandleShowButton()
    {
        nextBtn.gameObject.SetActive(true);
        nextBtn.gameObject.transform.DOScale(Vector3.one,0.5f);
        GameController.Instance.musicManager.PlayWinSound();
    }
    IEnumerator ChangeScene()
    {
         


        string name = "";

        name = SceneName.HOME_SCENE;
        var _asyncOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);

        while (!_asyncOperation.isDone)
        {

            yield return null;


        }
    }
    public override void OnEscapeWhenStackBoxEmpty()
    {
     
    }
}
