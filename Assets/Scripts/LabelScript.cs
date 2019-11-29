using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using TMPro;
public class LabelScript : MonoBehaviour {

    private DateTime startDt;
    [SerializeField] private InputField idInputField;
    [SerializeField] private GameObject startButton;
    [SerializeField] private Text startButtonText;
    [SerializeField] private TextMeshProUGUI currentExerciseText;
    private bool isTesting;
    private int subjectID = 0;
    private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private List<int> labelList;
    private List<double> timeList;
    private List<long> currentTimeList;

    // Use this for initialization
    void Start () {
        isTesting = false;
        currentExerciseText.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InputID()
    {
        subjectID = int.Parse(idInputField.text);
        startButton.SetActive(true);
    }

    public void OnClickLabelButton(int n)
    {
        labelList.Add(n);
        TimeSpan ts = DateTime.Now - startDt;
        timeList.Add(ts.TotalSeconds);
        TimeSpan span = DateTime.UtcNow - Jan1st1970;
        currentTimeList.Add((long)span.TotalMilliseconds);
    }
    public void SetExerciseText(string str)
    {
        currentExerciseText.text = str;
    }

    public void OnClickStartButton()
    {
        if (isTesting)
        {
            TestStop();
        }
        else
        {
            TestStart();
        }
    }

    private void TestStart()
    {
        isTesting = true;
        currentExerciseText.gameObject.SetActive(true);
        idInputField.gameObject.SetActive(false);
        startButtonText.text = "Stop";
        labelList = new List<int>();
        timeList = new List<double>();
        currentTimeList = new List<long>();
        startDt = DateTime.Now;
    }

    public void TestStop()
    {
        isTesting = false;
        currentExerciseText.gameObject.SetActive(false);
        idInputField.gameObject.SetActive(true);
        startButtonText.text = "Writing...";
        Thread thread = new Thread(
            () =>
            {
                WriteAccFile();
            });
        thread.Start();
    }

    void WriteAccFile()
    {
        string filepath = GetPath() + "/Label_" + String.Format("{0:000}", subjectID) + DateTime.Now.ToString("_yyyyMMdd_HHmmss") + ".csv";
        StreamWriter file = new StreamWriter(filepath, true);
        file.WriteLine("label, localTime, judgedNum");
        for (int i = 0; i < labelList.Count; i++)
        {
            file.Write(labelList[i] + ",");
            file.Write(timeList[i] + ",");
            file.WriteLine(currentTimeList[i] + ",");
        }
        file.Flush();
        file.Close();
        startButtonText.text = "Start";
    }

    private string GetPath()
    {
        #if UNITY_ANDROID
            return Application.persistentDataPath;
        #elif UNITY_IPHONE
            return Application.persistentDataPath+"/";
        #else
            return Application.dataPath + "/";
        #endif
    }
}
