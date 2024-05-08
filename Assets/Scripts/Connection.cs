using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Incluimos librerías de Photon
using Photon.Pun;
using Photon.Realtime;

public class Connection : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        //Conexión al Master con los parámetros definidos
        PhotonNetwork.ConnectUsingSettings();
        //Activar la sincronización de escenas
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //Método para el botón de conectar al servidor
    public void ButtonConnect()
    {
        //Establece las opciones de la habitación a crear
        RoomOptions options = new RoomOptions() {MaxPlayers = 4};
        //Crea la habitación, o se une a ella si ya está creada
        PhotonNetwork.JoinOrCreateRoom("room1", options, TypedLobby.Default);
    }

    //Cuando se realiza la conexión al Master
    override public void OnConnectedToMaster()
    {
        Debug.Log("Conexión al Master realizada.");
    }

    //Cuando se produzca la entrada a una habitación o sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Conectado a la sala " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Hay... " + PhotonNetwork.CurrentRoom.PlayerCount + " jugadores");
    }

    private void Update() {
        //En el update se controla si se pasa a la siguiente escena cuando haya más de un jugador
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.LoadLevel(1);
            Destroy(this);
        }
    }
}
