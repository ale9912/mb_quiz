﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    
    [SerializeField] private QuizGameUI quizGameUI;
    
    [SerializeField] private List<QuizDataScriptable> quizDataList;
    [SerializeField] private float timeInSeconds;
#pragma warning restore 649

    public AudioSource correctAudio;
    public AudioSource wrongAudio;
    private string currentCategory = "";
    private int correctAnswerCount = 0;
    //questions data
    private List<Question> questions;
    //current question data
    private Question selectedQuetion = new Question();
    private int gameScore;
    private int lifesRemaining;
    private float currentTime;
    private QuizDataScriptable dataScriptable;
    

    private GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus { get { return gameStatus; } }

    public List<QuizDataScriptable> QuizData { get => quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        currentCategory = category;
        correctAnswerCount = 0;
        gameScore = 0;
        lifesRemaining = 3;
        currentTime = timeInSeconds;
        //set the questions data
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);
        //select the question
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
        quizGameUI.NewRecord.gameObject.SetActive(false);
    }

    /// <summary>
    /// Method used to randomly select the question form questions data
    /// </summary>
    private void SelectQuestion()
    {
        //get the random number
        int val = UnityEngine.Random.Range(0, questions.Count);
        //set the selectedQuetion
        selectedQuetion = questions[val];
        //send the question to quizGameUI
        quizGameUI.SetQuestion(selectedQuetion);

        questions.RemoveAt(val);
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            currentTime -= Time.deltaTime;
            SetTime(currentTime);
        }
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);                       //set the time value
        quizGameUI.TimerText.text = time.ToString("mm':'ss");   //convert time to Time format

        if (currentTime <= 0)
        {
            //Game Over
            GameEnd();
        }
    }

    /// <summary>
    /// Method called to check the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        //set default to false
        bool correct = false;
        //if selected answer is similar to the correctAns
        if (selectedQuetion.correctAns == selectedOption)
        {
            //Yes, Ans is correct
            correctAudio.Play();
            correctAnswerCount++;
            correct = true;
            gameScore += 1;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
        }
        else
        {
            //No, Ans is wrong
            //Reduce Life
            wrongAudio.Play();
            lifesRemaining--;
            quizGameUI.ReduceLife(lifesRemaining);

            if (lifesRemaining == 0)
            {
                GameEnd();
            }
        }

        if (gameStatus == GameStatus.PLAYING)
        {
            if (questions.Count > 0)
            {
                //call SelectQuestion method again after 1s
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
        //return the value of correct bool
        return correct;
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);
        quizGameUI.FinalScoreText.text = "Punteggio finale: " + gameScore;
        

        if (correctAnswerCount > PlayerPrefs.GetInt(currentCategory))
        {
            PlayerPrefs.SetInt(currentCategory, correctAnswerCount);
            quizGameUI.NewRecord.gameObject.SetActive(true);
        }
            

        quizGameUI.RecordText.text = "Record: " + PlayerPrefs.GetInt(currentCategory);


    }
}



//Datastructure for storeing the quetions data
[System.Serializable]
public class Question
{
    public string questionInfo;         //question text
    public QuestionType questionType;   //type
    
    public List<string> options;        //options to select
    public string correctAns;           //correct option
}

[System.Serializable]
public enum QuestionType
{
    TEXT
  
}

[SerializeField]
public enum GameStatus
{
    PLAYING,
    NEXT
}