using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAssistant : MonoBehaviour
{
    
    //Sonidos
    [SerializeField] private AudioClip soundUp;
    [SerializeField] private AudioClip soundDown;
    [SerializeField] private AudioClip soundLeft;
    [SerializeField] private AudioClip soundRight;
    [SerializeField] private AudioClip soundNoFruit;
    
    
    //Para controlar los sonidos que emite
    private AudioSource _audioSource;
    
    //Permite la búsqueda de la fruta
    private bool canSearch = true;
    
    //Referencia al player
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //Buscar la fruta
        if(Input.GetKeyDown(KeyCode.LeftShift) && canSearch)
        {
            canSearch = false;
            StartCoroutine("FruitSearch");
        }
    }
    
    
    //Método que busca e indica la posición de la fruta con respecto al personaje
    private IEnumerator FruitSearch()
    {
        GameObject fruit = GameObject.FindWithTag("Fruit");

        if (fruit != null)
        {

            float deltaX = fruit.transform.position.x - player.transform.position.x;
            float deltaY = fruit.transform.position.y - player.transform.position.y;

            Debug.Log(deltaY);

            if (deltaX > 0.2f)
            {
                Debug.Log("La fruta está hacia tu derecha");
                _audioSource.PlayOneShot(soundRight);
            }
            else if (deltaX < -0.2f)
            {
                Debug.Log("La fruta está hacia tu izquierda");
                _audioSource.PlayOneShot(soundLeft);
            }

            yield return new WaitForSeconds(1f);
            
            if (deltaY > 0.5f)
            {
                Debug.Log("La fruta está hacia arriba");
                _audioSource.PlayOneShot(soundUp);
            }
            else if (deltaY < -0.5f)
            {
                Debug.Log("La fruta está hacia abajo");
                _audioSource.PlayOneShot(soundDown);
            }
        }
        else
        {
            Debug.Log("Todavía no hay ninguna fruta en la escena");
            _audioSource.PlayOneShot(soundNoFruit);
        }

        yield return new WaitForSeconds(0.5f);
        canSearch = true;
    }
}
