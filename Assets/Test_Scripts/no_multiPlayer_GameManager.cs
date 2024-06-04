using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_multiPlayer_GameManager : MonoBehaviour
{
    //Array con los posibles puntos de spwan de las frutas
    public GameObject [] fruits;
    public GameObject fruitPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        NewFruit();//Instancia la primera fruta del juego
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void NewFruit()
    {
        //Espera 5 segundos y llama al método que instancia una fruta          
        Invoke("InstantiateFruit", 5.0f);
    }
    private void InstantiateFruit()
    {
        //Instancia una fruta en la posición de uno de los swpanpoints elegido aleatoriamente (de cero hasta el número de spawn points que haya)
            int spawnPointFruit = Random.Range(0, fruits.Length);
            Instantiate(fruitPrefab, fruits[spawnPointFruit].transform.position, Quaternion.identity);

    }
}
