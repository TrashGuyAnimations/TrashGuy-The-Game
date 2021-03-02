using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Realtime;
using Photon.Pun;


public class FinishedLoading : MonoBehaviourPunCallbacks, IPunObservable
{

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Called me?"+ stream.IsWriting);
        if (stream.IsWriting)
        {
            var myParent = transform.parent;
            stream.SendNext(myParent ? myParent.GetComponent<PhotonView>().ViewID : -1);
            stream.SendNext(transform.localPosition.x);
            stream.SendNext(transform.localPosition.y);
            stream.SendNext(transform.position.x);
            stream.SendNext(transform.position.y);


        }
        else 

        {
            int parentId = (int)stream.ReceiveNext();
            transform.parent = PhotonView.Find(parentId)?.transform;
            float xLocal = (float)stream.ReceiveNext();
            float yLocal = (float)stream.ReceiveNext();
            float xSpace = (float)stream.ReceiveNext();
            float ySpace = (float)stream.ReceiveNext();
            transform.position = new Vector3(xSpace, ySpace,1);
            transform.localPosition = new Vector3(xLocal, yLocal,1);

        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        int[] ids = (int[])PhotonNetwork.CurrentRoom.CustomProperties["ids"];
        string[] letters = (string[])PhotonNetwork.CurrentRoom.CustomProperties["letters"];

        for (int i = 0; i < ids.Length; i++)
        {
            int id = ids[i];
            string letter = letters[i];
            if (id == photonView.ViewID)
            {
                transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = letter;
            }

        }
    }

}
