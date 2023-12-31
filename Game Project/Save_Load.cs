﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using Firebase;
using Firebase.Storage;
using UnityEngine.Networking;
using System.Globalization;

[System.Serializable]
public class canStat
{
    public int i_Level; 
    public int i_BuyPrice;
    public int i_UpgradePrice;
    public float f_NowAtk;
    public float f_NextAtk;
    public float f_AtkIncrease;
    public int i_NextUpgradePrice;

    public canStat(int _Level, int _buyPrice, int _upgradePrice, float _nowAtk, float _nextAtk, float _atkIncrease, int _nextUpgrade_Price)
    {
        i_Level = _Level;
        i_BuyPrice = _buyPrice; i_UpgradePrice = _upgradePrice;
        f_NowAtk = _nowAtk; f_NextAtk = _nextAtk;
        f_AtkIncrease = _atkIncrease; i_NextUpgradePrice = _nextUpgrade_Price;
    }
}

[System.Serializable]
public class saveData
{
    public string d_StartDay;
    public string d_EndDay;
    public bool b_isStarted = false;
    public int i_Level;
    public int i_Coin;
    public int i_Gem;
    public int i_Stage;
    public int i_NowLevelCount;
    public int i_PreLevelCount;
    public int i_CharType;
    public int i_ManCount;
    public int i_GetCount;  // 보상형 광고 남은 시청 수
    public int i_InventoryLevel;
    public int i_DeliveryLevel;
    public int i_DeliveryCount;

    public int i_LastSelectedNum;
    public int i_VendingMachineNum; // 구매 수량

    public int i_LangueValue;
    public string s_Language;
    public int i_LanguageIdx;

    public int[] i_nowUpgradeSteps; // 캐릭터 업글 횟수

    public float f_Exp;
    public float f_MaxExp;

    public bool b_BGOnOff;
    public bool b_EftOnOff;
    public bool b_isPremiumMode;    // 광고 제거 변수

    public int i_RankingPoint;   // 랭킹포인트

    // 인벤토리용 변수들
    public bool[] b_itemEmpty = new bool[80];
    public int[] i_ItemNum = new int[80];

    // 튜토리얼용 변수들
    public bool b_FirstPlaying;
    public bool b_isManPowerTutorial;
    public int i_TutorialCount;
    public bool b_CompletBoxTutorial;
    public bool b_CompletBagTutorial;
    public bool b_CompletSynthesisTutorial;

    public bool[] b_MachineOn = new bool[6];
    public bool[] b_ManPowerOn = new bool[1000];
    public bool[] b_CanOn = new bool[72];
    public bool[] b_CharacterBought = new bool[100];
    public bool[] b_CharacterSelected = new bool[100];

    public List<GameObject> g_Heroobj;
    public List<int> i_HeroType;

    public string c_Stats;  // 캔 스텟을 받아오는 스트링 구절
    public string m_Stats;  // 인력 스텟을 받아오는 스트링 구절

    // 출석 관련 변수
    public int i_LastAccessDay;
    public bool[] b_Absents;
    public bool[] b_Attendances;
    public bool[] b_GetRewards;
    public bool b_GetGem;
    public bool b_DontSee;

    public string[] d_Days; // 출석 슬롯의 저장값을 받아오는 스트링 구절

    // 미션 관련 변수
    public int[] i_MissionSlot;
    public int[] i_TargetMission;
    public int[] i_ClearMissionNum;
    public bool[] b_MissionSuccess;
    public bool[] b_GetReward;
    public bool[] b_GetMission;
    public bool b_isNoQuit;
    public bool b_isNoHit;
    public bool b_isNoDie;

    // 최초 달성 업적 체크포인트
    public bool[] b_isGetAchievment;

    // 이름들의 목록을 받아오는 스트링 구절
    public string s_JuimName;
    public string s_DealiName;
    public string s_GwajangName;
    public string s_BujangName;
    public string s_IsaName;
    public string s_SangmuName;
    public string s_JeonmuName;
    public string s_SajangName;


}

public class Save_Load : MonoBehaviour
{
    public static Save_Load instance;

    public saveData s_Data;
    public bool b_isTestReset = false;  // 테스트모드 리셋 설정시

    [SerializeField]
    private int[] i_StartStat = new int[5];
    public int i_StageIdx;

    public bool b_isNotFile = false;
    public bool b_FildLoad = false;    // 파일 로드
    public bool b_isLoad = false;      // 캐릭 스텟 로드
    bool b_MachineLoad = false; // 자판기 로드 

    public TextAsset ManTxt;

    string[] split_Man;       // 인력 스텟 받아오는 스플릿
    float[] f_M_Stats = new float[10000]; // 인력 스텟 받기

    string[] split_Can;       // 음료수 스텟 받아오는 스플릿
    float[] f_c_Stats = new float[1000]; // 음료수 스텟 받기


    string[] split_string;
    // 이름들 목록 받아오기
    string[] s_Juim = new string[100];
    string[] s_Deali = new string[100];
    string[] s_Gwajang = new string[100];
    string[] s_Bujang = new string[100];
    string[] s_Isa = new string[100];
    string[] s_Sangmu = new string[100];
    string[] s_Jeonmu = new string[100];
    string[] s_Sajang = new string[100];

    bool[,] b_Bought = new bool[6, 12]; // 음료수 샀냐?

    public string s_Path;
    string s_ManStat;
    string[] s_ManSplit;


    FirebaseStorage storage;
    StorageReference reference;

    private void Awake()
    {
        if (instance != null)
            return;
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // 이걸 이메일 형식으로 고치고 테스트 ㄱㄱ
        //s_Path = Path.Combine(Application.persistentDataPath + "/saveData.json");

        for (int i = 0; i < 10000; ++i)
        {
            f_M_Stats[i] = -1;
        }

        for (int i = 0; i < 1000; ++i)
        {
            f_c_Stats[i] = -1;
        }

        for (int i = 0; i < 100; ++i)
        {
            s_Juim[i] = null;
            s_Deali[i] = null;
            s_Gwajang[i] = null;
            s_Bujang[i] = null;
            s_Isa[i] = null;
            s_Sangmu[i] = null;
            s_Jeonmu[i] = null;
            s_Sajang[i] = null;
        }


        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j < 12; ++j)
                b_Bought[i, j] = false;
        }

    }

    public void Dataupload(string bytedata)
    {
        storage = FirebaseStorage.DefaultInstance;

#if UNITY_ANDROID

        string mail = GoogleManager_Scr.instance.loginMail;

#elif UNITY_IOS

        string mail = AppleManager_Scr.instance.loginMail;
                    Debug.Log("IDisthis = " + mail);

#endif

#if UNITY_ANDROID

        byte[] bytes = null;

        // 에디터일 경우
        if (GoogleManager_Scr.instance.loginMail == "")
        {
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_editor.json");

            bytes = Encoding.UTF8.GetBytes(bytedata);
        }
        // 로그인하여 Email을 변수로 가지고 있는 경우
        else
        {
            //서버에 저장될 파일 위치 및 파일 이름 정하기
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_" + mail + ".json");

            bytes = Encoding.UTF8.GetBytes(bytedata);
        }

        // 파일 업로드하기
        reference.PutBytesAsync(bytes).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //성공, 실패 여부는 아직 판단 불가
                Debug.Log("File upload.");
            }
        });


