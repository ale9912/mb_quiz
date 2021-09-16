using System;
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
    
    private List<Question> questions;
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
        
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
        quizGameUI.NewRecord.gameObject.SetActive(false);
    }

    //seleziona una domanda casuale
    private void SelectQuestion()
    {
        
        int val = UnityEngine.Random.Range(0, questions.Count);
        //imposta la domanda selezionata
        selectedQuetion = questions[val];
        //imposta domanda nella UI
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
        TimeSpan time = TimeSpan.FromSeconds(currentTime);                    
        quizGameUI.TimerText.text = time.ToString("mm':'ss");   //converte il formato

        if (currentTime <= 0)
        {
            //Game Over
            GameEnd();
        }
    }

    //controlla se la risposta è corretta
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        bool correct = false;
  
        if (selectedQuetion.correctAns == selectedOption)
        {
            //risposta corretta
            correctAudio.Play();
            correctAnswerCount++;
            correct = true;
            gameScore += 1;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
        }
        else
        {
            //risposta sbagliata
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
                //seleeziona nuova domanda dopo un tot di tempo
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
       
        return correct;
    }

    //partita finita, apre il pannello game over
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



//sruttura domanda
[System.Serializable]
public class Question
{
    public string questionInfo;         
    public QuestionType questionType;  
    
    public List<string> options;     
    public string correctAns; 
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