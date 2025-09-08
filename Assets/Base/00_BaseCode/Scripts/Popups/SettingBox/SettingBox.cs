using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Sirenix.OdinInspector;
public class SettingBox : BaseBox
{
    #region instance
    public static SettingBox instance;
    public static SettingBox Setup(bool isOffButton, bool isSaveBox = false, Action actionOpenBoxSave = null)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<SettingBox>(PathPrefabs.SETTING_BOX));
            instance.Init();
        }

        instance.InitState(isOffButton);
        return instance;
    }
    #endregion
    #region Var

    [SerializeField] private Button btnClose;
  

    [SerializeField] private Button btnVibration;
    [SerializeField] private Button btnMusic;
    [SerializeField] private Button btnSound;

    public RectTransform objMusic;
    public RectTransform objVibra;
    public RectTransform objSound;
    public Image imgMusic;
    public Image imgVibration;
    public Image imgSound;

    public Sprite spriteOn;
    public Sprite spriteOff;

 
    public Button btnHome;
   // public Button btnRestart;

    public bool isGameplay;

    public Vector3 postOn = new Vector3(90, -36, 0);
    public Vector3 postOff = new Vector3(35, -36, 0);

    #endregion
    private void Init()
    {
        btnClose.onClick.AddListener(delegate { OnClickButtonClose(); }); 
        btnVibration.onClick.AddListener(delegate { OnClickBtnVibration(); });
        btnMusic.onClick.AddListener(delegate { OnClickBtnMusic(); });
        btnSound.onClick.AddListener(delegate { OnClickBtnSound(); });
        
  
        btnHome.onClick.AddListener(delegate { HandleBtnHome(); });
      //  btnRestart.onClick.AddListener(delegate { HandleBtnRestart(); });
       
  
    }
  
    private void InitState(bool param)
    {
        isGameplay = param;
        if (param)
        {
            
            btnHome.gameObject.SetActive(true);
        //    btnRestart.gameObject.SetActive(true);
       
        }    
        else
        {
         
            btnHome.gameObject.SetActive(false);
         //   btnRestart.gameObject.SetActive(false);
        }    
    
        SetUpBtn();
       
    }

    public void OffBtn()
    {
        btnHome.gameObject.SetActive(false);
     //   btnRestart.gameObject.SetActive(false);
    }    
    private void SetUpBtn()
    {
        if (GameController.Instance.useProfile.OnVibration)
        {
            imgVibration.sprite = spriteOn;
            objVibra.anchoredPosition = postOn;
        }
        else
        {
            imgVibration.sprite = spriteOff;
            objVibra.anchoredPosition = postOff;
        }

        if (GameController.Instance.useProfile.OnMusic)
        {
            imgMusic.sprite = spriteOn;
            objMusic.anchoredPosition = postOn;
        }
        else
        {
            imgMusic.sprite = spriteOff;
            objMusic.anchoredPosition = postOff;
        }

        if (GameController.Instance.useProfile.OnSound)
        {
            objSound.anchoredPosition = postOn;
            imgSound.sprite = spriteOn;
        }
        else
        {
            objSound.anchoredPosition = postOff;
            imgSound.sprite = spriteOff;
        }
      
    }

  
    private void OnClickBtnVibration()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (GameController.Instance.useProfile.OnVibration)
        {
            GameController.Instance.useProfile.OnVibration = false;
        }
        else
        {
            GameController.Instance.useProfile.OnVibration = true;
        }
        SetUpBtn();
    }

    private void OnClickBtnMusic()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (GameController.Instance.useProfile.OnMusic)
        {
            GameController.Instance.useProfile.OnMusic = false;
        }
        else
        {
            GameController.Instance.useProfile.OnMusic = true;
        }
        SetUpBtn();
    }
    private void OnClickBtnSound()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (GameController.Instance.useProfile.OnSound)
        {
            GameController.Instance.useProfile.OnSound = false;
        }
        else
        {
            GameController.Instance.useProfile.OnSound = true;
        }
        SetUpBtn();
    }


    private void OnClickButtonClose()
    {
        GameController.Instance.musicManager.PlayClickSound();
        GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "BtnCloseSettingBox");

        void Next()
        {
            if(isGameplay)
            {
      
             
            }    
        
            Close();

        }
  
    }


    public void HandleBtnHome()
    {

        //GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "BtnBackHomeSettingBox");

        //void Next()
        //{

        //    Close();
        //    Initiate.Fade("HomeScene", Color.black, 1.5f);

        //}
        GameController.Instance.musicManager.PlayClickSound();
        Close();



    }
    public void HandleBtnRestart()
    {
        //GameController.Instance.admobAds.ShowInterstitial(false, actionIniterClose: () => { Next(); }, actionWatchLog: "Restart");
        //void Next()
        //{
        //    Close();
        //    Initiate.Fade("GamePlay", Color.black, 1.5f);
        //}
        //Close();
        GameController.Instance.musicManager.PlayClickSound();
        BackHomeBox.Setup(TypeBackHOme.ResetLevel).Show();


    }
    private void OnClickRestorePurchase()
    {
        GameController.Instance.iapController.RestorePurchases();
    }    

}