#elif UNITY_IOS


        byte[] bytes = null;

        // 에디터일 경우
        if (AppleManager_Scr.instance.loginMail == "")
        {
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_editor.json");

            bytes = Encoding.UTF8.GetBytes(bytedata);
        }
        // 로그인하여 Email을 변수로 가지고 있는 경우
        else
        {
            //서버에 저장될 파일 위치 및 파일 이름 정하기
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_" + mail + ".json");

            bytes = Encoding.UTF8.GetBytes(bytedata);
        }

        // 파일 업로드하기
        reference.PutBytesAsync(bytes).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                //성공, 실패 여부는 아직 판단 불가
                Debug.Log("File upload.");
            }
        });

#endif
    }
    public void Datadownload()
    {
        storage = FirebaseStorage.DefaultInstance;

        string localfile = null;

#if UNITY_ANDROID
        // 에디터일 경우
        if (GoogleManager_Scr.instance.loginMail == "")
        {
            // 서버에 있는 해당 경로의 파일 가져오기
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_editor.json");

            // 가져올 파일의 새 이름 부여 및 경로 정하기
            localfile = Application.persistentDataPath + "/saveData_editor.json";

            reference.GetFileAsync(localfile).ContinueWith(task =>
            {
                //성공, 실패 여부는 아직 판단 불가
                if (task.IsCompleted)
                {
                    if (File.Exists(localfile))
                    {
                        //냉동 뉴비이다.
                        string code = File.ReadAllText(localfile, Encoding.UTF8);
                        LoadData(code);
                    }
                    else
                    {
                        b_FildLoad = false;
                        GoogleManager_Scr.instance.b_isNextScene = true;
                        //서버에도 저장파일이 없다 = 이 사람은 완전 뉴비이다.
                        Debug.Log("서버에도 파일 없음");
                    }
                }
            });
        }
        // 로그인하여 Email을 변수로 가지고 있는 경우
        else
        {
            // 서버에 있는 해당 경로의 파일 가져오기
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_" + GoogleManager_Scr.instance.loginMail + ".json");

            // 가져올 파일의 새 이름 부여 및 경로 정하기
            localfile = Application.persistentDataPath + "/saveData_" + GoogleManager_Scr.instance.loginMail + ".json";

            // 파일 다운로드 하기
            reference.GetFileAsync(localfile).ContinueWith(task =>
            {
                //성공, 실패 여부는 아직 판단 불가
                if (task.IsCompleted)
                {
                    if (File.Exists(localfile))
                    {
                        //냉동 뉴비이다.
                        string code = File.ReadAllText(localfile, Encoding.UTF8);
                        LoadData(code);
                    }
                    else
                    {
                        b_FildLoad = false;
                        GoogleManager_Scr.instance.b_isNextScene = true;
                        //서버에도 저장파일이 없다 = 이 사람은 완전 뉴비이다.
                        Debug.Log("서버에도 파일 없음");
                    }
                }
            });
        }

#elif UNITY_IOS

        // 에디터일 경우
        if (AppleManager_Scr.instance.loginMail == "")
        {
            Debug.Log("checkthis12");
            // 서버에 있는 해당 경로의 파일 가져오기
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_editor.json");

            // 가져올 파일의 새 이름 부여 및 경로 정하기
            localfile = Application.persistentDataPath + "/saveData_editor.json";

            reference.GetFileAsync(localfile).ContinueWith(task =>
            {
                //성공, 실패 여부는 아직 판단 불가
                if (task.IsCompleted)
                {
                    FileInfo info = new FileInfo(localfile);
                    if (info == null || info.Exists == false)
                    {
                        Debug.Log("checkthis88");
                        b_FildLoad = false;
                        AppleManager_Scr.instance.b_isNextScene = true;
                        //서버에도 저장파일이 없다 = 이 사람은 완전 뉴비이다.
                        Debug.Log("checkthis99");
                        Debug.Log("서버에도 파일 없음");
                    }
                    else if (info != null && info.Exists != false)
                    {
                        Debug.Log("checkthis66");
                        //냉동 뉴비이다.
                        string code = File.ReadAllText(localfile, Encoding.UTF8);
                        LoadData(code);
                        Debug.Log("checkthis77");
                    }
                    /*
                    if (File.Exists(localfile))
                    {
                        //냉동 뉴비이다.
                        string code = File.ReadAllText(localfile, Encoding.UTF8);
                        LoadData(code);
                    }
                    else
                    {
                        b_FildLoad = false;
                        AppleManager_Scr.instance.b_isNextScene = true;
                        //서버에도 저장파일이 없다 = 이 사람은 완전 뉴비이다.
                        Debug.Log("서버에도 파일 없음");
                    }*/
                }
            });
        }
        // 로그인하여 Email을 변수로 가지고 있는 경우
        else
        {
        // 서버에 있는 해당 경로의 파일 가져오기
            reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_" + AppleManager_Scr.instance.loginMail + ".json");
            // reference = storage.GetReferenceFromUrl("gs://company-strike-9881160.appspot.com/SLfile/saveData_otsNbpDU6MTDnhaeCaAB6BmakG42.json");

            // 가져올 파일의 새 이름 부여 및 경로 정하기
            //  localfile = Application.persistentDataPath + "/saveData_" + AppleManager_Scr.instance.loginMail + ".json";

            localfile = "file://" + Application.persistentDataPath + "/saveData_" + AppleManager_Scr.instance.loginMail + ".json";
            string Load_localfile = Path.Combine(Application.persistentDataPath + "/saveData_" + AppleManager_Scr.instance.loginMail + ".json");

            // 파일 다운로드 하기
            reference.GetFileAsync(localfile).ContinueWith(task =>
            {
                Debug.Log("checkthis4");
                //성공, 실패 여부는 아직 판단 불가
                if (task.IsCompleted)
                {
                    if (File.Exists(Load_localfile))
                    {
                        //냉동 뉴비이다.
                        string code = File.ReadAllText(Load_localfile, Encoding.UTF8);
                        LoadData(code);
                    }
                    else
                    {
                        b_FildLoad = false;
                        AppleManager_Scr.instance.b_isNextScene = true;
                        //서버에도 저장파일이 없다 = 이 사람은 완전 뉴비이다.
                        Debug.Log("서버에도 파일 없음");
                    }
                }
            });

        }

#endif

    }

    public void readytoDelay()
    {
        Invoke("StartingCoroutine", 1f);
    }

    void StartingCoroutine()
    {
        StartCoroutine(Update_COU());
    }

    IEnumerator Update_COU()
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (true)
        {
            // 불러오기 전
            if (!b_isLoad)
            {
                if (b_FildLoad && ManPowerManager_Scr.instance != null && GoldManager_Scr.instance != null && StageManager_Scr.instance != null
                    && Inventory_Scr.instance != null && DeliveryBoxManager_Scr.instance != null)
                {
                    if (s_Data.i_InventoryLevel != 0)
                    {
                        Inventory_Scr.instance.InventoryLevel = s_Data.i_InventoryLevel;
                    }

                    if(s_Data.i_DeliveryLevel != 0)
                    {
                        DeliveryBoxManager_Scr.instance.GetBoxCount = s_Data.i_DeliveryCount;
                        DeliveryBoxManager_Scr.instance.DeliveryLevel = s_Data.i_DeliveryLevel;
                    }

                    //  TutorialManager.instance.b_FirstPlaying = false;
                    TutorialManager.instance.b_isManPowerTutorial = s_Data.b_isManPowerTutorial;
                    ManPowerManager_Scr.instance.i_Level = s_Data.i_Level;
                    ManPowerManager_Scr.instance.i_preLevelCount = s_Data.i_PreLevelCount;
                    ManPowerManager_Scr.instance.i_LevelManCount = s_Data.i_NowLevelCount;
                    ChangeManager_Scr.instance.i_CharType = s_Data.i_CharType;
                    Player_Input.instance.ActiveCharater(ChangeManager_Scr.instance.i_CharType - 1);
                    GoldManager_Scr.instance.i_Coin = s_Data.i_Coin;
                    GoldManager_Scr.instance.i_Gem = s_Data.i_Gem;
                    if (s_Data.i_Stage >= 2)
                    {
                        StageManager_Scr.instance.i_StageLevel = s_Data.i_Stage;
                        StageManager_Scr.instance.i_StageClearLevel = s_Data.i_Stage;
                    }
                    ExpManager_Scr.instance.f_fullExp = s_Data.f_MaxExp;
                    ExpManager_Scr.instance.LoadExp(s_Data.f_Exp);
                    Button_Option.instance.setVolume_Background = s_Data.b_BGOnOff;
                    Button_Option.instance.setVolume_Effect = s_Data.b_EftOnOff;

                    if (s_Data.i_LastSelectedNum != -1)
                    {
                        MachineSelectManager_Scr.instance.i_LastSelected = s_Data.i_LastSelectedNum;
                    }
                    else
                        MachineSelectManager_Scr.instance.i_LastSelected = -1;

                    ManPowerManager_Scr.instance.b_IsLoaded = true;
                    if (!string.IsNullOrEmpty(s_Data.i_LangueValue.ToString()))
                    {
                        TranslateManager_Scr.instance.OnTranslateLanguage(s_Data.s_Language);
                        TranslateManager_Scr.instance.OnCheckImage(s_Data.i_LangueValue);
                    }
                    b_isLoad = true;
                }
                else if (!b_FildLoad && ManPowerManager_Scr.instance != null && GoldManager_Scr.instance != null && StageManager_Scr.instance != null && Inventory_Scr.instance != null)
                {
                    // 세이브파일 없이 최초로 시작할 때
                    Inventory_Scr.instance.InventoryLevel = 1;
                    TutorialManager.instance.b_FirstPlaying = true;
                    TutorialManager.instance.i_TutorialCount = 0;
                    ChangeManager_Scr.instance.InitData();
                    ManPowerManager_Scr.instance.i_Level = i_StartStat[0];
                    ManPowerManager_Scr.instance.initBall();
                    GoldManager_Scr.instance.i_Coin = i_StartStat[1];
                    GoldManager_Scr.instance.i_Gem = i_StartStat[2];
                    StageManager_Scr.instance.i_StageLevel = i_StartStat[3];
                    ExpManager_Scr.instance.f_nowExp = i_StartStat[4];
                    Button_Option.instance.setVolume_Background = true;
                    Button_Option.instance.setVolume_Effect = true;
                    if (ADMobManager_Scr.instance != null)
                        ADMobManager_Scr.instance.b_isPremiumMode = false;
                    s_Data.b_isStarted = false;
                    AttendanceManager_Scr.instance.b_isStarted = s_Data.b_isStarted;
                    for (int i = 0; i < AttendanceManager_Scr.instance.t_SlotParent.childCount - 1; ++i)
                    {
                        AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Absent = false;
                        AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Attendance = false;
                        AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_GetReward = false;
                    }
                    AttendanceManager_Scr.instance.b_DontSee = false;
                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(AttendanceManager_Scr.instance.t_SlotParent.transform.childCount - 1).GetComponent<AttendanceSlot_Scr>().LoadIcon();
                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(AttendanceManager_Scr.instance.t_SlotParent.transform.childCount - 1).GetComponent<AttendanceSlot_Scr>().b_GetReward = false;
                    AttendanceManager_Scr.instance.i_LastAccessDay = DateTimeOffset.UtcNow.LocalDateTime.Day;
                    AttendanceManager_Scr.instance.LoadAttendance();
                    MissionManager_Scr.instance.InitSlot();

                    StageManager_Scr.instance.t_InGameStageName.text = "로비";


                    StageManager_Scr.instance.LoadBG();
                    ManPowerManager_Scr.instance.b_IsLoaded = false;
                    b_isLoad = true;
                }
            }
            //if (SceneManager.GetActiveScene().name == "PlayScene")
            if (manageMemory.instance.Onplay)
            {
                int ch = 0;
                if (b_FildLoad && !b_MachineLoad && TranslateManager_Scr.instance != null)
                {
                    if (s_Data.i_InventoryLevel != 0 && Inventory_Scr.instance != null)
                    {
                        Inventory_Scr.instance.InventoryLevel = s_Data.i_InventoryLevel;
                        for(int i = 0; i < 80; ++i)
                        {
                            if (!s_Data.b_itemEmpty[i])
                            {
                                Inventory_Scr.instance.LoadItem(i, s_Data.i_ItemNum[i]);
                            }
                        }
                    }

                    if(s_Data.i_DeliveryLevel != 0 && DeliveryBoxManager_Scr.instance != null)
                    {
                        DeliveryBoxManager_Scr.instance.DeliveryLevel = s_Data.i_DeliveryLevel;
                        DeliveryBoxManager_Scr.instance.GetBoxCount = s_Data.i_DeliveryCount;
                    }

                    TranslateManager_Scr.instance.s_Language = s_Data.s_Language;
                    TranslateManager_Scr.instance.i_LanguageValue = s_Data.i_LangueValue;
                    TranslateManager_Scr.instance.TitleImage(s_Data.s_Language);
                    if (GoogleManager_Scr.instance != null && GoogleManager_Scr.instance.b_isLogin)
                    {
                        if (s_Data.i_RankingPoint != 0)
                        {
                            GoogleManager_Scr.instance.SetLeaderBoardScore(s_Data.i_RankingPoint);
                            s_Data.i_RankingPoint = 0;
                        }
                    }
                    else
                    {
                        s_Data.i_RankingPoint = 0;
                    }

                    TutorialManager.instance.b_FirstPlaying = s_Data.b_FirstPlaying;
                    if (!TutorialManager.instance.b_FirstPlaying)
                        Button_Option.instance.g_RankButton.SetActive(true);
                    TutorialManager.instance.i_TutorialCount = s_Data.i_TutorialCount;

                    TutorialManager.instance.b_BagTutorial = s_Data.b_CompletBagTutorial;
                    TutorialManager.instance.b_BoxTutorial = s_Data.b_CompletBoxTutorial; 
                    TutorialManager.instance.b_SynthesisTutorial = s_Data.b_CompletSynthesisTutorial;

                    if (ADMobManager_Scr.instance != null)
                        ADMobManager_Scr.instance.b_isPremiumMode = s_Data.b_isPremiumMode;
                    #region 이름 바꾸기
                    for (int i = 0; i < s_Juim.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Juim[i]))
                            ShopManager_Scr.instance.s_JuimName.Add(s_Juim[i]);
                    }

                    for (int i = 0; i < s_Deali.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Deali[i]))
                            ShopManager_Scr.instance.s_DealiName.Add(s_Deali[i]);
                    }

                    for (int i = 0; i < s_Gwajang.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Gwajang[i]))
                            ShopManager_Scr.instance.s_GwajangName.Add(s_Gwajang[i]);
                    }

                    for (int i = 0; i < s_Bujang.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Bujang[i]))
                            ShopManager_Scr.instance.s_BujangName.Add(s_Bujang[i]);
                    }

                    for (int i = 0; i < s_Isa.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Isa[i]))
                            ShopManager_Scr.instance.s_IsaName.Add(s_Isa[i]);
                    }

                    for (int i = 0; i < s_Jeonmu.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Jeonmu[i]))
                            ShopManager_Scr.instance.s_JeonmuName.Add(s_Jeonmu[i]);
                    }

                    for (int i = 0; i < s_Sangmu.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Sangmu[i]))
                            ShopManager_Scr.instance.s_SangmuName.Add(s_Sangmu[i]);
                    }

                    for (int i = 0; i < s_Sajang.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(s_Sajang[i]))
                            ShopManager_Scr.instance.s_SajangName.Add(s_Sajang[i]);
                    }
                    #endregion

                    ch = 0;
                    int tmp = -1;
                    if (s_Data.b_isGetAchievment != null)
                    {
                        for (int i = 0; i < ManPowerManager_Scr.instance.b_isGetAchievement.Length; ++i)
                        {
                            ManPowerManager_Scr.instance.b_isGetAchievement[i] = s_Data.b_isGetAchievment[i];
                        }
                    }
                    if (s_Data.i_ManCount > ManPowerManager_Scr.instance.t_ParentMans.childCount)
                        tmp = s_Data.i_ManCount - ManPowerManager_Scr.instance.t_ParentMans.childCount;

                    for (int i = 0; i < tmp; ++i)
                    {
                        GameObject obj = Instantiate(Resources.Load("Slot/Man"), ManPowerManager_Scr.instance.t_ParentMans.position, Quaternion.identity) as GameObject;
                        obj.transform.SetParent(ManPowerManager_Scr.instance.t_ParentMans);
                        obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        obj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.zero);
                    }
                    for (int i = 0; i < ManPowerManager_Scr.instance.t_ParentMans.childCount; ++i)
                    {
                        ManStat_Scr obj = ManPowerManager_Scr.instance.t_ParentMans.GetChild(i).transform.GetComponent<ManStat_Scr>();

                        obj.b_Bought = s_Data.b_ManPowerOn[i];

                        if (f_M_Stats[ch] != -1)
                        {
                            obj.i_Level = (int)f_M_Stats[ch++];
                            obj.f_Atk = f_M_Stats[ch++];
                            obj.i_Money = (int)f_M_Stats[ch++];
                            obj.i_TypeNum = (int)f_M_Stats[ch++];
                            obj.i_UpgradeMoney = (int)f_M_Stats[ch++];
                            obj.i_UpgradeNum = (int)f_M_Stats[ch++];


                            if (obj.b_Bought)
                            {
                                s_ManStat = ManPowerManager_Scr.instance.ManData[obj.i_UpgradeNum]["Name_Korean"].ToString();
                                s_ManSplit = s_ManStat.Split('_');
                                s_ManStat = s_ManSplit[0];
                                obj.s_Name = s_ManStat;

                                s_ManStat = ManPowerManager_Scr.instance.ManData[obj.i_UpgradeNum]["Texture"].ToString();
                                s_ManStat = s_ManStat.Substring(8);

                                if (int.Parse(s_ManStat) == 9)
                                {
                                    obj.t_NextPrice.text = obj.i_UpgradeMoney.ToString();
                                    obj.g_Upgrade.SetActive(false);
                                    obj.g_NextLevel.SetActive(true);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < s_Data.i_HeroType.Count; i++)
                    {
                        Inventory_Scr.instance.LoadHero(s_Data.i_HeroType[i]);
                    }

                    if (s_Data.b_CharacterBought != null && s_Data.b_CharacterSelected != null)
                    {
                        for (int i = 0; i < ChangeManager_Scr.instance.t_CharacterList.childCount; ++i)
                        {
                            GameObject obj = ChangeManager_Scr.instance.t_CharacterList.GetChild(i).gameObject;
                            obj.GetComponent<CharChangerStat_Scr>().b_isBought = s_Data.b_CharacterBought[i];
                            if (i == 0)
                            {
                                obj.GetComponent<CharChangerStat_Scr>().b_isBought = true;
                            }
                            obj.GetComponent<CharChangerStat_Scr>().b_isSelected = s_Data.b_CharacterSelected[i];
                            if (s_Data.i_nowUpgradeSteps != null && s_Data.i_nowUpgradeSteps.Length > 0)
                                obj.GetComponent<CharChangerStat_Scr>().i_nowUpgradeSteps = s_Data.i_nowUpgradeSteps[i];
                        }
                    }

                    StageManager_Scr.instance.LoadBG();

                    #region 미션
                    if (s_Data.b_GetMission != null)
                    {
                        MissionManager_Scr.instance.b_isNoQuit = s_Data.b_isNoQuit;
                        MissionManager_Scr.instance.b_isNoDie = s_Data.b_isNoDie;
                        MissionManager_Scr.instance.b_isNoHit = s_Data.b_isNoHit;
                        for (int i = 0; i < MissionManager_Scr.instance.G_MissionList.Length; ++i)
                        {
                            MissionSlot_Scr obj = MissionManager_Scr.instance.G_MissionList[i].GetComponent<MissionSlot_Scr>();
                            if (!obj.b_isPerfectReward)
                            {
                                if (s_Data.b_GetMission[i])
                                {
                                    obj.b_GetMission = s_Data.b_GetMission[i];
                                    MissionManager_Scr.instance.i_MissionSlot[i] = s_Data.i_MissionSlot[i];
                                    MissionManager_Scr.instance.i_TargetMission[i] = s_Data.i_TargetMission[i];
                                    MissionManager_Scr.instance.i_ClearMissionNum[i] = s_Data.i_ClearMissionNum[i];
                                    MissionManager_Scr.instance.b_MissionSuccess[i] = s_Data.b_MissionSuccess[i];
                                    MissionManager_Scr.instance.b_GetReward[i] = s_Data.b_GetReward[i];
                                }
                                else
                                    obj.b_GetMission = false;

                                print(s_Data.i_LastAccessDay + ", " + DateTimeOffset.UtcNow.LocalDateTime.Day);
                                if (s_Data.i_LastAccessDay == DateTimeOffset.UtcNow.LocalDateTime.Day)
                                    obj.LoadCoinMission(s_Data.i_MissionSlot[i]);
                            }
                            else
                            {
                                MissionManager_Scr.instance.b_GetReward[i] = s_Data.b_GetReward[i];


                                obj.LoadGemMission();
                            }
                        }
                    }
                    #endregion

                    #region 출석
                    if (s_Data.b_Absents != null)
                    {
                        AttendanceManager_Scr.instance.i_GetCount = s_Data.i_GetCount;
                        AttendanceManager_Scr.instance.b_DontSee = s_Data.b_DontSee;

                        if (!string.IsNullOrEmpty(s_Data.d_StartDay))
                        {
                            if (s_Data.d_StartDay.Substring(8, 2) == "0001")
                            {
                                AttendanceManager_Scr.instance.d_StartDay = DateTimeOffset.UtcNow;
                            }
                            else
                            {
                                Debug.Log(new DateTime(int.Parse(s_Data.d_StartDay.Substring(0, 4)), int.Parse(s_Data.d_StartDay.Substring(5, 2)), int.Parse(s_Data.d_StartDay.Substring(8, 2))));
                                AttendanceManager_Scr.instance.d_StartDay = new DateTime(int.Parse(s_Data.d_StartDay.Substring(0, 4)), int.Parse(s_Data.d_StartDay.Substring(5, 2)), int.Parse(s_Data.d_StartDay.Substring(8, 2)));
                                //  AttendanceManager_Scr.instance.d_StartDay = new DateTime(int.Parse(s_Data.d_StartDay.Substring(0, 4)), int.Parse(s_Data.d_StartDay.Substring(5, 2)), int.Parse(s_Data.d_StartDay.Substring(8, 2)));
                            }
                        }
                        if (!string.IsNullOrEmpty(s_Data.d_EndDay))
                        {
                            AttendanceManager_Scr.instance.d_EndDay = new DateTime(int.Parse(s_Data.d_EndDay.Substring(0, 4)), int.Parse(s_Data.d_EndDay.Substring(5, 2)), int.Parse(s_Data.d_EndDay.Substring(8, 2)));
                        }

                        AttendanceManager_Scr.instance.b_isStarted = s_Data.b_isStarted;

                        if (s_Data.d_Days != null)
                        {
                            for (int i = 0; i < AttendanceManager_Scr.instance.t_SlotParent.childCount; ++i)
                            {
                                if (i != AttendanceManager_Scr.instance.t_SlotParent.childCount - 1)
                                {
                                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().d_Day = new DateTime(int.Parse(s_Data.d_Days[i].Substring(0, 4)), int.Parse(s_Data.d_Days[i].Substring(5, 2)), int.Parse(s_Data.d_Days[i].Substring(8, 2)));
                                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Absent = s_Data.b_Absents[i];
                                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Attendance = s_Data.b_Attendances[i];
                                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_GetReward = s_Data.b_GetRewards[i];
                                    print(AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().d_Day);
                                }
                                else
                                {
                                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().LoadIcon();
                                    AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_GetReward = s_Data.b_GetGem;
                                }

                            }
                        }
                        AttendanceManager_Scr.instance.i_LastAccessDay = s_Data.i_LastAccessDay;

                    }
                    else
                    {
                        AttendanceManager_Scr.instance.b_isStarted = s_Data.b_isStarted;
                        //AttendanceManager_Scr.instance.b_DontSee = false;
                        for (int i = 0; i < AttendanceManager_Scr.instance.t_SlotParent.childCount - 1; ++i)
                        {
                            AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Absent = false;
                            AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Attendance = false;
                            AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_GetReward = false;
                        }
                        AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(AttendanceManager_Scr.instance.t_SlotParent.transform.childCount - 1).GetComponent<AttendanceSlot_Scr>().LoadIcon();
                        AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(AttendanceManager_Scr.instance.t_SlotParent.transform.childCount - 1).GetComponent<AttendanceSlot_Scr>().b_GetReward = false;
                    }
                    AttendanceManager_Scr.instance.LoadAttendance();
                    #endregion



                    // 자판기 구매 수량 로드
                    MachineSelectManager_Scr.instance.i_NowBoughtMachine = s_Data.i_VendingMachineNum;

                    for (int i = 0; i < s_Data.b_MachineOn.Length; ++i)
                    {
                        // 자판기를 샀는지 안샀는지의 여부. 가격도 저장. -> 세이브로드 오류뜨면 여기부분
                        MachineSelectManager_Scr.instance.OnLoadMachineOnOff(i, s_Data.b_MachineOn[i]);
                    }

                    if (s_Data.b_CanOn != null && s_Data.c_Stats != null)
                    {
                        ch = 0;
                        for (int i = 0; i < MachineSelectManager_Scr.instance.t_AllCanList.childCount; ++i)
                        {
                            for (int j = 0; j < MachineSelectManager_Scr.instance.t_AllCanList.GetChild(i).GetChild(0).GetChild(0).childCount; ++j)
                            {
                                // 음료수들의 정보 가져오기
                                GameObject obj = MachineSelectManager_Scr.instance.t_AllCanList.GetChild(i).GetChild(0).GetChild(0).GetChild(j).gameObject;
                                //if (b_Bought[i, j])
                                //  obj.SetActive(true);

                                obj.GetComponent<CanStat_Scr>().b_IsBought = b_Bought[i, j];

                                if (f_c_Stats[ch] != -1)
                                {
                                    obj.GetComponent<CanStat_Scr>().i_Level = (int)f_c_Stats[ch++];
                                    obj.GetComponent<CanStat_Scr>().i_BuyPrice = (int)f_c_Stats[ch++];
                                    obj.GetComponent<CanStat_Scr>().i_UpgradePrice = (int)f_c_Stats[ch++];
                                    obj.GetComponent<CanStat_Scr>().f_NowAtk = f_c_Stats[ch++];
                                    obj.GetComponent<CanStat_Scr>().f_NextAtk = f_c_Stats[ch++];
                                    obj.GetComponent<CanStat_Scr>().i_ListNum = (int)f_c_Stats[ch++];
                                    obj.GetComponent<CanStat_Scr>().i_CanNum = (int)f_c_Stats[ch++];
                                }
                                obj.GetComponent<CanStat_Scr>().OnLoadLaunchCanNum();

                            }
                        }
                    }


                    b_MachineLoad = true;
                    b_FildLoad = false;
                }
            }
            yield return delay;
        }
    }

    public void LoadData(string code)
    {
        /*
        string jsonData = File.ReadAllText(s_Path);
        s_Data = JsonUtility.FromJson<saveData>(jsonData);
        */

        // 실제 배포 -> File.Exists(s_Path), 테스트모드 -> File.Exists(s_TestPath)

        // Debug.Log("불러오기");

        if (!string.IsNullOrEmpty(code) && !string.IsNullOrWhiteSpace(code))
        {
            if (code.Substring(0, 1) == "{")
                s_Data = JsonUtility.FromJson<saveData>(code);
            else
            {
                byte[] bytes = System.Convert.FromBase64String(code);
                string jsonData = System.Text.Encoding.UTF8.GetString(bytes);
                s_Data = JsonUtility.FromJson<saveData>(jsonData);
            }

            // 인력 정보 가져오기
            if (s_Data.m_Stats != null)
                split_Man = s_Data.m_Stats.Split(',');
            for (int i = 0; i < split_Man.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_Man[i]))
                {
                    f_M_Stats[i] = Convert.ToSingle(split_Man[i]);
                }
            }

            // 음료수 정보 가져오기
            if (s_Data.c_Stats != null)
                split_Can = s_Data.c_Stats.Split(',');
            for (int i = 0; i < split_Can.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_Can[i]))
                {
                    f_c_Stats[i] = Convert.ToSingle(split_Can[i]);
                }
            }

            // 이름들 정보 가져오기
            #region 이름 정보들 가져오기
            split_string = null;
            if (s_Data.s_JuimName != null)
                split_string = s_Data.s_JuimName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Juim[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_DealiName != null)
                split_string = s_Data.s_DealiName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Deali[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_GwajangName != null)
                split_string = s_Data.s_GwajangName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Gwajang[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_BujangName != null)
                split_string = s_Data.s_BujangName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Bujang[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_IsaName != null)
                split_string = s_Data.s_IsaName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Isa[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_JeonmuName != null)
                split_string = s_Data.s_JeonmuName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Jeonmu[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_SangmuName != null)
                split_string = s_Data.s_SangmuName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Sangmu[i] = split_string[i];
                }
            }

            split_string = null;
            if (s_Data.s_SajangName != null)
                split_string = s_Data.s_SajangName.Split(',');
            for (int i = 0; i < split_string.Length; ++i)
            {
                if (!string.IsNullOrEmpty(split_string[i]))
                {
                    s_Sajang[i] = split_string[i];
                }
            }
            #endregion
            if (s_Data.b_CanOn != null)
                if (s_Data.b_CanOn != null && s_Data.b_CanOn.Length == 72)
                    b_Bought = Make2DArray(s_Data.b_CanOn, 6, 12);

            b_FildLoad = true;
        }
        else
            b_FildLoad = false;

