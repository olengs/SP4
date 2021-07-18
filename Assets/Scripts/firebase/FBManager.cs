using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FBManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public static DatabaseReference DBreference;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    private void Awake()
    {
        PlayerPrefs.SetString("UserID", "");
    }
    // Start is called before the first frame update
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitialiseFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: {0}" + dependencyStatus);
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void InitialiseFirebase()
    {
        Debug.Log("Set up FB Auth");
        // set up auth instance obj
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }
    public IEnumerator Login(string _email, string _password)
    {
        // call FB auth signin func passing the email and pw
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        // wait till it done
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if(LoginTask.Exception!=null)
        {
            // if there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "MissingEmail";
                    break;
                case AuthError.MissingPassword:
                    message = "MissingPassword";
                    break;
                case AuthError.WrongPassword:
                    message = "WrongPassword";
                    break;
                case AuthError.InvalidEmail:
                    message = "InvalidEmail";
                    break;
                case AuthError.UserNotFound:
                    message = "UserNotFound";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // user login..get result
            user = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "";

            yield return new WaitForSeconds(1);

            PlayerPrefs.SetString("UserID", user.UserId);
            StartCoroutine(UpdateUsernameAuth(user.DisplayName));
            StartCoroutine(UpdateUsernameDatabase(user.DisplayName));

            SceneManager.LoadScene("MainMenu");
            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
        }
    }

    private IEnumerator Register(string _email,string _password,string _username)
    {
        if(_username=="")
        {
            warningRegisterText.text = "Missing Username";
        }    
        else if(passwordRegisterField.text!=passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password does not match";
        }
        else
        {
            // call fb auth signin func passing email and pw
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            // wait till complete
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if(RegisterTask.Exception!=null)
            {
                //if got error handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register failed";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "MissingEmail";
                        break;
                    case AuthError.MissingPassword:
                        message = "MissingPassword";
                        break;
                    case AuthError.WeakPassword:
                        message = "WeakPassword";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "EmailAlreadyInUse";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                // user has now been created and get the result
                user = RegisterTask.Result;
                if(user!=null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    // call the fb auth update user prof func passing prof with username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    // wait till task is done
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if(ProfileTask.Exception!=null)
                    {
                        // if there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username set failed";
                    }
                    else
                    {
                        // username set..return login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        warningLoginText.text = "Register successful";
                        ClearRegisterFields();
                        ClearLoginFields();
                    }
                }
            }
        }

    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        // Create a user prof and set username
        UserProfile profile = new UserProfile { DisplayName = _username };

        // call the fb auth update user prof func passing the prof with the username
        var ProfileTask = user.UpdateUserProfileAsync(profile);
        // wait till task complete
        yield return new WaitUntil(predicate:()=>ProfileTask.IsCompleted); 

        if(ProfileTask.Exception!=null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            // auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(user.UserId).Child("username").SetValueAsync(_username);

        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // database username is now updated
        }
    }

    public static IEnumerator UpdateIntDatabase(List<string> key, int value)
    {
        // Set the currently logged in user username in the database
        var DBref = DBreference;
        foreach(string s in key)
        {
            DBref = DBref.Child(s);
        }
        var DBTask = DBref.SetValueAsync(value);

        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // database username is now updated
        }
    }

    public static IEnumerator UpdateStringDatabase(List<string> key, string value)
    {
        // Set the currently logged in user username in the database
        var DBref = DBreference;
        foreach (string s in key)
        {
            DBref = DBref.Child(s);
        }
        var DBTask = DBref.SetValueAsync(value);

        // wait till task complete
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // database username is now updated
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
