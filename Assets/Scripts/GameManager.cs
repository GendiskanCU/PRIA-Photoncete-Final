using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviour
{
    //Array con los posibles puntos de spwan de las frutas
    public GameObject [] fruits;


    // Start is called before the first frame update
    void Start()
    {
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
    private void InstantiateFruit()
    {
        if(PhotonNetwork.IsMasterClient) //Hay que ponerlo para que solo instancie la fruta el jugador Master. Si no, cada jugador que hubiera instanciaría una
        {
            //Instancia una fruta en la posición de uno de los swpanpoints elegido aleatoriamente (de cero hasta el número de spawn points que haya)
            int spawnPointFruit = Random.Range(0, fruits.Length);
            PhotonNetwork.Instantiate("Fruit", fruits[spawnPointFruit].transform.position, Quaternion.identity);
        }
        
    }
   
}
