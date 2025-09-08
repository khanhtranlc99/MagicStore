using Crystal;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateGame
{
    Loading = 0,
    Playing = 1,
    Win = 2,
    Lose = 3,
    Pause = 4
}

public class GamePlayController : Singleton<GamePlayController>
{
    public StateGame stateGame;
    public PlayerContain playerContain;
    public GameScene gameScene;
    public TutorialFunController tutorial_Level_1;
 
 
    
    protected override void OnAwake()
    {
        //  GameController.Instance.currentScene = SceneType.GamePlay;

     
        Init();

    }

    public void Init()
    {


        playerContain.Init(delegate
        {
            gameScene.Init(playerContain.levelData);
            tutorial_Level_1.Init();
            tutorial_Level_1.StartTut();
        });
      
        UseProfile.FirstLoading = true;
       

  



    }


   
}
