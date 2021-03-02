using UnityEngine;
using TMPro;
using Photon.Pun;

public class SceneGenerator:MonoBehaviour
{
    [SerializeField] float xMin = -5.02f;
    [SerializeField] float xMax = 9f;
    [SerializeField] float yMin = -3f;
    [SerializeField] float yMax = 0;
    [SerializeField] int letterCount = 9;
    public GameObject letter;
    string[] AllowedLetters = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    string[] Voyels = new string[5] { "A", "E", "I", "O", "U" };
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    public (string[], int[]) GenerateData()
    {
        string[] letters = new string[letterCount];
        int[] ids = new int[letterCount];
        for (int i = 0; i < letterCount; i++)
        {
            // TODO need voyels and no over each other
            string n;
            if (i % 3==0)
            {
                n = Voyels[Random.Range(0, Voyels.Length)];
            }
            else
            {
                n = AllowedLetters[Random.Range(0, AllowedLetters.Length)];
            }
            GameObject prefab = PhotonNetwork.Instantiate("Letters", new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 1), Quaternion.identity);
            letters[i] = n;
            ids[i] = prefab.GetComponent<PhotonView>().ViewID;
            Debug.Log("Instaniated");
            Debug.Log(prefab.GetComponent<PhotonView>().ViewID);

            prefab.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = n;
        }
        return (letters,ids);

    }

}
