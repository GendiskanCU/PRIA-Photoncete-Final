using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviourPunCallbacks
{
    public float speed = 5;//Velocidad de movimiento
    public float jumForce = 200;//Fuerza de impulso de salto

    //Puntos del personaje
    private int puntos = 0;
    //Cuadro de texto en la UI donde se mostrarán las puntuaciones de cada jugador
    public TMP_Text puntuacionJugador1;
    public TMP_Text puntuacionJugador2;
    //Texto que se mostrará en el cuadro de texto de la UI
    private string textoPuntuacion;

    //Para controlar las físicas del personaje
    private Rigidbody2D rig;

    //Para controlar las animaciones del personaje
    private Animator anim;

    //Para controlar el giro del personaje
    private SpriteRenderer spriteRenderer;

    //Para controlar cuándo puede saltar el personaje
    private bool canJump;

    //Para controlar cuándo puede disparar el personaje
    private bool canShot;

    //Punto de disparo
    private GameObject shotPoint;
    //Posiciones del punto de disparo según hacia dónde mira el personaje
    [SerializeField] private Vector3 initialPositionshotPoint = new Vector3(0.51f, -0.61f, 0);
    [SerializeField] private Vector3 flipPositionshotPoint = new Vector3(-0.62f, -0.61f, 0);

    //Tiempo de espera entre disparos
    [SerializeField] private float timeUntilNewShoot = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PhotonView>().IsMine)//Solo captura los componentes cuando sean los míos
        {
            rig = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            shotPoint = transform.GetChild(0).gameObject;
            shotPoint.transform.localPosition = initialPositionshotPoint;

            //Se le asigna la cámara principal (hay que tener en cuenta que las cámaras no se sincronizan)
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = transform.position + (Vector3.up) + transform.forward * -10;
        }


        //Capturamos los dos TextMeshPro que mostrarán las puntuaciones
        puntuacionJugador1 = GameObject.Find("PuntosJugador1").GetComponent<TMP_Text>();
        puntuacionJugador2 = GameObject.Find("PuntosJugador2").GetComponent<TMP_Text>();
        //Llama al método que actualiza la puntuación en los cuadros de texto
        ActualizaTextoPuntuacion();


        //Permite el salto
        canJump = true;

        //Permite el disparo
        canShot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)//Solo habrá movimiento si son mis componentes
        {
            //Movimiento. Se le da velocidad, en ambos ejes porque si no se quedaría parado
            rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);

            //Giro del personaje
            //Se debe utilizar el método RPC de Photon para que la rotación se sincronice adecuadamente
            //Se evita que RPC se ejecute en cada frame con la segunda condición
            if (rig.velocity.x > 0.1f && spriteRenderer.flipX)
            {
                GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, false);
                //Recoloca el punto de disparo
                shotPoint.transform.localPosition = (shotPoint.transform.localPosition == initialPositionshotPoint) ? flipPositionshotPoint : initialPositionshotPoint;
            }

            else if (rig.velocity.x < -0.1f && !spriteRenderer.flipX)
            {
                GetComponent<PhotonView>().RPC("RotateSprite", RpcTarget.All, true);
                //Recoloca el punto de disparo
                shotPoint.transform.localPosition = (shotPoint.transform.localPosition == initialPositionshotPoint) ? flipPositionshotPoint : initialPositionshotPoint;
            }


            //Salto
            if (canJump && Input.GetButtonDown("Jump"))
            {
                rig.AddForce(transform.up * jumForce);
                canJump = false;
            }

            //Disparo. Se debe sincronizar en todos los clientes
            if (canShot && Input.GetButtonDown("Fire1"))
            {
                //Instancia el shuriken               
                GameObject shuriken = PhotonNetwork.Instantiate("Shuriken", shotPoint.transform.position, shotPoint.transform.rotation);
                //Lo lanza en la dirección adecuada según el giro del personaje
                if (spriteRenderer.flipX)
                    shuriken.GetComponent<Shuriken>().Launch(-1);
                else
                    shuriken.GetComponent<Shuriken>().Launch(1);

                canShot = false;
                //Permite disparar de nuevo cuando pase el tiempo especificado
                Invoke("AllowNewShot", timeUntilNewShoot);
            }

            //Animación
            anim.SetFloat("velocityX", Mathf.Abs(rig.velocity.x));
            anim.SetFloat("velocityY", rig.velocity.y);
        }
    }

    //Método que rota el sprite para uso con RPC
    [PunRPC]
    public void RotateSprite(bool rotate)
    {
        GetComponent<SpriteRenderer>().flipX = rotate;
    }


    //Método para controlar cuándo el personaje toca suelo, pudiendo saltar de nuevo
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Floor"))
            canJump = true;
    }

    //Método que permite disparar de nuevo al personaje
    private void AllowNewShot()
    {
        canShot = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Si el personaje colisiona con una fruta
        if (other.gameObject.CompareTag("Fruit"))
        {
            //Se destruye la fruta
            Destroy(other.gameObject);

            //Se aumenta la puntuación en 10 puntos por fruta recogida
            puntos += 10;

            Debug.Log("Puntos jugador " + gameObject.name + ": " + puntos);
            //Se actualiza el cuadro de texto con la nueva puntuación
            ActualizaTextoPuntuacion();

            //Se indica al GameManager que vuelva a instanciar una nueva fruta
            GameObject.Find("SceneManager").GetComponent<GameManager>().NewFruit();

        }
    }


    private void ActualizaTextoPuntuacion()
    {
        if (GetComponent<PhotonView>().IsMine)//Nos aseguramos de actuar solo sobre nuestros propios componentes/atributos
        {
            //Asigna a cada jugador el cuadro de texto del canvas que le corresponda para mostrar su puntuación
            //Utilizo el identificador ActorNumber para saber qué jugador soy
            int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            //Utiliza el cuadro de texto adecuado según quién sea el jugador
            switch (playerNumber)
            {
                case 1: //Si es el jugador 1
                    textoPuntuacion = "Jugador 1\n" + puntos.ToString() + " puntos";
                    puntuacionJugador1.text = textoPuntuacion;//Actualizo el texto en local
                    //Y luego actualizo el texto en red para que cambie también en el otro jugador (usando RPC)
                    photonView.RPC(nameof(CambiartextoPuntuacionRed), RpcTarget.AllBuffered, 1, textoPuntuacion);
                    break;

                case 2: //Si es el jugador 2
                    textoPuntuacion = "Jugador 2\n" + puntos.ToString() + " puntos";
                    puntuacionJugador2.text = textoPuntuacion;//Actualizo el texto en local
                    //Y luego actualizo el texto en red para que cambie también en el otro jugador (usando RPC)
                    photonView.RPC(nameof(CambiartextoPuntuacionRed), RpcTarget.AllBuffered, 2, textoPuntuacion);
                    break;
            }
        }
        
    }

    [PunRPC]
    private void CambiartextoPuntuacionRed(int jugador, string texto)
    {
        //Se recibe como primer parámetro el número de jugador al que hay que cambiar la puntuación y como segundo parámetro el texto a mostrar
        if (jugador == 1 && puntuacionJugador1)
        {
            puntuacionJugador1.text = texto;
        }
        if (jugador == 2 && puntuacionJugador2) //Nota: en los if he añadido la segunda condición para evitar un error que aparecía en consola de objetos nulos
        {
            puntuacionJugador2.text = texto;
        }
    }

}
