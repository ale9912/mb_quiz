using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController PFC;

    private string userEmail;
    private string userPassword;
    private string username;
    public GameObject loginPanel;

    [Header("Screens")]
    public GameObject LoginPanel;
    public GameObject RegisterPanel;

    [Header("Login Screen")]
    public TMP_InputField LoginEmailField;
    public TMP_InputField LoginPasswordField;
    public Button LoginBtn;
    public Button RegisterBtn;

    [Header("Register Screen")]
    public TMP_InputField RegisterEmailField;
    public TMP_InputField RegisterUsernameField;
    public TMP_InputField RegisterPasswordwordField;
    public Button RegisterAccountBtn;
    public Button BackBtn;


    public void Start()
    {
        RegisterPanel.SetActive(false);
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "FD55A";
        }
        PlayerPrefs.DeleteAll();
       
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
        }

    }

    #region Login

    public void OnTryLogin()
    {
        string email = LoginEmailField.text;
        string password = LoginPasswordField.text;

        LoginBtn.interactable = false;

        LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest { Email = email, Password = password };
        PlayFabClientAPI.LoginWithEmailAddress(request, success =>
        {
            SceneManager.LoadScene("Menu");
            Debug.Log("Login Success");
        },
        error =>
        {
            Debug.Log("Error: " + error.ErrorMessage);
            LoginBtn.interactable = true;
        });

    }




    public void OnTryRegisterNewAccount()
    {
        BackBtn.interactable = false;
        RegisterAccountBtn.interactable = false;

        string email = RegisterEmailField.text;
        string displayName = RegisterUsernameField.text;
        string password = RegisterPasswordwordField.text;

        //crea la richiesta
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest
        {
            Email = email,
            DisplayName = displayName,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,
        success =>
        {
            BackBtn.interactable = true;
            RegisterAccountBtn.interactable = true;
            OpenLoginPanel();
            Debug.Log(success.PlayFabId);
        },
        error =>
        {
            BackBtn.interactable = true;
            RegisterAccountBtn.interactable = true;
            Debug.Log("Error: " + error.ErrorMessage);
        });
    }

    #endregion Login


    public void OpenLoginPanel()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void OpenRegistrationPanel()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

}


