using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks {
  public static Launcher Instance;

  [SerializeField] TMP_InputField playerNameInputField;
  [SerializeField] TMP_Text titleWelcomeText;
  [SerializeField] TMP_InputField roomNameInputField;
  [SerializeField] Transform roomListContent;
  [SerializeField] GameObject roomListItemPrefab;
  [SerializeField] TMP_Text roomNameText;
  [SerializeField] Transform playerListContent;
  [SerializeField] GameObject playerListItemPrefab;
  [SerializeField] GameObject startGameButton;
  [SerializeField] TMP_Text errorText;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    if (PhotonNetwork.IsConnected)
    {
      //Si el jugador está conectado al servidor es porque venimos de la escena del juego, por lo que
      //cargamos el panel de la sala
      ConfigureRoom();
    }
    else
    {
      //Si el jugador no está conectado realiza el proceso de conexión desde el principio
      Debug.Log("¡Conectando al máster!");
      PhotonNetwork.ConnectUsingSettings();
    }
  }

  public override void OnConnectedToMaster() {
    Debug.Log("¡Conectado al máster!");
    PhotonNetwork.JoinLobby();
    
    PhotonNetwork.AutomaticallySyncScene = true;
  }

  public override void OnJoinedLobby() {
    if (PhotonNetwork.NickName == "") {
      PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString(); // Ponemos un nickname al azar
      MenuManager.Instance.OpenMenu("name");
    } else {
      MenuManager.Instance.OpenMenu("title");
    }
    Debug.Log("Unido al lobby");
  }

  public void SetName() {
    string name = playerNameInputField.text;
    if (!string.IsNullOrEmpty(name)) {
      PhotonNetwork.NickName = name;
      titleWelcomeText.text = $"Bienvenido, {name}!";
      MenuManager.Instance.OpenMenu("title");
      playerNameInputField.text = "";
    } else {
      Debug.Log("No se ha introducido nombre");
     
    }
  }

  public void CreateRoom() {
    if (!string.IsNullOrEmpty(roomNameInputField.text)) {
      PhotonNetwork.CreateRoom(roomNameInputField.text);
      MenuManager.Instance.OpenMenu("loading");
      roomNameInputField.text = "";
    } else {
      Debug.Log("No se ha introducido habitación");
    
    }
  }

  public override void OnJoinedRoom()
  {
    ConfigureRoom();
  }

  private void ConfigureRoom()
  {
    MenuManager.Instance.OpenMenu("room");
    roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
    foreach (Transform trans in playerListContent) {
      Destroy(trans.gameObject);
    }
    for (int i = 0; i < players.Count(); i++) {
      Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
    }
    //Habilitamos el botón si el jugador está en la habitación
    startGameButton.SetActive(PhotonNetwork.IsMasterClient);
  }

  public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) {
    startGameButton.SetActive(PhotonNetwork.IsMasterClient);
  }

  public void LeaveRoom() {
    PhotonNetwork.LeaveRoom();
    MenuManager.Instance.OpenMenu("loading");
  }

  public void JoinRoom(RoomInfo info) {
    PhotonNetwork.JoinRoom(info.Name);
    MenuManager.Instance.OpenMenu("loading");
  }

  public override void OnLeftRoom() {
    MenuManager.Instance.OpenMenu("title");
  }

  public override void OnRoomListUpdate(List<RoomInfo> roomList) {
    foreach (Transform trans in roomListContent) {
      Destroy(trans.gameObject);
    }
    for (int i = 0; i < roomList.Count; i++) {
      if (roomList[i].RemovedFromList) {
     
        continue;
      }
      Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
    }
  }

  public override void OnCreateRoomFailed(short returnCode, string message) {
    errorText.text = "Falló al crear habitación: " + message;
    MenuManager.Instance.OpenMenu("error");
  }

  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
    Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
  }

  public void StartGame() {
    
    //Cargamos la escena que está en "buil settings" como 1
    PhotonNetwork.LoadLevel(1);
  }

  public void QuitGame() {
    Application.Quit();
  }
}
