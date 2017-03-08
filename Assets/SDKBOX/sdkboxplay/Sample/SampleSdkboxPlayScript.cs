using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Sdkbox;

public class SampleSdkboxPlayScript : MonoBehaviour {

	Sdkbox.sdkboxplay sdplay;
	Text txtInfo;
	// Use this for initialization
	void Start ()
	{
		txtInfo = GameObject.Find("txt_info").GetComponent<Text>();
		sdplay = FindObjectOfType<Sdkbox.sdkboxplay>();
		if (null == sdplay) {
			output ("can't find sdkboxplay");
			return;
		}
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void signin() {
		if (null == sdplay) {
			return;
		}
		sdplay.signin ();
	}

	public void signout() {
		if (null == sdplay) {
			return;
		}
		sdplay.signout ();
	}

	public void showLeaderboard() {
		if (null == sdplay) {
			return;
		}
		sdplay.showLeaderboard ("ldb1");
	}

	public void showAchievements() {
		if (null == sdplay) {
			return;
		}
		sdplay.showAchievements ();
	}

	public void unlockAchievement() {
		if (null == sdplay) {
			return;
		}
		sdplay.unlockAchievement ("ten-games");
	}

	public void incrementAchievement() {
		if (null == sdplay) {
			return;
		}
		sdplay.incrementAchievement ("ten-games", 1);
	}

	public void submitScore() {
		if (null == sdplay) {
			return;
		}
		sdplay.submitScore ("ldb1", 1);
	}

	public void onConnectionStatusChanged( int status ) {
		string s = "onConnectionStatusChanged:" + status;
		output(s);
	}

	public void onScoreSubmitted( ScoreSubmitArgs args ) {
		string s = "onScoreSubmitted:" + args.leaderboard_name
			+ " score:" + args.score
			+ " maxScoreAlltime:" + args.maxScoreAllTime
			+ " maxScoreWeek:" + args.maxScoreWeek
			+ " maxScoreToday:" + args.maxScoreToday;
		output(s);
	}

	public void onMyScore( string leaderboard_name, int time_span, int collection_type, long score ) {
		string s = "onMyScore:" + leaderboard_name + " time_span:" + time_span + " collection_type:" + collection_type + " score:" + score;
		output(s);
	}

	public void onMyScoreError( string leaderboard_name, int time_span, int collection_type, ErrorArgs e) {
		string s = "onMyScoreError:" + leaderboard_name
			+ " time_span:" + time_span + " collection_type:" + collection_type
			+ " error_code:" + e.error_code + " error_description:" + e.error_description;
		output(s);
	}

	public void onPlayerCenteredScores( string leaderboard_name, int time_span, int collection_type, string json_with_score_entries ) {
		string s = "onPlayerCenteredScores:" + leaderboard_name
			+ " time_span:" + time_span + " collection_type:" + collection_type
			+ " json_with_score_entries:" + json_with_score_entries;
		output(s);
	}

	public void onPlayerCenteredScoresError( string leaderboard_name, int time_span, int collection_type, ErrorArgs e) {
		string s = "onPlayerCenteredScoresError:" + leaderboard_name
			+ " time_span:" + time_span + " collection_type:" + collection_type
			+ " error_code:" + e.error_code + " error_description:" + e.error_description;
		output(s);
	}

	public void onIncrementalAchievementUnlocked( string achievement_name ) {
		string s = "onIncrementalAchievementUnlocked:" + achievement_name;
		output(s);
	}

	public void onIncrementalAchievementStep( string achievement_name, int step ) {
		string s = "onIncrementalAchievementStep:" + achievement_name + " step:" + step;
		output(s);
	}

	public void onIncrementalAchievementStepError( string name, int steps, ErrorArgs e ) {
		string s = "onIncrementalAchievementStepError:" + name + " steps:" + steps
			+ " error_code:" + e.error_code + " error_description:" + e.error_description;
		output(s);
	}

	public void onAchievementUnlocked( string achievement_name, bool newlyUnlocked ) {
		string s = "onAchievementUnlocked:" + achievement_name + " newlyUnlocked:" + newlyUnlocked;
		output(s);
	}

	public void onAchievementUnlockError( string achievement_name, ErrorArgs e ) {
		string s = "onAchievementUnlockError:" + achievement_name
			+ " error_code:" + e.error_code + " error_description:" + e.error_description;
		output(s);
	}

	public void onAchievementsLoaded( bool reload_forced, string json_achievements_info ) {
		string s = "onAchievementsLoaded:" + reload_forced + " json_achievements_info:" + json_achievements_info;
		output(s);
	}

	public void onSetSteps( string name, int steps ) {
		string s = "onSetSteps:" + name + " steps:" + steps;
		output(s);
	}

	public void onSetStepsError( string name, int steps, ErrorArgs e ) {
		string s = "onSetStepsError:" + name + " steps:" + steps
			+ " error_code:" + e.error_code + " error_description:" + e.error_description;
		output(s);
	}

	public void onReveal( string name) {
		string s = "onReveal:" + name;
		output(s);
	}

	public void onRevealError( string name, ErrorArgs e ) {
		string s = "onRevealError:" + name
			+ " error_code:" + e.error_code + " error_description:" + e.error_description;
		output(s);
	}


	private void output(string s) {
		Debug.Log (s);
		if (null == txtInfo) {
			return;
		}
		string[] lines = txtInfo.text.Split ('\n');
		string line1 = "";
		string line2 = "";
		string line3 = s;
		if (lines.Length >= 1) {
			line2 = lines [lines.Length - 1];
		}
		if (lines.Length >= 2) {
			line1 = lines [lines.Length - 2];
		}
		txtInfo.text = line1 + "\n" + line2 + "\n" + line3;
	}

}
