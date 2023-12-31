﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Advertisements;

public class RewardManager_Scr : MonoBehaviour
{
    public static RewardManager_Scr instance;

    public GameObject go_RespawnManager;
    public GameObject G_Reward; // 리워드창
    public GameObject g_CurLevelClear;
    public GameObject g_PreLevelClear;

    int tmp_Gem;
    int tmp_Coin;
     
    #region UI부분
    [Header("승리")]
    public GameObject G_Win;
    public TextMeshProUGUI t_WinStage;
    public TextMeshProUGUI t_CurGoldText;
    public TextMeshProUGUI t_PreGoldText;
    public TextMeshProUGUI t_CashText;
    public TextMeshProUGUI t_ADCashText;
    public Image i_ManImage;
    public TextMeshProUGUI t_AddManText;
    public Image i_WinbojImage;
    public Sprite[] i_winSprite;

    [Header("패배")]
    public GameObject G_Lose;
    public Image i_LosebojImage;
    public Sprite[] i_loseSprite;
    public TextMeshProUGUI t_LoseStage;
    #endregion

    [Header("버튼 막기용")]
    public GameObject G_ButtonShield;

    public int i_ADCash;    // 광고 클릭 시 획득 캐쉬
    public bool b_isWin;
    public bool b_isEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            return;
    }

    void Update()
    {
        if (RespawnManager.instance.b_BossSkillTime)
            G_ButtonShield.SetActive(true);
    }

    public void ChangeWinLoseBG(bool random, int Kinds)
    {
        if (random)
        {
            int tmp_reesourceNum = 0;
            if (TranslateManager_Scr_CI.instance.s_Language != "Korean")
            {
                tmp_reesourceNum = Random.Range(1, 4);
            }
            else
            {
                tmp_reesourceNum = Random.Range(0, 4);
            }
            i_WinbojImage.sprite = i_winSprite[tmp_reesourceNum];
            i_LosebojImage.sprite = i_loseSprite[tmp_reesourceNum];
        }
        else
        {
            i_WinbojImage.sprite = i_winSprite[Kinds];
            i_LosebojImage.sprite = i_loseSprite[Kinds];
        }
    }

    public void StartShieldOff()
    {
        Invoke("ShieldOff", 0.1f);
    }

    public void ShieldOff()
    {
        if (AttendanceManager_Scr.instance.b_DontSee)
        {
            if (G_ButtonShield.activeSelf)
                G_ButtonShield.SetActive(false);

            /*if (ADMobManager_Scr.instance != null && !ADMobManager_Scr.instance.b_isPremiumMode && !ADMobManager_Scr.instance.b_isNativeLoad)
                ADMobManager_Scr.instance.ShowNativeAD();
            else if (ADMobManager_Scr.instance != null && ADMobManager_Scr.instance.b_isPremiumMode)
                ADMobManager_Scr.instance.HideNativeAD();*/
        }
    }

    public void RewardWindowOn()
    {
        if (b_isEnd)
        {
            StageManager_Scr.instance.myAbility = 0;
            if (!G_Win.activeSelf && !G_Lose.activeSelf)
            {
                HPManager_Scr.instance.g_BloodScreen.GetComponent<Animator>().enabled = false;
                HPManager_Scr.instance.g_BloodScreen.GetComponent<Image>().color = new Vector4(255, 255, 255, 0);
                System.GC.Collect();    // 스테이지가 끝나고 결과창이 나올때 가비지 컬렉터를 수행하도록 설정
                Player_Input.instance.g_ButtonShield.SetActive(false);
                Player_Input.instance.resetRound();
                b_isWin = true;
                if (HPManager_Scr.instance.f_Hp <= 0)
                {
                    if (RespawnManager.instance.b_NomalGame && MissionManager_Scr.instance.b_isNoDie)
                        MissionManager_Scr.instance.i_ClearMissionNum[2] = 0;

                    if (RespawnManager.instance.b_NomalGame)
                    {
                        if (TranslateManager_Scr.instance.b_isEnglish || TranslateManager_Scr.instance.b_isIndia || TranslateManager_Scr.instance.b_isItalia || TranslateManager_Scr.instance.b_isGermany
                            || TranslateManager_Scr.instance.b_isFrance || TranslateManager_Scr.instance.b_isVietnam)
                            t_LoseStage.text = TranslateManager_Scr.instance.TranslateData[159]["English"] + StageManager_Scr.instance.i_StageLevel.ToString();
                        else
                            t_LoseStage.text = TranslateManager_Scr.instance.TranslateData[159][TranslateManager_Scr.instance.s_Language] + StageManager_Scr.instance.i_StageLevel.ToString();
                    }
                    else
                        t_LoseStage.text = "RankPoint " + ExpManager_Scr.instance.f_RankPoint.ToString();

                    // 패배창 활성화 
                    
                    G_Lose.SetActive(true);
                    b_isWin = false;

                }
                else
                {
                    t_CashText.text = string.Concat("+ ", (RespawnManager.instance.RespawnData[StageManager_Scr.instance.i_StageLevel - 1]["Gem_Reward"]).ToString());
                    if (TranslateManager_Scr.instance.b_isEnglish || TranslateManager_Scr.instance.b_isIndia || TranslateManager_Scr.instance.b_isItalia || TranslateManager_Scr.instance.b_isGermany
                        || TranslateManager_Scr.instance.b_isFrance || TranslateManager_Scr.instance.b_isVietnam)
                        t_WinStage.text = string.Concat(TranslateManager_Scr.instance.TranslateData[159]["English"].ToString(), (StageManager_Scr.instance.i_StageLevel).ToString());
                    else
                        t_WinStage.text = string.Concat(TranslateManager_Scr.instance.TranslateData[159][TranslateManager_Scr.instance.s_Language].ToString(), (StageManager_Scr.instance.i_StageLevel).ToString());
                    // 승리창 활성화

                    G_Win.SetActive(true);
                    SoundManager_sfx.instance.PlaySE("Game Complete", false);
                    // 승리 스테이지에 따른 캐쉬 획득
                    if (StageManager_Scr.instance.i_StageLevel >= StageManager_Scr.instance.i_StageClearLevel)
                    {
                        if (StageManager_Scr.instance.i_StageClearLevel > 1)
                            MissionManager_Scr.instance.i_ClearMissionNum[2]++;
                        // 이미 클리어한 스테이지에서는 보상을 획득하지 못하도록 막음
                        t_CurGoldText.text = string.Concat("+ ", (RespawnManager.instance.RespawnData[StageManager_Scr.instance.i_StageLevel - 1]["Coin_Reward"]).ToString());
                        g_CurLevelClear.SetActive(true);
                        g_PreLevelClear.SetActive(false);
                        tmp_Gem = (int)RespawnManager.instance.RespawnData[StageManager_Scr.instance.i_StageLevel - 1]["Gem_Reward"];
                        tmp_Coin = (int)RespawnManager.instance.RespawnData[StageManager_Scr.instance.i_StageLevel - 1]["Coin_Reward"];
                        GoldManager_Scr.instance.i_Gem += tmp_Gem;
                        GoldManager_Scr.instance.PlusGold(tmp_Coin);

                        if (GoogleManager_Scr.instance != null && GoogleManager_Scr.instance.b_isLogin)
                            StageClearAchievement();

#if UNITY_ANDROID
                        if (GoogleManager_Scr.instance.b_isLogin && !ReViewManager_Scr.instance.b_isWritten && StageManager_Scr.instance.i_StageLevel % 30 == 0)
                        {
                            Button_Option.instance.ReviewOn();
                        }
#elif UNITY_IOS 
                        if (AppleManager_Scr.instance.b_isLogin && !ReViewManager_Scr.instance.b_isWritten && StageManager_Scr.instance.i_StageLevel % 30 == 0)
                        {
                            Button_Option.instance.ReviewOn();
                        }
#endif


                        if (StageManager_Scr.instance.i_StageLevel < 1000)
                        {

                            StageManager_Scr.instance.i_StageLevel++;
                        }
                        StageManager_Scr.instance.i_StageClearLevel = StageManager_Scr.instance.i_StageLevel;
                    }
                    else
                    {
                        // 이미 클리어한 스테이지에서는 해당 스테이지의 50%의 코인만을 획득할 수 있게 설정
                        double tmpCoin = System.Convert.ToDouble(RespawnManager.instance.RespawnData[StageManager_Scr.instance.i_StageLevel - 1]["Coin_Reward"]) * 0.5f;
                        t_PreGoldText.text = string.Concat("+ ", ((int)tmpCoin).ToString());
                        g_CurLevelClear.SetActive(false);
                        g_PreLevelClear.SetActive(true);
                        tmp_Coin = (int)tmpCoin;
                        GoldManager_Scr.instance.PlusGold(tmp_Coin);
                    }
                    StageManager_Scr.instance.BGChange();
                }
                Button_Option.instance.OnClickAfterPauseHomeOn();
                Button_Option.instance.b_isStart = false;
                Button_Option.instance.G_After.SetActive(false);
            }
        }
    }

    public void StageClearAchievement()
    {
#if UNITY_ANDROID
        if (StageManager_Scr.instance.i_StageLevel == 10)
        {
            GoogleManager_Scr.instance.GetAchievementStage10();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 50)
        {
            GoogleManager_Scr.instance.GetAchievementStage50();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 100)
        {
            GoogleManager_Scr.instance.GetAchievementStage100();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 200)
        {
            GoogleManager_Scr.instance.GetAchievementStage200();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 300)
        {
            GoogleManager_Scr.instance.GetAchievementStage300();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 400)
        {
            GoogleManager_Scr.instance.GetAchievementStage400();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 500)
        {
            GoogleManager_Scr.instance.GetAchievementStage500();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 600)
        {
            GoogleManager_Scr.instance.GetAchievementStage600();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 700)
        {
            GoogleManager_Scr.instance.GetAchievementStage700();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 800)
        {
            GoogleManager_Scr.instance.GetAchievementStage800();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 900)
        {
            GoogleManager_Scr.instance.GetAchievementStage900();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 1000)
        {
            GoogleManager_Scr.instance.GetAchievementStageALL();
        }

#elif UNITY_IOS

        if (StageManager_Scr.instance.i_StageLevel == 10)
        {
            AppleManager_Scr.instance.GetAchievementStage10();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 50)
        {
            AppleManager_Scr.instance.GetAchievementStage50();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 100)
        {
            AppleManager_Scr.instance.GetAchievementStage100();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 200)
        {
            AppleManager_Scr.instance.GetAchievementStage200();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 300)
        {
            AppleManager_Scr.instance.GetAchievementStage300();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 400)
        {
            AppleManager_Scr.instance.GetAchievementStage400();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 500)
        {
            AppleManager_Scr.instance.GetAchievementStage500();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 600)
        {
            AppleManager_Scr.instance.GetAchievementStage600();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 700)
        {
            AppleManager_Scr.instance.GetAchievementStage700();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 800)
        {
            AppleManager_Scr.instance.GetAchievementStage800();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 900)
        {
            AppleManager_Scr.instance.GetAchievementStage900();
        }
        else if (StageManager_Scr.instance.i_StageLevel == 1000)
        {
            AppleManager_Scr.instance.GetAchievementStageALL();
        }

#endif
    }

    public void OnClickClosePanel()
    {
        if (b_isEnd)
        {
            if (!b_isWin)
            {
                if (ADMobManager_Scr.instance != null && !ADMobManager_Scr.instance.b_isPremiumMode)
                    ADMobManager_Scr.instance.ShowFrontAd();
            }

            b_isEnd = false;
        }
        // 라운드 시작 시 아래쪽 사이드바 투명하지 않게 만들도록 설정
        for (int i = 1; i < Button_Option.instance.g_DownSideGroup.transform.childCount; ++i)
        {
            GameObject obj = Button_Option.instance.g_DownSideGroup.transform.GetChild(i).gameObject;
            obj.GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
            for (int j = 0; j < obj.transform.childCount; ++j)
            {
                GameObject obj2 = obj.transform.GetChild(j).gameObject;
                if (obj2.GetComponent<TextMeshProUGUI>())
                {
                    obj2.GetComponent<TextMeshProUGUI>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
                }

                if (obj2.gameObject.name == "NewManCount")
                {
                    obj2.transform.GetChild(0).GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
                    obj2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
                }
            }
        }
        Button_Option.instance.AfterUI_Downside.transform.GetChild(1).GetComponent<Image>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);
        Button_Option.instance.AfterUI_Downside.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(255 / 255, 255 / 255, 255 / 255, 255 / 255);

        HPManager_Scr.instance.f_Hp = HPManager_Scr.instance.f_FullHp;
        G_ButtonShield.SetActive(false);
        //G_Reward.SetActive(false);
        if (!Button_Option.instance.b_InGame)
        {
            Button_Option.instance.G_After.SetActive(true);
            Button_Option.instance.G_Before.SetActive(true);
        }
        SoundManager_sfx.instance.sfxPlayer[SoundManager_sfx.instance.i_beStopsfxPlayerNum].Stop();
        G_Win.SetActive(false);
        G_Lose.SetActive(false);
    }
}
