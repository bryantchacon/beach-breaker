using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f); //static significa que la variable estara en cada uno de los caramelos, y private que solo uno de ellos usara el valor que tenga
    private static Candy previousSelected = null; //Caramelo seleccionado previamente    
    
    private SpriteRenderer spriteRenderer; //Variable que hace referencia a un componente del mismo game object en el que esta este script, cuando es asi, la variable siempre sera private
    //NOTA: Siempre que haya una variable asi, se debe inicializar en el Awake()
    private bool isSelected = false;

    public int id;

    private Vector2[] adjacentDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    }; //Variable enum tipo array(por []) 2D(por Vector2)

    private void Awake() //Se ejecuta antes del Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); //Inicializacion(obtencion del componente) de la variable
    }
    
    void Start() //Se ejecuta antes del primer fps
    {
        
    }
    
    void Update() //Se ejecuta cada fps
    {
        
    }
}