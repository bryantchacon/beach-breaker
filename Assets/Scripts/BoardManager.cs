using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance; //Variable del singleton

    public List<Sprite> prefabs = new List<Sprite>(); //Variable tipo lista que contendra sprites, la lista se declara vacia ya que los sprites se asignaran en el editor, la variable aparecera como un desplegable donde primero se indicara el numero de elementos que tendra y al hacerlo los campos donde iran apareceran debajo
    public GameObject currentCandy; //Base donde ira el sprite de los caramelos(es el cuadro cafe 5_2 de los sprites)
    public int xSize, ySize; //Variables del tamaño del tablero en "x" y "y"

    private GameObject[,] candies; //Game object array del tablero, es private porque solo se usara en este script, cualquiera variable que sea asi sera private

    public bool isShifting { get; set; } //Variable que evitara que se intercambien varios dulces a la vez y que solo sea de uno en uno, como nadie mas que este mismo manager cambiara su valor, llevara get y set

    private Candy selectedCandy;
    
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

        Vector2 offset = currentCandy.GetComponent<BoxCollider2D>().size; //offset es la distancia entre cada caramelo hacia arriba y hacia abajo, y su valor sera el alto y ancho del currentCandy, el cual se sabe por .size por medio de la obtencion de su BoxCollider, es Vector2 porque los datos son valores en "x" y "y"
        CreateInitialBoard(offset);
    }

    private void CreateInitialBoard(Vector2 offset) //Crear tablero inicial
    {
        candies = new GameObject[xSize, ySize]; //candies guardara un array de game object de determinadas columnas(x) y filas(y)

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;
        //Las coordenadas a partir de las cuales se va a inicializar el board seran las del game object en el que esta este script, en este caso un game object vacio

        int idx = -1; //Variable para gestionar el id de los candy, a las variables que aun no se saben que valor tendran se les asigna -1 por default

        //Bucle doble para generar los candys del board
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject newCandy = Instantiate(currentCandy, new Vector3(startX + (offset.x * x), startY + (offset.y * y), 0), currentCandy.transform.rotation); //Instancia un caramelo en tales coordenadas y con determinada rotacion(porque se multiplican por "x" y "y"?)
                newCandy.name = string.Format("Candy [{0}][{1}]", x, y); //Le da nombre a cada candy, format es para darle el formato que tiene entre "", los datos entre los [] son los que van fuera de las "" y separados con , en el orden en el que se escriben y son el valor de las variables "x" y "y" en cada interacion de los bucles

                do //Haz...
                {
                    idx = Random.Range(0, prefabs.Count); //... de manera aleatoria la eleccion del idx al elegir un numero entre 0 y el tamaño del array prefabs[]...
                } while ((x > 0 && idx == candies[x - 1, y].GetComponent<Candy>().id) || (y > 0 && idx == candies[x, y - 1].GetComponent<Candy>().id)); //... cuando "x" o "y" sean mayor a 0 y el idx generado anteriormente sea igual al del candy que esta a la izquierda o debajo del candy actual
                //El do while anterior empieza a funcionar a partir de la generacion del candy en la coordenada 0, 1, porque el primero en 0, 0 no tiene nada a su izquierda y debajo por ser el primer candy, en el el idx se genera de forma normal y a partir del 0, 1 se fuerza a que los candys adyascentes no coincidan por medio de sus ids, todo esto debido a que el board se genera en la 2da coordenada del plano cartesiano

                Sprite sprite = prefabs[idx]; //Guarda en la variable sprite, el sprite del candy seleccionado aleatoriamente por medio del idx
                newCandy.GetComponent<SpriteRenderer>().sprite = sprite; //Asigna el sprite elejido anteriormente al new candy generado, o sea, aqui se indica que en lugar de mostrar el sprite del curretCandy(referenciado en el editor), mostrara los del array prefabs[]
                newCandy.GetComponent<Candy>().id = idx; //Se obtiene el script de new candy(por que todos los candy lo tienen) y se le asigna el idx como nuevo id

                newCandy.transform.parent = transform; //Indica que el parent del new candy sera el game object al que esta añadido este script, o sea, el board manager, esto para que el board manager tenga un desplegable y en el se oculte el listado de todos los candys
                candies[x, y] = newCandy; //Agrega cada candy al array, las coordenadas son el valor de las variables "x" y "y" en cada interacion de los bucles
            }
        }
    }
}