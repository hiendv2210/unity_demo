using UnityEngine;
using System.Collections;
using System;
using UnityEngine.iOS;

public class LocalNotificationManager : MonoBehaviour {
	public double DistanceTime;
//	PushLocalTime[] listPushTime = new PushLocalTime[]{
//		new PushLocalTime(0,60,10),
//		new PushLocalTime(1,65,11),
//		new PushLocalTime(2,300,12),
//		new PushLocalTime(3,360,13),
//		new PushLocalTime(4,420,14),
//		new PushLocalTime(5,480,15),
//		new PushLocalTime(6,540,16),
//		new PushLocalTime(7,24*60*60,15),
//		new PushLocalTime(8,3*24*60*60,30),
//		new PushLocalTime(9,7*24*60*60,100),
//		new PushLocalTime(10,14*24*60*60,200),
//	};
//

//    PushLocalTime[] listPushTime = new PushLocalTime[]{
//        new PushLocalTime(0,300,1),
//        new PushLocalTime(1,600,1),
//        new PushLocalTime(2,900,3),
//        new PushLocalTime(3,1200,10),
//        new PushLocalTime(4,1500,20),
//    };
	PushLocalTime[] listPushTime = new PushLocalTime[]{
		new PushLocalTime(0,3600,1),
		new PushLocalTime(1,24*60*60,1),
		new PushLocalTime(2,3*24*60*60,3),
		new PushLocalTime(3,7*24*60*60,10),
		new PushLocalTime(4,14*24*60*60,20),
	};

    private bool isStart = false;
	void Awake(){
		GetDistanceTime();
	}
	// Use this for initialization
	void Start () {
        Debug.LogError("Start");
		DontDestroyOnLoad(this);
        CheckExitsPush();
        isStart = true;
	}

	public void CheckExitsPush(){
		bool checkStartAppFromNotificaiton = KaopizLocalNotification.CheckStartAppFromNotification();
		if(checkStartAppFromNotificaiton){
            
			AddGemForStartApp();
            Debug.LogError("CheckExitsPush:"+AppState.CurrentScreen);
            if(AppState.CurrentScreen != ScreenType.Loading){
				ShowReciveRubyPopup();
			}
		}
		else{
			DistanceTime = 0;
            DataInLocalStorage.IsReceiveGemForStart = 0;
        }
        ClearAllLocalNotification();
	}


	public void ShowReciveRubyPopup(){
        if (DataInLocalStorage.IsReceiveGemForStart > 0) {
            PopupController.Instance.ReceiveRubyPopup.InitPopup ( MessageData.RECEIVE_DIAMON ,DataInLocalStorage.IsReceiveGemForStart);
            DataInLocalStorage.IsReceiveGemForStart = 0;
        }
	}


	public void GetDistanceTime(){
		Debug.LogError("GetDistanceTime:"+DistanceTime.ToString());
		DateTime currentTime = DateTime.Now;
        if(DataInLocalStorage.CloseAppTime != null && DataInLocalStorage.CloseAppTime != "" && DistanceTime == 0){
            DateTime dateOfExit = DateTime.Parse(DataInLocalStorage.CloseAppTime);
			DistanceTime = currentTime.Subtract(dateOfExit).TotalSeconds;
			Debug.LogError("OnApplicationPause:"+DistanceTime.ToString());
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddGemForStartApp(){
		PushLocalTime receiveDiamon = GetPresentWhenOpenApp();
        Debug.LogError("AddGemForStartApp:"+receiveDiamon);
        if(receiveDiamon != null){
            DataInLocalStorage.UserDiamon += receiveDiamon.Diamon;
            DataInLocalStorage.CurrentPushLocalID = receiveDiamon.ID + 1;
            DataInLocalStorage.IsReceiveGemForStart = receiveDiamon.Diamon;
		}
		DistanceTime = 0;
	}

	public PushLocalTime GetPresentWhenOpenApp(){
		PushLocalTime rubyPush = null;
        for(int i = DataInLocalStorage.CurrentPushLocalID; i < listPushTime.Length; i++){
			if(DistanceTime >= (double)listPushTime[i].PushTime){
				rubyPush = listPushTime[i];
			}
			else{
				break;
			}
		}

        Debug.LogError("GetPresentWhenOpenApp:"+rubyPush);

		return rubyPush;
	}


	public void CreateOpenAppNotification(){

		KaopizDebug.LogError("CreateOpenAppNotification:"+listPushTime.Length);
        if( DataInLocalStorage.SettingDiamon != 0){
            return;
        }
        int index = DataInLocalStorage.CurrentPushLocalID;
		for(int i = index; i < listPushTime.Length; i++){
            KaopizDebug.LogError("Create Notification");
            KaopizLocalNotification.SendNotification(i, listPushTime[i].PushTime , "魔女", MessageData.ReceiveDiamonFromNotification(listPushTime[i].Diamon),Color.white);
		}
		SetCurrentTimeToLocalstorage();

	}

	public void SetCurrentTimeToLocalstorage(){
		DateTime currentTime = DateTime.Now;
		string saveTime = currentTime.ToString();
        DataInLocalStorage.CloseAppTime = saveTime;
	}

	void OnApplicationPause(bool pauseStatus) {
        if( !isStart){
            return;
        }
		if(pauseStatus){
			CreateOpenAppNotification();
		}
		else{
			GetDistanceTime();
			CheckExitsPush();
			Screen.orientation = ScreenOrientation.Portrait;
			ClearAllLocalNotification();
			DateTime currentTime = DateTime.Now;
            if(DataInLocalStorage.CloseAppTime != null && DataInLocalStorage.CloseAppTime != ""){
				SetCurrentTimeToLocalstorage();
			}
		}
	}

	void ClearAllLocalNotification(){
		KaopizLocalNotification.CancelAllNotificaiton();
	}
}

public class PushLocalTime{
	public int ID;
	public long PushTime;
	public int Diamon;
    public PushLocalTime(int id,long pushTime , int diamon){
		ID = id;
		PushTime = pushTime;
        Diamon = diamon;
	}

}
