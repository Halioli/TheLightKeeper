using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ChatBox : MonoBehaviour
{
    private static float FADE_IN_TIME = 0.1f;
    private static float FADE_OUT_TIME = 0.1f;
    private static float LETTER_DELAY = 0.035f;
    private static float DOT_DELAY = 0.4f;
    private static float COMMA_DELAY = 0.2f;

    private CanvasGroup chatCanvasGroup;
    private bool chatOpen;
    private string fullMssgText;
    private string currentMssgText = "";
    private List<string> textToShow;

    private int eventIndex = -1;
    private TutorialMessages.MessegeEventType eventType;
    private int eventID;
    private AudioSource audioSource;

    public delegate void ChatNextInput();
    public static event ChatNextInput OnChatNextInput;

    public delegate void ChatEventAction(int eventID);
    public static event ChatEventAction OnChatTutorialEvent;
    public static event ChatEventAction OnChatCameraEvent;

    public delegate void FinishedChatMessage();
    public static event FinishedChatMessage OnFinishChatMessage;
    public bool allTextShown;
    public int currentTextNum = 0;
    public TextMeshProUGUI mssgText;
    public GameObject duckFace;
    public GameObject astronautFace;
    public Transform buttonTransoform;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        chatCanvasGroup = GetComponent<CanvasGroup>();
        chatOpen = false;
        allTextShown = false;
    }

    private void Update()
    {
        if (chatOpen && Input.GetKeyDown(KeyCode.Space))
        {
            if (OnChatNextInput != null)
                OnChatNextInput();

            buttonTransoform.DOComplete();
            buttonTransoform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.25f, 3);

            mssgText.maxVisibleCharacters = fullMssgText.Length;

            NextText();
        }
    }

    private void OnEnable()
    {
        TutorialMessages.OnNewMessage += ShowChatBox;
        MessageItemToStorage.OnNewMessage += ShowChatBox;
    }

    private void OnDisable()
    {
        TutorialMessages.OnNewMessage -= ShowChatBox;
        MessageItemToStorage.OnNewMessage -= ShowChatBox;
    }

    private void ShowChatBox(string[] mssg, TutorialMessages.ChatEventData chatEventData)
    {
        //ParseText(mssg);
        textToShow = new List<string>(mssg);
        
        this.eventIndex = chatEventData.eventIndex;
        this.eventType = chatEventData.eventType;
        this.eventID = chatEventData.eventID;

        // Set canvas group to 1
        StartCoroutine("CanvasFadeIn", chatCanvasGroup);

        NextText();

        PlayerInputs.instance.isLanternPaused = true;
        //PlayerInputs.instance.
    }

    private void DisplayText()
    {
        // Display text
        StartCoroutine("ShowText");
    }

    private void NextText()
    {
        if (!allTextShown)
        {
            StopCoroutine("ShowText");
            allTextShown = true;
        }
        else
        {
            if (textToShow.Count > currentTextNum)
            {
                allTextShown = false;
                fullMssgText = textToShow[currentTextNum];

                DisplayText();
                
                if (currentTextNum == eventIndex)
                {
                    InvokeChatEvents();
                }

                currentTextNum++;
            }
            else
            {
                // Send Action
                if (OnFinishChatMessage != null)
                    OnFinishChatMessage();

                HideChat();
            }
        }
    }


    private void InvokeChatEvents()
    {
        switch (this.eventType)
        {
            case TutorialMessages.MessegeEventType.TUTORIAL:
                if (OnChatTutorialEvent != null) OnChatTutorialEvent(eventID);
                break;

            case TutorialMessages.MessegeEventType.CAMERA:
                if (OnChatCameraEvent != null) OnChatCameraEvent(eventID);
                break;

            default:
                break;
        }
    }


    private void HideChat()
    {
        // Set canvas group to 0
        StartCoroutine("CanvasFadeOut", chatCanvasGroup);

        PlayerInputs.instance.isLanternPaused = false;
    }

    private void ResetValues()
    {
        textToShow.Clear();
        currentTextNum = 0;
        TutorialMessages.tutorialOpened = false;
        chatOpen = false;
        allTextShown = false;
    }

    IEnumerator CanvasFadeIn(CanvasGroup canvasGroup)
    {
        Vector2 startVector = new Vector2(0f, 0f);
        Vector2 endVector = new Vector2(1f, 1f);

        for (float t = 0f; t < FADE_IN_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / FADE_IN_TIME;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;

        chatOpen = true;
    }

    IEnumerator CanvasFadeOut(CanvasGroup canvasGroup)
    {
        Vector2 startVector = new Vector2(1f, 1f);
        Vector2 endVector = new Vector2(0f, 0f);

        while (!allTextShown)
        {
            yield return null;
        }

        for (float t = 0f; t < FADE_OUT_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / FADE_OUT_TIME;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;

        ResetValues();
    }

    IEnumerator ShowText()
    {
        allTextShown = false;
        mssgText.text = fullMssgText;
        mssgText.maxVisibleCharacters = 0;
        for (int i = 0; i < fullMssgText.Length; i++)
        {
            mssgText.maxVisibleCharacters++;
            duckFace.transform.DOShakeRotation(LETTER_DELAY, 10, 10, 50);
            audioSource.pitch = Random.Range(0f, 2f);
            audioSource.Play();

            if (fullMssgText[i] == '.' || fullMssgText[i] == '!' || fullMssgText[i] == '?')
            {
                yield return new WaitForSeconds(DOT_DELAY);
            }
            else if (fullMssgText[i] == ',')
            {
                yield return new WaitForSeconds(COMMA_DELAY);
            }
            else
            {
                yield return new WaitForSeconds(LETTER_DELAY);
            }
            //yield return new WaitForSeconds(fullMssgText[i] == '.' ? DOT_DELAY : LETTER_DELAY);
        }
        allTextShown = true;
    }
}
