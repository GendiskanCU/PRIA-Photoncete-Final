using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class no_multiPlayer_Player : MonoBehaviour
{
   public float speed = 5;
    public float jumForce = 200;
    
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

    [SerializeField] private GameObject shurikenPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
            rig = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            shotPoint = transform.GetChild(0).gameObject;
            shotPoint.transform.localPosition = initialPositionshotPoint;

            //Se le asigna la cámara principal (hay que tener en cuenta que las cámaras no se sincronizan)
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.position = transform.position + (Vector3.up) +transform.forward * -10;
        

        //Permite el salto
        canJump = true;

        //Permite el disparo
        canShot = true;
    }

    // Update is called once per frame
    void Update()
    {
        
            //Movimiento. Se le da velocidad, en ambos ejes porque si no se quedaría parado
            rig.velocity = (transform.right * speed * Input.GetAxis("Horizontal")) + (transform.up * rig.velocity.y);

            //Giro del personaje
            //Se debe utilizar el método RPC de Photon para que la rotación se sincronice adecuadamente
            //Se evita que RPC se ejecute en cada frame con la segunda condición
            if(rig.velocity.x > 0.1f && spriteRenderer.flipX)
            {
                RotateSprite(false);
                 
                 //Recoloca el punto de disparo
                 shotPoint.transform.localPosition = (shotPoint.transform.localPosition == initialPositionshotPoint ) ? flipPositionshotPoint : initialPositionshotPoint;
            }
               
            else if(rig.velocity.x < -0.1f && !spriteRenderer.flipX)
            {
                RotateSprite(true);
                //Recoloca el punto de disparo
                shotPoint.transform.localPosition = (shotPoint.transform.localPosition == initialPositionshotPoint ) ? flipPositionshotPoint : initialPositionshotPoint;
            }
                

            //Salto
            if (canJump && Input.GetButtonDown("Jump"))
            {
                rig.AddForce(transform.up * jumForce);
                canJump = false;
            }
                
            //Disparo. Se debe sincronizar en todos los clientes
            if(canShot && Input.GetButtonDown("Fire1"))
            { 
                //Instancia el shuriken               
                GameObject shuriken = Instantiate(shurikenPrefab, shotPoint.transform.position, shotPoint.transform.rotation);
                //Lo lanza en la dirección adecuada según el giro del personaje
                if(spriteRenderer.flipX)
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

    //Método que rota el sprite 
   
    public void RotateSprite(bool rotate)
    {
        GetComponent<SpriteRenderer>().flipX = rotate;       
    }

    
    //Método para controlar cuándo el personaje toca suelo, pudiendo saltar de nuevo
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Floor"))
            canJump = true;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Si el personaje colisiona con una fruta
        if (other.gameObject.CompareTag("Fruit"))
        {
            //Se destruye la fruta
            Destroy(other.gameObject);

            

            //Se indica al GameManager que vuelva a instanciar una nueva fruta
            GameObject.Find("no_multi_GameManager").GetComponent<no_multiPlayer_GameManager>().NewFruit();

        }
    }

    //Método que permite disparar de nuevo al personaje
    private void AllowNewShot()
    {
        canShot = true;
    }
}
