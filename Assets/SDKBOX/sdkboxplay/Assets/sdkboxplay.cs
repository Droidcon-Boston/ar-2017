/*****************************************************************************
Copyright © 2015 SDKBOX.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using Sdkbox;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace Sdkbox
{
	[Serializable]
	public struct ScoreSubmitArgs {
		public string leaderboard_name;
		public int score;
		public bool maxScoreAllTime;
		public bool maxScoreWeek;
		public bool maxScoreToday;
	}

	[Serializable]
	public struct ErrorArgs {
		public int error_code;
		public string error_description;
	}


	[Serializable]
	public class sdkboxplay : PluginBase<sdkboxplay>
	{
		[Serializable]
		public class Callbacks
		{
			[Serializable]
			public class IntEvent : UnityEvent<int> {}
			[Serializable]
			public class ArgsEvent : UnityEvent<ScoreSubmitArgs> {}
			[Serializable]
			public class StringEvent : UnityEvent<string> {}
			[Serializable]
			public class StringIntEvent : UnityEvent<string, int> {}
			[Serializable]
			public class StringBoolEvent : UnityEvent<string, bool> {}
			[Serializable]
			public class StringIntIntLongEvent : UnityEvent<string, int, int, long> {}
			[Serializable]
			public class StringIntIntStringEvent : UnityEvent<string, int, int, string> {}
			[Serializable]
			public class StringIntIntErrorEvent : UnityEvent<string, int, int, ErrorArgs> {}
			[Serializable]
			public class StringIntErrorEvent : UnityEvent<string, int, ErrorArgs> {}
			[Serializable]
			public class StringErrorEvent : UnityEvent<string, ErrorArgs> {}
			[Serializable]
			public class BooleanStringEvent : UnityEvent<bool, string> {}


			public IntEvent onConnectionStatusChanged = null;
			public ArgsEvent onScoreSubmitted = null;
			public StringIntIntLongEvent onMyScore = null;
			public StringIntIntErrorEvent onMyScoreError = null;
			public StringIntIntStringEvent onPlayerCenteredScores = null;
			public StringIntIntErrorEvent onPlayerCenteredScoresError = null;
			public StringEvent onIncrementalAchievementUnlocked = null;
			public StringIntEvent onIncrementalAchievementStep = null;
			public StringIntErrorEvent onIncrementalAchievementStepError = null;
			public StringBoolEvent onAchievementUnlocked = null;
			public StringErrorEvent onAchievementUnlockError = null;
			public BooleanStringEvent onAchievementsLoaded = null;
			public StringIntEvent onSetSteps = null;
			public StringIntErrorEvent onSetStepsError = null;
			public StringEvent onReveal = null;
			public StringErrorEvent onRevealError = null;

			Callbacks()
			{
				if (null == onConnectionStatusChanged) {
					onConnectionStatusChanged = new IntEvent();
				}

				if (null == onScoreSubmitted) {
					onScoreSubmitted = new ArgsEvent();
				}

				if (null == onMyScore) {
					onMyScore = new StringIntIntLongEvent();
				}

				if (null == onMyScoreError) {
					onMyScoreError = new StringIntIntErrorEvent();
				}

				if (null == onPlayerCenteredScores) {
					onPlayerCenteredScores = new StringIntIntStringEvent();
				}

				if (null == onPlayerCenteredScoresError) {
					onPlayerCenteredScoresError = new StringIntIntErrorEvent();
				}

				if (null == onIncrementalAchievementUnlocked) {
					onIncrementalAchievementUnlocked = new StringEvent();
				}

				if (null == onIncrementalAchievementStep) {
					onIncrementalAchievementStep = new StringIntEvent();
				}

				if (null == onIncrementalAchievementStepError) {
					onIncrementalAchievementStepError = new StringIntErrorEvent();
				}

				if (null == onAchievementUnlocked) {
					onAchievementUnlocked = new StringBoolEvent();
				}

				if (null == onAchievementUnlockError) {
					onAchievementUnlockError = new StringErrorEvent();
				}

				if (null == onAchievementsLoaded) {
					onAchievementsLoaded = new BooleanStringEvent();
				}

				if (null == onSetSteps) {
					onSetSteps = new StringIntEvent();
				}

				if (null == onSetStepsError) {
					onSetStepsError = new StringIntErrorEvent();
				}

				if (null == onReveal) {
					onReveal = new StringEvent();
				}

				if (null == onRevealError) {
					onRevealError = new StringErrorEvent();
				}


			}
		};

		[Serializable]
		public class LeaderBoardItem
		{
			public string id;
			public string name;
		};

		[Serializable]
		public class AchievementItem
		{
			public string id;
			public string name;
			public bool incremental;
		};

		[Serializable]
		public class Settings
		{
			public LeaderBoardItem[] leaderboards;
			public AchievementItem[] achievements;
			public bool debug;
			public bool connect_on_start;

			Settings()
			{
				debug = false;
				connect_on_start = true;
			}
		}

		public Settings  settings;
		public Callbacks callbacks;

		// delegate signature for callbacks from SDKBOX sdkboxplay runtime.
		public delegate void CallbacksdkboxplayDelegate(string method, string json);

		[MonoPInvokeCallback(typeof(CallbacksdkboxplayDelegate))]
		public static void sdkboxplayCallback(string method, string json)
		{
			if (null != _this)
			{
				var sdkboxplay = (_this as sdkboxplay);
				sdkboxplay.handleCallback(method, json);
			}
			else
			{
				Debug.Log("Missed sdkboxplay callback " + method);
			}
		}

		protected void handleCallback(string method, string jsonString)
		{
			Debug.Log("Dispatching sdkboxplay callback method: " + method);
			Json json = Json.parse(jsonString);
			if (json.is_null())
			{
				Debug.LogError("Failed to parse JSON callback payload");
				throw new System.ArgumentException("Invalid JSON payload");
			}

			switch (method)
			{
			case "onConnectionStatusChanged": {
				if (null != callbacks.onConnectionStatusChanged) {
					callbacks.onConnectionStatusChanged.Invoke(json["status"].int_value());
				}
				break;
			}
			case "onScoreSubmitted": {
				if (null != callbacks.onScoreSubmitted) {
					ScoreSubmitArgs args = new ScoreSubmitArgs ();
					args.leaderboard_name = json["leaderboard_name"].string_value();
					args.score = json["score"].int_value();
					args.maxScoreAllTime = json["maxScoreAllTime"].bool_value();
					args.maxScoreWeek = json["maxScoreWeek"].bool_value();
					args.maxScoreToday = json["maxScoreToday"].bool_value();

					callbacks.onScoreSubmitted.Invoke(args);
				}
				break;
			}
			case "onMyScore": {
				if (null != callbacks.onMyScore) {
						callbacks.onMyScore.Invoke(json["leaderboard_name"].string_value(),
							json["time_span"].int_value(),
							json["collection_type"].int_value(),
							(long)(json["score"].double_value()));
				}
				break;
			}
			case "onMyScoreError": {
				if (null != callbacks.onMyScoreError) {
						ErrorArgs e;
						e.error_code = json ["error_code"].int_value ();
						e.error_description = json ["error_description"].string_value ();
						callbacks.onMyScoreError.Invoke(json["leaderboard_name"].string_value(),
							json["time_span"].int_value(),
							json["collection_type"].int_value(),
							e);
				}
				break;
			}
			case "onPlayerCenteredScores": {
				if (null != callbacks.onPlayerCenteredScores) {
						callbacks.onPlayerCenteredScores.Invoke(
							json["leaderboard_name"].string_value(),
							json["time_span"].int_value(),
							json["collection_type"].int_value(),
							json["json_with_score_entries"].string_value());
				}
				break;
			}
			case "onPlayerCenteredScoresError": {
				if (null != callbacks.onPlayerCenteredScoresError) {
					ErrorArgs e;
					e.error_code = json ["error_code"].int_value ();
					e.error_description = json ["error_description"].string_value ();
					callbacks.onPlayerCenteredScoresError.Invoke(
						json["leaderboard_name"].string_value(),
						json["time_span"].int_value(),
						json["collection_type"].int_value(),
						e);
				}
				break;
			}
			case "onIncrementalAchievementUnlocked": {
				if (null != callbacks.onIncrementalAchievementUnlocked) {
					callbacks.onIncrementalAchievementUnlocked.Invoke(json["achievement_name"].string_value());
				}
				break;
			}
			case "onIncrementalAchievementStep": {
				if (null != callbacks.onIncrementalAchievementStep) {
					callbacks.onIncrementalAchievementStep.Invoke(json["achievement_name"].string_value(), json["step"].int_value());
				}
				break;
			}
			case "onIncrementalAchievementStepError": {
				if (null != callbacks.onIncrementalAchievementStepError) {
					ErrorArgs e;
					e.error_code = json ["error_code"].int_value ();
					e.error_description = json ["error_description"].string_value ();
					callbacks.onIncrementalAchievementStepError.Invoke(
						json["name"].string_value(),
						json["steps"].int_value(),
						e);
				}
				break;
			}
			case "onAchievementUnlocked": {
				if (null != callbacks.onAchievementUnlocked) {
					callbacks.onAchievementUnlocked.Invoke(json["achievement_name"].string_value(), json["newlyUnlocked"].bool_value());
				}
				break;
			}
			case "onAchievementUnlockError": {
				if (null != callbacks.onAchievementUnlockError) {
					ErrorArgs e;
					e.error_code = json ["error_code"].int_value ();
					e.error_description = json ["error_description"].string_value ();
					callbacks.onAchievementUnlockError.Invoke(
						json["achievement_name"].string_value(),
						e);
				}
				break;
			}
			case "onAchievementsLoaded": {
				if (null != callbacks.onAchievementsLoaded) {
					callbacks.onAchievementsLoaded.Invoke(
						json["reload_forced"].bool_value(),
						json["json_achievements_info"].string_value());
				}
				break;
			}
			case "onSetSteps": {
				if (null != callbacks.onSetSteps) {
					callbacks.onSetSteps.Invoke(
						json["name"].string_value(),
						json["steps"].int_value());
				}
				break;
			}
			case "onSetStepsError": {
				if (null != callbacks.onSetStepsError) {
					ErrorArgs e;
					e.error_code = json ["error_code"].int_value ();
					e.error_description = json ["error_description"].string_value ();
					callbacks.onSetStepsError.Invoke(
						json["name"].string_value(),
						json["steps"].int_value(),
						e);
				}
				break;
			}
			case "onReveal": {
				if (null != callbacks.onReveal) {
					callbacks.onReveal.Invoke(json["name"].string_value());
				}
				break;
			}
			case "onRevealError": {
				if (null != callbacks.onRevealError) {
					ErrorArgs e;
					e.error_code = json ["error_code"].int_value ();
					e.error_description = json ["error_description"].string_value ();
					callbacks.onRevealError.Invoke(
						json["name"].string_value(),
						e);
				}
				break;
			}
			default:
				throw new System.ArgumentException("Unknown sdkboxplay callback type");
			}
		}

		protected string buildConfiguration()
		{
			Json config = newJsonObject();
			Json cur;

			cur = config;
			cur["ios"]    = newJsonObject(); cur = cur["ios"];
			cur["sdkboxplay"] = newJsonObject(); cur = cur["sdkboxplay"];
			cur["debug"] = new Json(settings.debug);
			cur["connect_on_start"] = new Json(settings.connect_on_start);

			List<Json> li = new List<Json>();
			foreach (var p in settings.leaderboards)
			{
				Json j = newJsonObject();
				j["id"] = new Json(p.id);
				j["name"] = new Json(p.name);
				li.Add (j);
			}
			cur ["leaderboards"] = new Json (li);

			List<Json> ai = new List<Json>();
			foreach (var p in settings.achievements)
			{
				Json j = newJsonObject();
				j["id"] = new Json(p.id);
				j["name"] = new Json(p.name);
				j["incremental"] = new Json(p.incremental);
				ai.Add (j);
			}
			cur ["achievements"] = new Json (ai);

			cur = config;
			cur["android"] = newJsonObject(); cur = cur["android"];
			cur["sdkboxplay"] = newJsonObject(); cur = cur["sdkboxplay"];
			cur["debug"] = new Json(settings.debug);
			cur["connect_on_start"] = new Json(settings.connect_on_start);

			List<Json> la = new List<Json>();
			foreach (var p in settings.leaderboards)
			{
				Json j = newJsonObject();
				j["id"] = new Json(p.id);
				j["name"] = new Json(p.name);
				la.Add (j);
			}
			cur ["leaderboards"] = new Json (la);

			List<Json> aa = new List<Json>();
			foreach (var p in settings.achievements)
			{
				Json j = newJsonObject();
				j["id"] = new Json(p.id);
				j["name"] = new Json(p.name);
				j["incremental"] = new Json(p.incremental);
				aa.Add (j);
			}
			cur ["achievements"] = new Json (aa);

			return config.dump();
		}

		protected override void init()
		{
			Debug.Log("SDKBOX sdkboxplay starting.");

			SDKBOX.Instance.init(); // reference the SDKBOX singleton to ensure shared init.

			#if !UNITY_EDITOR
			string config = buildConfiguration();
			Debug.Log("configuration: " + config);

			#if UNITY_ANDROID
			sdkboxplay._player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				// call sdkboxplay::init()
				sdkbox_play_init(config);
				sdkbox_play_set_unity_callback(sdkboxplayCallback);
				Debug.Log("SDKBOX sdkboxplay Initialized.");
			}));
			#else
			sdkbox_play_init(config);
			sdkbox_play_set_unity_callback(sdkboxplayCallback);
			Debug.Log("SDKBOX sdkboxplay Initialized.");
			#endif
			#endif // !UNITY_EDITOR
		}

//		public string getVersion()
//		{
//			string ret = "";
//			#if !UNITY_EDITOR
//			_lazy_init();
//			#if UNITY_ANDROID
//			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
//			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
//				ret = sdkbox_play_getVersion();
//			}));
//			#else
//			ret = sdkbox_play_getVersion();
//			#endif
//			#endif // !UNITY_EDITOR
//			return ret;
//		}

		public void submitScore(string name, int score)
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
			sdkbox_play_submitScore(name, score);
			}));
			#else
			sdkbox_play_submitScore(name, score);
			#endif
			#endif // !UNITY_EDITOR
		}

		public void showLeaderboard(string name)
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
			sdkbox_play_showLeaderboard(name);
			}));
			#else
			sdkbox_play_showLeaderboard(name);
			#endif
			#endif // !UNITY_EDITOR
		}

		public void unlockAchievement(string name)
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
			sdkbox_play_unlockAchievement(name);
			}));
			#else
			sdkbox_play_unlockAchievement(name);
			#endif
			#endif // !UNITY_EDITOR
		}

		public void incrementAchievement(string name, int increment)
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
			sdkbox_play_incrementAchievement(name, increment);
			}));
			#else
			sdkbox_play_incrementAchievement(name, increment);
			#endif
			#endif // !UNITY_EDITOR
		}

		public void showAchievements()
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
			sdkbox_play_showAchievements();
			}));
			#else
			sdkbox_play_showAchievements();
			#endif
			#endif // !UNITY_EDITOR
		}

		public void signin()
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				sdkbox_play_signin();
			}));
			#else
			sdkbox_play_signin();
			#endif
			#endif // !UNITY_EDITOR
		}

		public void signout()
		{
			#if !UNITY_EDITOR
			_lazy_init();
			#if UNITY_ANDROID
			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				sdkbox_play_signout();
			}));
			#else
			sdkbox_play_signout();
			#endif
			#endif // !UNITY_EDITOR
		}

//		public bool isSignedIn()
//		{
//			bool ret = false;
//			#if !UNITY_EDITOR
//			_lazy_init();
//			#if UNITY_ANDROID
//			AndroidJavaObject activity = sdkboxplay._player.GetStatic<AndroidJavaObject>("currentActivity");
//			activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
//				ret = sdkbox_play_isSignedIn();
//			}));
//			#else
//			ret = sdkbox_play_isSignedIn();
//			#endif
//			#endif // !UNITY_EDITOR
//			return ret;
//		}

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_init(string jsonstring);

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_set_unity_callback(CallbacksdkboxplayDelegate callback);

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern string sdkbox_play_getVersion();

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_submitScore(string name, int scrore);

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_showLeaderboard(string name);

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_unlockAchievement(string name);

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_incrementAchievement(string name, int increment);

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_showAchievements();

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern bool sdkbox_play_isConnected();

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_signin();

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern void sdkbox_play_signout();

		#if UNITY_IOS
		[DllImport("__Internal")]
		#else
		[DllImport("sdkboxplay")]
		#endif
		private static extern bool sdkbox_play_isSignedIn();

	}
}
