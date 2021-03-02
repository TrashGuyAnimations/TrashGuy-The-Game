using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using WeCantSpell.Hunspell;
using TMPro;

public class WordChecker : MonoBehaviour
{   
    public int minLen = 3;
    WordList dictionary;
    [SerializeField] TextMeshProUGUI wordCreated;
    [SerializeField] TextMeshProUGUI alertBox;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(LoadResourceTextfile("en_GB"));

         dictionary = WordList.CreateFromStreams(LoadResourceTextfile("en_GB"), LoadResourceTextfile("aff"));

    }
    public MemoryStream LoadResourceTextfile(string path)
    {

        

        TextAsset targetFile = Resources.Load<TextAsset>(path);
        return new MemoryStream(targetFile.bytes);
    }

    void EmptyAlert()
    {
        alertBox.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (wordCreated.text.Length > 3)
            {
                bool ok = dictionary.Check(wordCreated.text);
                if (ok)
                {
                    alertBox.text = "Congrats! Your score is ??";
                    Time.timeScale = 0;
                }
                else
                {
                    alertBox.text = "The word is not real!";
                    Invoke("EmptyAlert", 2);

                }
            }
            else
            {
                alertBox.text = "Word has to be longer than 3 letters!";
                Invoke("EmptyAlert", 2);

            }

        }
    }
}
