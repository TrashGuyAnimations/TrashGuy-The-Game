using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;

public class MultiPlayerManager : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Start()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnStart() called by PUN. Now this client is in a room.");
        PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 1), Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            var (letters,ids) = GetComponent<SceneGenerator>().GenerateData();
            Hashtable hash = new Hashtable
            {
                { "letters", letters },
                { "ids", ids },
            };

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        }

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }
}
