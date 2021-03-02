using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Realtime;
using Photon.Pun;
public class SetName : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = photonView.Owner.NickName;
    }

    // Update is called once per frame

}
