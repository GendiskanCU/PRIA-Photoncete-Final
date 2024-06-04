using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //Array con los posibles puntos de spwan de las frutas
    public GameObject [] fruits;

    //Panel de fin del juego
    [SerializeField] private GameObject panelEndGame;
    [SerializeField] private TMP_Text textEndGame;

    //Puntuación necesaria para alcanzar la victoria
    [SerializeField] private int _scoreForVictory = 2;


    private string nameJug1, nameJug2;

    public string NameJug1 => nameJug1;
    public string NameJug2 => nameJug2;

    public int ScoreForVictory => _scoreForVictory;

    public static GameManager instance;


    private void Awake()
    {
        instance = this;
        
        //Guarda el nombre de los jugadores
        Dictionary<int, Photon.Realtime.Player> playerList = PhotonNetwork.CurrentRoom.Players;
        nameJug1 = playerList[1].NickName;
        nameJug2 = playerList[2].NickName;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Bloquea el cursor del ratón
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //La instanciación de cada personaje se hace con Photon        
        if(PhotonNetwork.IsMasterClient)//Si es el jugador 1, el master
        {
            PhotonNetwork.Instantiate("Frog", new Vector3(-11f, 2.5f, 0f), Quaternion.identity);            
        }
        else//Si es el jugador 2
        {
            PhotonNetwork.Instantiate("VirtualBoy", new Vector3(11f, 2.5f, 0f), Quaternion.identity);
        }

        NewFruit();//Instancia la primera fruta del juego
    }



    public void NewFruit()
    {
        //Espera 5 segundos y llama al método que instancia una fruta          
        Invoke("InstantiateFruit", 5.0f);
    }
    //Método que instancia una fruta en uno de los spawnpoints elegidos aleatoriamente
    private void InstantiateFruit()
    {
        if(PhotonNetwork.IsMasterClient) //Hay que ponerlo para que solo instancie la fruta el jugador Master. Si no, cada jugador que hubiera instanciaría una
        {
            //Instancia una fruta en la posición de uno de los swpanpoints elegido aleatoriamente (de cero hasta el número de spawn points que haya)
            int spawnPointFruit = Random.Range(0, fruits.Length);
            PhotonNetwork.Instantiate("Fruit", fruits[spawnPointFruit].transform.position, Quaternion.identity);
        }
        
    }

    //Método que muestra el panel de fin del juego, informando qué jugador ha vencido
    public void EndGame(string WinningPlayer)
    {
        textEndGame.text = "¡¡¡" + WinningPlayer + " ha vencido!!!";
        Time.timeScale = 0; //Detiene el juego
        //Desbloquea el cursor del ratón
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelEndGame.SetActive(true);
    }

    public void BtnExitGame()
    {
        //Restaura la velocidad normal del juego y regresa al menú
        Time.timeScale = 1;
        PhotonNetwork.LoadLevel(0);
    }
   
}
