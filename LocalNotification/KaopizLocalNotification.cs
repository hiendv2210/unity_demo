using UnityEngine;
using UnityEngine.iOS;
using System.Collections;
using System.Collections.Generic;
using System;

class KaopizLocalNotification
{
	/// <summary>
	/// Inexact uses `set` method
	/// Exact uses `setExact` method
	/// ExactAndAllowWhileIdle uses `setAndAllowWhileIdle` method
	/// Documentation: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
	/// </summary>
	public enum NotificationExecuteMode
	{
		Inexact = 0,
		Exact = 1,
		ExactAndAllowWhileIdle = 2
	}

	#if UNITY_ANDROID && !UNITY_EDITOR
    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    private static string mainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";
#endif

	public static void SendNotification (int id, TimeSpan delay, string title, string message)
	{
		SendNotification (id, (int)delay.TotalSeconds, title, message, Color.white);
	}
    
	public static void SendNotification (int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
	{
		KaopizDebug.LogError(message);
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delay * 1000L, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName);
        }
#elif UNITY_IOS
        var notif = new  UnityEngine.iOS.LocalNotification();
		notif.fireDate = DateTime.Now.AddSeconds(delay);
		notif.alertBody = message;
        notif.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		notif.applicationIconBadgeNumber = 1;

        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
#endif
	}

	public static void SendRepeatingNotification (int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, delay * 1000L, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName);
        }
		#elif UNITY_IOS
#endif
	}

	public static void CancelNotification (int id)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
		#elif UNITY_IOS
#endif
	}

	public static void CancelAllNotificaiton ()
	{

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
		if (pluginClass != null) {
			pluginClass.CallStatic("CancelAll");
		}

		for(int i = 0; i < 11; i++){
			CancelNotification(i);
        }

        #elif UNITY_IPHONE
        Debug.LogError("CancelAllNotificaiton");
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications ();
		
        var temp = new UnityEngine.iOS.LocalNotification();
        temp.fireDate = DateTime.Now.AddSeconds(0.01);
		temp.applicationIconBadgeNumber = -1;
		temp.alertBody = "";
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
        UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(temp);
		#endif
	}

	public static bool CheckStartAppFromNotification ()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
		if (pluginClass != null) {
			return pluginClass.CallStatic<bool>("checkStartFromNotification");
		}
		return false;
        #elif UNITY_IOS
        Debug.LogError("localNotificationCount:"+UnityEngine.iOS.NotificationServices.localNotificationCount);

//        if (UnityEngine.iOS.NotificationServices.localNotificationCount > 0) {
//            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
//			return true;
//		}
//		return false;
        return true;
		#endif
		return true;
	}
}
