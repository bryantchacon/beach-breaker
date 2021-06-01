using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance; //Variable del singleton

    public List<Sprite> prefabs = new List<Sprite>(); //Variable tipo lista que contendra sprites, la lista se declara vacia ya que los sprites se asignaran en el editor, la variable aparecera como un desplegable donde primero se indicara el numero de elementos que tendra y al hacerlo los campos donde iran apareceran debajo
    public GameObject currentCandy; //Base donde ira el sprite de los caramelos(es el cuadro cafe 5_2 de los sprites)
    public int xSize, ySize; //Variables del tamaño del tablero en "x" y "y"

    private GameObject[,] candies; //Game object array del tablero, es private porque solo se usara en este script, cualquier variable que sea asi sera private

    public bool isShifting { get; set; } //Variable que evitara que se intercambien varios dulces a la vez y que solo sea de uno en uno, como nadie mas que este mismo manager cambiara su valor, llevara get y set

    private Candy selectedCandy;

    public const int MinNeighborCandiesToMatch = 2; //El nombre se refiere que para los match se cuenta el actual mas 2 vecinos, asi, el codigo funcionara con 3 candys o mas. Esta variable se accede desde el script Candy de cada candy. Las constantes inician en mayusculas
    
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
                newCandy.name = string.Format("Candy[{0}][{1}]", x, y); //Le da nombre a cada candy, format es para darle el formato que tiene entre "", los datos entre los [] son los que van fuera de las "" y separados con , en el orden en el que se escriben y son el valor de las variables "x" y "y" en cada interacion de los bucles

                do //Haz...
                {
                    idx = Random.Range(0, prefabs.Count); //... de manera aleatoria la eleccion del idx al elegir un numero entre 0 y el tamaño del array prefabs[]...
                } while ((x > 0 && idx == candies[x - 1, y].GetComponent<Candy>().id) || (y > 0 && idx == candies[x, y - 1].GetComponent<Candy>().id)); //... cuando "x" o "y" sean mayor a 0 y el idx generado anteriormente sea igual al del candy que esta a la izquierda o debajo del candy actual
                //El do while anterior empieza a funcionar a partir de la generacion del candy en la coordenada 0, 1, porque el primero en 0, 0 no tiene nada a su izquierda y debajo por ser el primer candy, en el el idx se genera de forma normal y a partir del 0, 1 se fuerza a que los candys adyascentes no coincidan por medio de sus ids, todo esto debido a que el board se genera en la 2da coordenada del plano cartesiano

                Sprite sprite = prefabs[idx]; //Guarda en la variable sprite, el sprite del candy seleccionado aleatoriamente por medio del idx
                newCandy.GetComponent<SpriteRenderer>().sprite = sprite; //Asigna el sprite elejido anteriormente al new candy generado, o sea, aqui se indica que en lugar de mostrar el sprite del curretCandy(referenciado en el editor), mostrara los del array prefabs[]
                newCandy.GetComponent<Candy>().id = idx; //Se obtiene el script de new candy(por que todos los candy lo tienen) y se le asigna el idx como nuevo id

                newCandy.transform.parent = this.transform; //Indica que el parent del new candy sera el game object al que esta añadido este script, o sea, el board manager, esto para que el board manager tenga un desplegable y en el se oculte el listado de todos los candys
                candies[x, y] = newCandy; //Agrega cada candy al array, las coordenadas son el valor de las variables "x" y "y" en cada interacion de los bucles
            }
        }
    }

    public IEnumerator FindNullCandies() //Corutina que encuentra y baja los candys a los espacios que no tengan, recorriendo el array que los contiene, se llama desde el script Candy de cada candy
    {
        for (int x = 0; x < xSize; x++) //Bucle para la coordenada x(numero de columna)
        {
            for (int y = 0; y < ySize; y++) //Bucle para la coordenada y(numero de fila)
            {
                if (candies[x, y].GetComponent<SpriteRenderer>().sprite == null) //Si el candy que se esta checando no tiene sprite...
                {
                    yield return StartCoroutine(MakeCandiesFall(x, y)); //... activa la corutina que hara caer los que esten arriba y...
                    break; //... finaliza la corrutina, esto es necesario indicarlo, si no, seguira
                }
            }
        }

        //Despues de haber caido y generado los nuevos candys, por cada candy en el board...
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                candies[x, y].GetComponent<Candy>().FindAllMatches(); //... se activa la funcion FindAllMatches() para ver si al caer los candys no se generan nuevos matches en otros lados
            }
        }
    }

    private IEnumerator MakeCandiesFall(int x, int yStart, float shiftDelay = 0.05f) //Corutina para hacer caer los candys en los espacios vacios que quedan cuando hay un match, yStart(fila) se refiere a que la corutina checara hacia arriba con x(columna) fija. Para que una de las variables de los parametros tenga un valor por default, este se indica en la  declaracion de la funcion y al llamarla no se indica de nuevo, por que ya tiene un valor
    {
        isShifting = true; //Vuelve isShifting true al inicio del codigo

        List<SpriteRenderer> renders = new List<SpriteRenderer>(); //Lista que guarda el espacio de los candys sin sprite
        int nullCandies = 0; //Numero de candys sin sprite

        for (int y = yStart; y < ySize; y++) //Bucle que recorrera la columna hacia arriba en busca de candys sin sprites...
        {
            SpriteRenderer spriteRenderer = candies[x, y].GetComponent<SpriteRenderer>(); //... guarda el candy actual y obtiene su SpriteRenderer

            if (spriteRenderer.sprite == null) //... checa si el candy no tiene sprite
            {
                nullCandies++; //... si no tiene, suma 1 a la variable nullCandies
            }

            renders.Add(spriteRenderer); //... y agrega el candy a la lista renders
        }

        for (int i = 0; i < nullCandies; i++) //Bucle que hace caer los caramelos. La cantidad de veces que se repetira sera el valor de nullCandies
        {
            yield return new WaitForSeconds(shiftDelay); //Espera determinados segundos(segun el parametro) antes de hacer caer otro candy

            for (int j = 0; j < renders.Count - 1; j++) //Bucle que baja el sprite de arriba. La cantidad de veces que se repetira sera el numero de elementos de renders(por .Count). El -1 es porque la numeracion de elementos inicia en 0
            {                
                renders[j].sprite = renders[j + 1].sprite; //Baja el sprite del candy de arriba
                renders[j + 1].sprite = GetNewCandy(x, ySize - 1); //Genera el sprite de un nuevo candy en los espacios vacios que quedan al bajar los candys. El -1 en ySize es para generar el de hasta arriba, porque si no quedaria fuera del grid
            }
        }

        isShifting = false; //Vuelve isShifting false al inicio del codigo
    }

    private Sprite GetNewCandy(int x, int y) //Funcion para generar un nuevo candy en un espacio vacio, como regresa el sprite de un candy la funcion es tipo sprite
    {
        List<Sprite> newPossibleCandies = new List<Sprite>(); //Lista que guardara los posibles candys
        newPossibleCandies.AddRange(prefabs); //Indica que los posibles candys seran los originales de la lista de prefabs agregandolos a la variable, esto en POO es copia, no referencia, por eso el contenido de newPossibleCandies no se asigna con =

        //Los siguientes ifs eliminan candys de newPossibleCandies, tomando como parametro los sprites adyacentes al candy, esto para que al elegir el nuevo candy, no coincida con los adyacentes
        if (x > 0) //Si x es mayor a 0...
        {
            newPossibleCandies.Remove(candies[x - 1, y].GetComponent<SpriteRenderer>().sprite); //... se elimina de newPossibleCandies el sprite que este a la IZQUIERDA del candy...
        }

        if (x < xSize - 1) //Si x es menor al tamaño de columnas(xSize)...
        {
            newPossibleCandies.Remove(candies[x + 1, y].GetComponent<SpriteRenderer>().sprite); //... se elimina de newPossibleCandies el sprite que este a la DERECHA del candy...
        }

        if (y > 0) //si y es mayor a 0...
        {
            newPossibleCandies.Remove(candies[x, y - 1].GetComponent<SpriteRenderer>().sprite); //... se elimina de newPossibleCandies el sprite que este a DEBAJO del candy...
        }

        return newPossibleCandies[Random.Range(0, newPossibleCandies.Count)]; //... y de los sprites que queden retorna uno de forma aleatoria que sera el que ira en el espacio vacio
    }
}