using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    //Fuerza con la que saldrá impulsado el shuriken
    [SerializeField] float impulseForce = 10.0f;

    //Tiempo de vida del shuriken
    [SerializeField] float timeOfLife = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        //El shuriken se destruirá transcurrido el tiempo de vida especificado
        Invoke("DestroyShuriken", timeOfLife);
    }  

    /// <summary>
    /// Lanza el shuriken en la dirección especificada    ///
    /// <param name="direction"> dirección de impulso (1 para derecha ó -1 para izquierda)</param>
    /// </summary>    
    public void Launch(int direction)
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector3(1, 0, 0) * (impulseForce * direction), ForceMode2D.Impulse);
    }

    //Destruye el Shuriken
    private void DestroyShuriken()
    {
        Destroy(this.gameObject);
    }
}
