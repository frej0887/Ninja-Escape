﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ToLvlSelect : MonoBehaviour {
    public Button LvlSelectBtn;

    // Use this for initialization
    void Start()
    {
        Button btnLvl1 = LvlSelectBtn.GetComponent<Button>();
        btnLvl1.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void TaskOnClick()
    {
        SceneManager.LoadScene("Level switch");
    }
}