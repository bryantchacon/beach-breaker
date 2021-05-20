using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance; //Variable del singleton

    public List<Sprite> prefabs = new List<Sprite>(); //Variable tipo lista que contendra sprites, la lista se declara vacia ya que los sprites se asignaran en el editor
    public GameObject currentCandy; //Dulce seleccionado actualmente
    public int xSize, ySize; //Tamaño del tablero en "x" y "y"
    private GameObject[,] boardSize; //Array del tablero, es private porque solo se usara en este script, cualquiera variable que sea asi sera private
    public bool isShifting { get; set; } //Variable que evitara que se intercambien varios dulces a la vez y que solo sea de uno en uno, como nadie mas que este mismo manager cambiara su valor, llevara get y set
    
    void Start()
    {
        if (sharedInstance == null) //Creacion del singleton del BoardManager
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject); //Esto es por si por error hay mas de 1, solo funcione el que llego primero y los demas son destruidos
        }

        Vector2 offset = currentCandy.GetComponent<BoxCollider>().size; //offset es el borde que tendra el tablero y su valor sera el alto y ancho del currentCandy, el cual se sabe por .size por medio de la obtencion de su BoxCollider, es Vector2 porque los datos son valores en "x" y "y"
        CreateInitialBoard(offset);
    }

    private void CreateInitialBoard(Vector2 offset) //Crear tablero inicial
    {

    }
        
    void Update()
    {
        
    }
}