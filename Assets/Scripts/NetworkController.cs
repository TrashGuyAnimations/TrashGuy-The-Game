using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public Button openCreateRoomModal;
    public InputField nameField;
    public GameObject roomOpts;
    public Button closeRoomModal;
    public InputField roomName;
    public InputField roomPassword;
    public Button createRoom;
    public Slider roomMaxPlayersSlider;
    public TextMeshProUGUI roomMaxPlayersText;
    public GameObject toHide;
    public GameObject toShow;
    public GameObject roomPrefab;
    public GameObject scrollView;
    private ScrollRect myScrollRect;
    public GameObject roomList;
    public GameObject joinRoomModal;
    public Text toastText;
    public InputField joinPassword;

    void ShowToast(string text,
        int duration)
    {
        toastText.gameObject.SetActive(true);
        StartCoroutine(ShowToastCoroutine(text, duration));
    }

    private IEnumerator ShowToastCoroutine(string text,
        int duration)
    {
        Color orginalColor = toastText.color;

        toastText.text = text;
        toastText.enabled = true;

        //Fade in
        yield return fadeInAndOut(toastText, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(toastText, false, 0.5f);

        toastText.enabled = false;
        toastText.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }

    void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();

        myScrollRect.verticalNormalizedPosition = 0f;

        Canvas.ForceUpdateCanvases();
    }

    void ChangeNickname(string newName)
    {
        PhotonNetwork.NickName = newName;
    }

    private void OpenCreateRoomModal()
    {
        roomOpts.SetActive(true);
    }

    private void CloseCreateRoomModal()
    {
        roomOpts.SetActive(false);
        roomName.text = "";
        roomPassword.text = "";
        roomMaxPlayersSlider.value = 4;
        roomMaxPlayersText.text = "4";
    }

    private void OnSliderValueChange(float newValue)
    {
        roomMaxPlayersText.text = ((int) newValue).ToString();
    }

    private void CreateNewRoom()
    {
        String nickName = PhotonNetwork.NickName != String.Empty
            ? PhotonNetwork.NickName
            : "Guest" + (Random.Range(0, 10000).ToString());
        PhotonNetwork.NickName = nickName;
        var text = roomName.text;
        String roomText = text != String.Empty ? text : nickName + "'s Room";
        String password = roomPassword.text;
        Debug.Log(password);
        int maxPlayer = (int) roomMaxPlayersSlider.value;
        if (maxPlayer < 2)
        {
            maxPlayer = 2;
        }

        var roomOptions = new RoomOptions
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte) maxPlayer,
            CustomRoomProperties = new Hashtable {{"password", password}, {"roomName", roomText}},
            CustomRoomPropertiesForLobby = new[] {"password", "roomName"}
        };
        PhotonNetwork.CreateRoom("Room #" + Random.Range(0, 10000).ToString(), roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a room");
        CreateNewRoom();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    private void JoinRoom(RoomInfo roomInfo)
    {
        if (joinPassword.text == roomInfo.CustomProperties["password"] as string)
        {
            if (PhotonNetwork.NickName == String.Empty)
            {
                PhotonNetwork.NickName = "Guest" + (Random.Range(0, 10000).ToString());
            }

            PhotonNetwork.JoinRoom(roomInfo.Name);
        }
        else
        {
            Debug.Log("Wrong password");
            ShowToast("Wrong Password", 2);
        }
    }

    private void OpenJoinRoomModal(RoomInfo roomInfo)
    {
        joinRoomModal.SetActive(true);
        var roomModalName = joinRoomModal.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        roomModalName.text = "Do you really want to join " + roomInfo.Name + " ?";
        var joinButton = joinRoomModal.transform.GetChild(1).GetComponent<Button>();
        if (roomInfo.CustomProperties["password"] as string != String.Empty)
        {
            joinRoomModal.transform.GetChild(2).GetComponent<InputField>().gameObject.SetActive(true);
        }

        joinButton.onClick.AddListener(delegate { JoinRoom(roomInfo); });
    }

    private void AddRoom(RoomInfo roomInfo)
    {
        Debug.Log(roomInfo);
        GameObject newRoom = Instantiate(roomPrefab, roomList.transform, false) as GameObject;
        var roomId = newRoom.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        var roomName = newRoom.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var roomPlayers = newRoom.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        roomId.text = roomInfo.Name;
        roomName.text = roomInfo.CustomProperties["roomName"] as string;
        roomPlayers.text = roomInfo.PlayerCount + "/" + (int) roomInfo.MaxPlayers;
        newRoom.GetComponent<Button>().onClick.AddListener(delegate { OpenJoinRoomModal(roomInfo); });
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to " + PhotonNetwork.CloudRegion);
        toShow.SetActive(true);
        toHide.SetActive(false);
        roomOpts.SetActive(false);
        joinRoomModal.SetActive(false);
        myScrollRect = scrollView.GetComponent<ScrollRect>();
        PhotonNetwork.JoinLobby();
        nameField.onEndEdit.AddListener(ChangeNickname);
        openCreateRoomModal.onClick.AddListener(OpenCreateRoomModal);
        closeRoomModal.onClick.AddListener(CloseCreateRoomModal);
        roomMaxPlayersSlider.onValueChanged.AddListener(OnSliderValueChange);
        createRoom.onClick.AddListener(CreateNewRoom);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room List is ");
        foreach (var roomInfo in roomList)
        {
            AddRoom(roomInfo);
            ScrollToBottom();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        toHide.SetActive(true);
        toShow.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
    }
}