#if UNITY_ANDROID
        GoogleManager_Scr.instance.b_isNextScene = true;

#elif UNITY_IOS
        AppleManager_Scr.instance.b_isNextScene = true;

#endif
    }

    public void Save()
    {
        if (ManPowerManager_Scr.instance != null && GoldManager_Scr.instance != null && StageManager_Scr.instance != null)
        {
            s_Data.i_Level = ManPowerManager_Scr.instance.i_Level;
            s_Data.i_Coin = GoldManager_Scr.instance.i_Coin;
            s_Data.i_Gem = GoldManager_Scr.instance.i_Gem;
            s_Data.i_Stage = StageManager_Scr.instance.i_StageClearLevel;
            s_Data.f_Exp = ExpManager_Scr.instance.f_nowExp;
            s_Data.b_BGOnOff = Button_Option.instance.setVolume_Background;
            s_Data.b_EftOnOff = Button_Option.instance.setVolume_Effect;
            s_Data.f_MaxExp = ExpManager_Scr.instance.f_fullExp;
        }

        s_Data.s_Language = TranslateManager_Scr.instance.s_Language;
        s_Data.i_LangueValue = TranslateManager_Scr.instance.i_LanguageValue;
        s_Data.i_LanguageIdx = TranslateManager_Scr.instance.i_LanguageIdx;

        if (Inventory_Scr.instance != null)
        {
            s_Data.i_InventoryLevel = Inventory_Scr.instance.InventoryLevel;
            for (int i = 0; i < 80; ++i)
            {
                if (Inventory_Scr.instance.myItems[i].item != null)
                {
                    s_Data.b_itemEmpty[i] = false;

                }
                else
                {
                    s_Data.b_itemEmpty[i] = true;
                
                }

                s_Data.i_ItemNum[i] = Inventory_Scr.instance.myItems[i].csvNum;
            }
        }

        if(DeliveryBoxManager_Scr.instance != null)
        {
            s_Data.i_DeliveryLevel = DeliveryBoxManager_Scr.instance.DeliveryLevel;
            s_Data.i_DeliveryCount = DeliveryBoxManager_Scr.instance.GetBoxCount;
        }

        if (GoogleManager_Scr.instance != null && !GoogleManager_Scr.instance.b_isLogin)
            s_Data.i_RankingPoint = GoogleManager_Scr.instance.i_RankingPoint;

        if (ADMobManager_Scr.instance != null)
            s_Data.b_isPremiumMode = ADMobManager_Scr.instance.b_isPremiumMode;

        s_Data.i_nowUpgradeSteps = new int[ChangeManager_Scr.instance.t_CharacterList.childCount];
        s_Data.b_CharacterBought = new bool[100];
        s_Data.b_CharacterSelected = new bool[100];
        for (int i = 0; i < ChangeManager_Scr.instance.t_CharacterList.childCount; ++i)
        {
            GameObject obj = ChangeManager_Scr.instance.t_CharacterList.GetChild(i).gameObject;
            s_Data.b_CharacterBought[i] = obj.GetComponent<CharChangerStat_Scr>().b_isBought;
            s_Data.b_CharacterSelected[i] = obj.GetComponent<CharChangerStat_Scr>().b_isSelected;
            s_Data.i_nowUpgradeSteps[i] = obj.GetComponent<CharChangerStat_Scr>().i_nowUpgradeSteps;
        }

        s_Data.i_NowLevelCount = ManPowerManager_Scr.instance.i_LevelManCount;
        s_Data.i_PreLevelCount = ManPowerManager_Scr.instance.i_preLevelCount;
        s_Data.i_CharType = ChangeManager_Scr.instance.i_CharType;

        #region 이름 변경
        s_Data.s_JuimName = null;
        s_Data.s_DealiName = null;
        s_Data.s_GwajangName = null;
        s_Data.s_BujangName = null;
        s_Data.s_SajangName = null;
        s_Data.s_JeonmuName = null;
        s_Data.s_IsaName = null;
        s_Data.s_SangmuName = null;
        for (int i = 0; i < ShopManager_Scr.instance.s_JuimName.Count; ++i)
        {
            s_Data.s_JuimName += string.Concat(ShopManager_Scr.instance.s_JuimName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_DealiName.Count; ++i)
        {
            s_Data.s_DealiName += string.Concat(ShopManager_Scr.instance.s_DealiName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_GwajangName.Count; ++i)
        {
            s_Data.s_GwajangName += string.Concat(ShopManager_Scr.instance.s_GwajangName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_BujangName.Count; ++i)
        {
            s_Data.s_BujangName += string.Concat(ShopManager_Scr.instance.s_BujangName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_IsaName.Count; ++i)
        {
            s_Data.s_IsaName += string.Concat(ShopManager_Scr.instance.s_IsaName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_JeonmuName.Count; ++i)
        {
            s_Data.s_JeonmuName += string.Concat(ShopManager_Scr.instance.s_JeonmuName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_SangmuName.Count; ++i)
        {
            s_Data.s_SangmuName += string.Concat(ShopManager_Scr.instance.s_SangmuName[i], ",");
        }

        for (int i = 0; i < ShopManager_Scr.instance.s_SajangName.Count; ++i)
        {
            s_Data.s_SajangName += string.Concat(ShopManager_Scr.instance.s_SajangName[i], ",");
        }
        #endregion

        s_Data.m_Stats = null;
        s_Data.b_ManPowerOn = new bool[1000];
        s_Data.b_isGetAchievment = new bool[6];
        for (int i = 0; i < ManPowerManager_Scr.instance.b_isGetAchievement.Length; ++i)
        {
            s_Data.b_isGetAchievment[i] = ManPowerManager_Scr.instance.b_isGetAchievement[i];
        }
        s_Data.i_ManCount = ManPowerManager_Scr.instance.t_ParentMans.childCount - ManPowerManager_Scr.instance.heroList.Count;

        int count = 0;
        s_Data.i_HeroType.Clear();
        for (int i = 0; i < ManPowerManager_Scr.instance.t_ParentMans.childCount; ++i)
        {
            GameObject obj = ManPowerManager_Scr.instance.t_ParentMans.GetChild(i).gameObject;
            if (obj.transform.GetComponent<ManStat_Scr>().b_isHero)
            {
                s_Data.i_HeroType.Add(obj.transform.GetComponent<ManStat_Scr>().i_HeroNum);
                count++;
            }
            else
            {
                int tmp = i - count;
                s_Data.b_ManPowerOn[tmp] = obj.GetComponent<ManStat_Scr>().b_Bought;
                s_Data.m_Stats += obj.GetComponent<ManStat_Scr>().i_Level + "," + obj.GetComponent<ManStat_Scr>().f_Atk + "," +
                obj.GetComponent<ManStat_Scr>().i_Money + "," + obj.GetComponent<ManStat_Scr>().i_TypeNum + "," + obj.GetComponent<ManStat_Scr>().i_UpgradeMoney + "," +
                obj.GetComponent<ManStat_Scr>().i_UpgradeNum + ",";
            }
            /*
            if (!ManPowerManager_Scr.instance.t_ParentMans.GetChild(i).GetComponent<ManStat_Scr>().b_isHero)
            {
                GameObject obj = ManPowerManager_Scr.instance.t_ParentMans.GetChild(i).gameObject;
                s_Data.b_ManPowerOn[i] = obj.GetComponent<ManStat_Scr>().b_Bought;
                s_Data.m_Stats += obj.GetComponent<ManStat_Scr>().i_Level + "," + obj.GetComponent<ManStat_Scr>().f_Atk + "," +
                obj.GetComponent<ManStat_Scr>().i_Money + "," + obj.GetComponent<ManStat_Scr>().i_TypeNum + "," + obj.GetComponent<ManStat_Scr>().i_UpgradeMoney + "," +
                obj.GetComponent<ManStat_Scr>().i_UpgradeNum + ",";
                Debug.Log("nomal");

            }
            else
            {
                ManStat_Scr obj = ManPowerManager_Scr.instance.t_ParentMans.GetChild(i).GetComponent<ManStat_Scr>();
                s_Data.b_ManPowerOn[i] = true;
                s_Data.m_Stats += 0 + "," + -10 + "," + 0 + "," + obj.i_TypeNum + "," + 0 + "," + 0 + ",";
                Debug.Log("hero");

            }*/
        }

        s_Data.b_CanOn = new bool[72];
        s_Data.b_MachineOn = new bool[6];
        s_Data.c_Stats = null;
        for (int i = 0; i < 6; ++i)
        {
            // 해당 자판기 구매 여부 체크
            s_Data.b_MachineOn[i] = MachineSelectManager_Scr.instance.OnSaveMachineOnOff(i);
        }
        for (int i = 0; i < MachineSelectManager_Scr.instance.t_AllCanList.childCount; ++i)
        {
            for (int j = 0; j < MachineSelectManager_Scr.instance.t_AllCanList.GetChild(i).GetChild(0).GetChild(0).childCount; ++j)
            {
                // 모든 캔들을 체크하기 위함, 캔을 구매 했는지, 스텟은 어떻게 되는지 등
                GameObject obj = MachineSelectManager_Scr.instance.t_AllCanList.GetChild(i).GetChild(0).GetChild(0).GetChild(j).gameObject;
                if (obj.GetComponent<CanStat_Scr>().b_IsBought)
                    b_Bought[i, j] = true;
                else
                    b_Bought[i, j] = false;
                s_Data.c_Stats += obj.GetComponent<CanStat_Scr>().i_Level.ToString() + "," + obj.GetComponent<CanStat_Scr>().i_BuyPrice.ToString() + "," + obj.GetComponent<CanStat_Scr>().i_UpgradePrice.ToString() + ","
                    + obj.GetComponent<CanStat_Scr>().f_NowAtk.ToString() + "," + obj.GetComponent<CanStat_Scr>().f_NextAtk.ToString() + "," +
                     obj.GetComponent<CanStat_Scr>().i_ListNum.ToString() + "," + obj.GetComponent<CanStat_Scr>().i_CanNum.ToString() + ",";
            }
        }
        s_Data.i_LastSelectedNum = MachineSelectManager_Scr.instance.i_LastSelected;
        s_Data.i_VendingMachineNum = MachineSelectManager_Scr.instance.i_NowBoughtMachine;

        Debug.Log("시간 확인 : " + AttendanceManager_Scr.instance.d_StartDay);
        s_Data.i_GetCount = AttendanceManager_Scr.instance.i_GetCount;
        s_Data.d_StartDay = AttendanceManager_Scr.instance.d_StartDay.ToString("yyyy-MM-dd tt hh:mm:ss");
        s_Data.d_EndDay = AttendanceManager_Scr.instance.d_EndDay.ToString("yyyy-MM-dd tt hh:mm:ss");
        s_Data.b_Absents = new bool[5];
        s_Data.b_Attendances = new bool[5];
        s_Data.b_GetRewards = new bool[5];
        s_Data.d_Days = new string[5];
        s_Data.b_GetGem = AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(AttendanceManager_Scr.instance.t_SlotParent.transform.childCount - 1).GetComponent<AttendanceSlot_Scr>().b_GetReward;

        for (int i = 0; i < AttendanceManager_Scr.instance.t_SlotParent.childCount - 1; ++i)
        {
            s_Data.d_Days[i] = AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().d_Day.ToString("yyyy-MM-dd tt hh:mm:ss");
            s_Data.b_Absents[i] = AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Absent;
            s_Data.b_Attendances[i] = AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_Attendance;
            s_Data.b_GetRewards[i] = AttendanceManager_Scr.instance.t_SlotParent.transform.GetChild(i).GetComponent<AttendanceSlot_Scr>().b_GetReward;
        }

        s_Data.b_DontSee = AttendanceManager_Scr.instance.b_DontSee;
        s_Data.b_isStarted = AttendanceManager_Scr.instance.b_isStarted;
        s_Data.i_LastAccessDay = DateTimeOffset.UtcNow.LocalDateTime.Day;

        s_Data.i_MissionSlot = new int[3];
        s_Data.i_TargetMission = new int[3];
        s_Data.i_ClearMissionNum = new int[3];
        s_Data.b_MissionSuccess = new bool[3];
        s_Data.b_GetReward = new bool[4];
        s_Data.b_GetMission = new bool[3];

        s_Data.b_isNoQuit = MissionManager_Scr.instance.b_isNoQuit;
        s_Data.b_isNoHit = MissionManager_Scr.instance.b_isNoHit;
        s_Data.b_isNoDie = MissionManager_Scr.instance.b_isNoDie;

        if (s_Data.b_GetMission != null)
        {
            for (int i = 0; i < MissionManager_Scr.instance.G_MissionList.Length; ++i)
            {
                MissionSlot_Scr obj = MissionManager_Scr.instance.G_MissionList[i].GetComponent<MissionSlot_Scr>();
                if (!obj.b_isPerfectReward)
                {
                    if (obj.b_GetMission)
                    {
                        s_Data.b_GetMission[i] = true;
                        s_Data.i_MissionSlot[i] = MissionManager_Scr.instance.i_MissionSlot[i];
                        s_Data.i_TargetMission[i] = MissionManager_Scr.instance.i_TargetMission[i];
                        s_Data.i_ClearMissionNum[i] = MissionManager_Scr.instance.i_ClearMissionNum[i];
                        s_Data.b_MissionSuccess[i] = MissionManager_Scr.instance.b_MissionSuccess[i];
                        s_Data.b_GetReward[i] = MissionManager_Scr.instance.b_GetReward[i];
                    }
                    else
                        s_Data.b_GetMission[i] = false;
                }
                else
                {
                    s_Data.b_GetReward[i] = MissionManager_Scr.instance.b_GetReward[i];
                }
            }
        }

        s_Data.b_FirstPlaying = TutorialManager.instance.b_FirstPlaying;
        s_Data.b_isManPowerTutorial = TutorialManager.instance.b_isManPowerTutorial;
        s_Data.i_TutorialCount = TutorialManager.instance.i_TutorialCount;

        s_Data.c_Stats = s_Data.c_Stats.Substring(0, s_Data.c_Stats.Length - 1);
        Buffer.BlockCopy(b_Bought, 0, s_Data.b_CanOn, 0, 72 * sizeof(bool));
        s_Data.m_Stats = s_Data.m_Stats.Substring(0, s_Data.m_Stats.Length - 1);


        s_Data.b_CompletBagTutorial = TutorialManager.instance.b_BagTutorial;
        s_Data.b_CompletBoxTutorial = TutorialManager.instance.b_BoxTutorial; 
        s_Data.b_CompletSynthesisTutorial = TutorialManager.instance.b_SynthesisTutorial; 

        //Debug.Log("데이터 세이브 끝");

        IAPManager_Scr.instance.saveReceipt();

        if (manageMemory.instance.Onplay)
        {

            if (!b_isTestReset)
            {

                // 암호화 버전
                // 실제 배포 -> s_Path, 테스트모드 -> s_TestPath

                string jsonData = JsonUtility.ToJson(s_Data);
                string path = s_Path;

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
                string code = System.Convert.ToBase64String(bytes);

                File.WriteAllText(path, code, Encoding.UTF8);

                /*
                string jsonData = JsonUtility.ToJson(s_Data);
                string path = s_Path;

                File.WriteAllText(path, jsonData, Encoding.UTF8);
                */

                 Debug.Log("파일 저장 끝");
                Dataupload(code);
            }
        }

        /* 테스트모드
        if (!b_isTestReset)
        {
            string path = s_TestPath;

            File.WriteAllText(path, jsonData, Encoding.UTF8);
        }
        else
            return;
        */

    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name == "PlayScene")
        {
            Save();
            DatabaseManager.instance.UpdateUserData_HaveGem();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (SceneManager.GetActiveScene().name == "PlayScene")
        {
            Save();
            DatabaseManager.instance.UpdateUserData_HaveGem();
        }
    }
    private static T[,] Make2DArray<T>(T[] input, int height, int width)
    {
        T[,] output = new T[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                output[i, j] = input[i * width + j];
            }
        }
        return output;
    }
}
static class Extension
{
    public static IEnumerable<T> ToRow1D<T>(this T[,] array)
    {
        foreach (var item in array)
        {
            yield return item;
        }
    }
}