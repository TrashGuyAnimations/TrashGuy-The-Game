using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class GrabLetters : MonoBehaviourPun
{
    // Start is called before the first frame update
    List<GameObject> currentCollisions = new List<GameObject>();
    bool touchedTrashbin = false;
    GameObject openedLetter;
    [SerializeField] TextMeshProUGUI wordCreated;
    bool firstItem = true;
    void Start()
    {
        wordCreated = GameObject.FindGameObjectWithTag("CreatedWord").GetComponent<TextMeshProUGUI>();
                enabled = photonView.IsMine;

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Letters")
        {
            currentCollisions.Add(col.gameObject);
        }
        if (col.gameObject.tag == "Trashbin")
        {
            touchedTrashbin = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Letters")
        {
            currentCollisions.Remove(col.gameObject);
        }

        if (col.gameObject.tag == "Trashbin")
        {
            touchedTrashbin = false;
        }

    }
    [PunRPC]
    public void DoIDestroy(int openedLetter)
    {
        //PhotonNetwork.Destroy(PhotonView.Find(openedLetter));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {

            if (openedLetter)
            {

                
                if (touchedTrashbin)
                {
                    int viewId = openedLetter.GetComponent<PhotonView>().ViewID;
                    openedLetter.SetActive(false) ;
                    PhotonNetwork.Destroy(openedLetter);
                    //photonView.RPC("DoIDestroy", RpcTarget.MasterClient, viewId);

                    if (firstItem)
                    {
                        wordCreated.text = "";
                        firstItem = false;
                    }
                    wordCreated.text += openedLetter.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;

                }
                else
                {
                    openedLetter.transform.parent = null;

                }
                currentCollisions.Clear();
                openedLetter = null;
            }
            else if (currentCollisions.Count > 0)
            {
                Debug.Log("currentCollisions.Count");
                Debug.Log(currentCollisions.Count);

                GameObject gameObject = currentCollisions[0];
                Debug.Log(gameObject.name);

                openedLetter = gameObject;

                gameObject.transform.parent = transform;
                if (transform.localScale.x < 0)
                {
                    gameObject.transform.localPosition = new Vector3(1.23f, 0.1f, 0);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(1.16f, 0.06f, 0);
                }
                Destroy(gameObject.transform.GetComponent<Rigidbody2D>());
                gameObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);

            }
        }

    }
}
