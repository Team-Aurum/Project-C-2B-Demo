using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverListeners : MonoBehaviour
{
    public Button tryAgain;
    public Button quit;
    public AudioSource audioSource;
    public AudioClip loopAudio;
    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {
        tryAgain = GameObject.Find("Try Again").GetComponent<Button>();
        quit = GameObject.Find("Quit").GetComponent<Button>();
        audioSource = GameObject.Find("Canvas").GetComponent<AudioSource>();
        int stemPosition = audioSource.clip.name.IndexOf("Start");
        string name = audioSource.clip.name.Substring(0, stemPosition);
        Debug.Log(name + "Loop");
        loopAudio = Resources.Load<AudioClip>("Music/" + name + "Loop");

        tryAgain.onClick.AddListener(delegate{Restart();});
        quit.onClick.AddListener(delegate{Quit();});
        coroutine = PlayAudioLoop();
        StartCoroutine(coroutine);
    }

    public void Restart(){
        coroutine = FadeOutToLoad();
        StartCoroutine(coroutine);
        // SceneManager.LoadScene("battle");
    }

    public void Quit(){
        Debug.Log("Reached Statement");
        Application.Quit();
    }

    IEnumerator PlayAudioLoop(){
        yield return new WaitForSeconds(audioSource.clip.length);
        Debug.Log("Playing Loop Audio");
        audioSource.clip = loopAudio;
        audioSource.Play();
    }

    IEnumerator FadeOutToLoad(){
        while(audioSource.volume > 0){
            audioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene("battle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
