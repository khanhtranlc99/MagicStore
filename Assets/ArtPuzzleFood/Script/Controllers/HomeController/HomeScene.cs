using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class HomeScene : BaseScene
{

    public Button btnSetting;
 
 
    public void ShowGift()
    {
        

    }
    public int NumberPage(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.ShopButton:
                return 0;
                break;

            case ButtonType.HomeButton:
                return 1;
                break;

            case ButtonType.RankButton:
                return 2;
                break;

        }
        return 0;
    }


    public void Init()
    {
   
   
      
    
        btnSetting.onClick.AddListener(delegate { GameController.Instance.musicManager.PlayClickSound(); OnSettingClick(); });

  

        
       
   
    }
    //private void Update()
    //{

    //       // OnScreenChange();


    //}





    public override void OnEscapeWhenStackBoxEmpty()
    {
        //Hiển thị popup bạn có muốn thoát game ko?
    }
    private void OnSettingClick()
    {
        SettingBox.Setup(true).Show();
        //MMVibrationManager.Haptic(HapticTypes.MediumImpact);
    }

    


}
