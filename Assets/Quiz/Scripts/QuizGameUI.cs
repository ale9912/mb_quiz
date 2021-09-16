using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizGameUI : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizManager quizManager; 
    [SerializeField] private CategoryBtnScript categoryBtnPrefab;
    [SerializeField] private GameObject scrollHolder;
    [SerializeField] private Text scoreText, timerText, finalScoreText,recordText,newRecord;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private GameObject gameOverPanel, mainMenu, gamePanel;
    [SerializeField] private Color correctCol, wrongCol, normalCol; 
    [SerializeField] private Text questionInfoText;                 
    [SerializeField] private List<Button> options;                  
    #pragma warning restore 649

    private float audioLength;          
    private Question question;          
    private bool answered = false;      

    public Text TimerText { get => timerText; }                     
    public Text ScoreText { get => scoreText; }                     
    public GameObject GameOverPanel { get => gameOverPanel; }  
    public Text FinalScoreText { get => finalScoreText;  }
    public Text RecordText { get => recordText; }
    public Text NewRecord { get => newRecord; }


    private void Start()
    {
        
        for (int i = 0; i < options.Count; i++)
        {
            Button localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        CreateCategoryButtons();

    }
   
    
    public void SetQuestion(Question question)
    {
        //imposta domanda
        this.question = question;
        
        //controlla il tipo di domanda (in questo caso solo testo, poi potremmo aggiungere foto, video ecc)
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                
                break;
         
        }

        questionInfoText.text = question.questionInfo; 

        
        List<string> ansOptions = ShuffleList.ShuffleListItems<string>(question.options);

        //imposta bottoni per le risposte
        for (int i = 0; i < options.Count; i++)
        {
            //testo dei bottoni (Risposte)
            options[i].GetComponentInChildren<Text>().text = ansOptions[i];
            options[i].name = ansOptions[i];    
            options[i].image.color = normalCol; 
        }

        answered = false;                       

    }

    public void ReduceLife(int remainingLife)
    {
        lifeImageList[remainingLife].color = Color.red;
    }

   
    /// <param name="btn">ref to the button object</param>
    void OnClick(Button btn)
    {
        if (quizManager.GameStatus == GameStatus.PLAYING)
        {
            
            if (!answered)
            {
               
                answered = true;
             
                bool val = quizManager.Answer(btn.name);

                
                if (val)
                {
                    //colora il bottone di verde
                    StartCoroutine(BlinkImg(btn.image));
                }
                else
                {
                    //colora di rosso
                    btn.image.color = wrongCol;
                }
            }
        }
    }


    void CreateCategoryButtons()
    {
        //crea n bottoni per n risposte della domanda
        for (int i = 0; i < quizManager.QuizData.Count; i++)
        {
            CategoryBtnScript categoryBtn = Instantiate(categoryBtnPrefab, scrollHolder.transform);
            categoryBtn.SetButton(quizManager.QuizData[i].categoryName, quizManager.QuizData[i].questions.Count);
            int index = i;
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, quizManager.QuizData[index].categoryName));
        }
    }

   
    private void CategoryBtn(int index, string category)
    {
        quizManager.StartGame(index, category); 
        mainMenu.SetActive(false);              
        gamePanel.SetActive(true);  
    }

    IEnumerator BlinkImg(Image img)
    {
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = correctCol;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
