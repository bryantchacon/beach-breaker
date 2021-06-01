using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f); //static significa que la variable sera compartida por todos los candy, o sea, que su valor sera el mismo en cada uno de los caramelos, y private que solo cada uno de ellos usara el valor que tenga
    private static Candy previousSelected = null; //Se refiere al primer candy seleccionado

    private SpriteRenderer spriteRenderer; //Variable que hace referencia a un componente del mismo game object en el que esta este script, cuando es asi, la variable siempre sera private. NOTA: Siempre que haya una variable asi, se inicializa en el Awake()
    private bool isSelected = false;

    public int id;

    private Vector2[] adjacentDirections = new Vector2[] //Enum tipo array(por []) 2D(por Vector2)
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    private void Awake() //Se ejecuta antes del Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); //Inicializacion(obtencion del componente) de la variable
    }

    private void SelectCandy()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Candy>(); //Guarda el script del primer candy seleccionado
    }

    private void DeselectCandy()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }

    private void OnMouseDown() //Funcion de unity. Al hacer tap/clic en este candy...
    {
        if (spriteRenderer.sprite == null || BoardManager.sharedInstance.isShifting) //... si donde se hace tap/clic no hay imagen o se estan intercambiando los candys...
        {
            return; //... no hara nada
        }
        //Como no hace nada, despues de aqui el codigo no sigue

        if (isSelected) //Si este candy esta seleccionado...
        {
            DeselectCandy(); //... se deselecciona al dar tap/clic en este mismo candy...
        }
        else //Si no...
        {
            if (previousSelected == null) //... si previousSelected esta vacio, o sea, no hay ningun candy seleccionado...
            {
                SelectCandy(); //... se selecciona este candy al darle tap/clic al mismo...
            }
            else //Si no, si previousSelected si tiene algo, o sea, si hay un candy seleccionado, pero se le da clic a otro...
            {                
                if (CanSwipe()) //... si segun CanSwipe() el primero seleccionado esta en la lista de los que rodean el segundo seleccionado, o sea, esta a lado de el...
                {
                    SwapSprite(previousSelected); //... intercambia el sprite de los candys recibiendo como parametro el previousSelected, o sea, el primer candy seleccionado, esta funcion se activa al darle clic/tap al segundo candy(SwapSprite primero checa que el segundo seleccionado no sea igual al primero seleccionado para asi poder hacer el swipe)...
                    previousSelected.FindAllMatches(); //... busca y elimina los candys que hagan match si el PRIMERO seleccionado es el que lo provoca
                    previousSelected.DeselectCandy(); //... el primer candy seleccionado se deselecciona
                    FindAllMatches(); //... busca y elimina los candys que hagan match si el SEGUNDO seleccionado es el que lo provoca        
                }
                else //Si no, si ninguno se puede intercambiar por el, segun la lista donde seguarda(GetAllNeighbors()) o por que los caramelos estan en las esquinas que rodean al candy o lejos de el...
                {
                    previousSelected.DeselectCandy();
                    SelectCandy();
                }
            }
        }
    }

    public void SwapSprite(Candy firstCandySelected) //Intercambiar candys
    {
        if (spriteRenderer.sprite == firstCandySelected.GetComponent<SpriteRenderer>().sprite) //Si el sprite del segundo candy seleccionado(este candy) es igual al del primero seleccionado...
        {
            return; //... no hara nada
        }
        //Como no hace nada, despues de aqui el codigo no sigue

        //Intercambio de sprites
        Sprite tempSprite = firstCandySelected.spriteRenderer.sprite; //Variable auxiliar que almacena el sprite del primer candy seleccionado
        firstCandySelected.spriteRenderer.sprite = this.spriteRenderer.sprite; //El sprite del primer candy seleccionado sera el de este, o sea, el del segundo candy seleccionado
        this.spriteRenderer.sprite = tempSprite; //El sprite de este candy(el segundo seleccionado), sera el del primero, guardado en la variable tempSprite

        //Intercambio de ids
        int tempId = firstCandySelected.id; //Variable auxiliar que almacena el id del primer candy seleccionado
        firstCandySelected.id = this.id; //El id del primer candy seleccionado sera el de este, o sea, el del segundo candy seleccionado
        this.id = tempId; //El id de este candy(el segundo seleccionado), sera el del primero, guardado en la variable tempId        
    }

    //NOTA: Antes del siguiente codigo, para evitar que el raycast identifique primero al propio candy, o sea, una autocolision, se va a Edit -> Project Settings -> Physics 2D y se desactiva la casilla "Queries Start in Colliders"
    private GameObject GetNeighbor(Vector2 direction) //Funcion que obtiene el vecino, con direction(variable local y 2D por Vector2) como parametro de la direccion en la que se buscara. Debido a su tipo de dato, la funcion retornara un game object
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction); //Traza un rayo desde este candy en determinada direccion y en la variable hit guarda(por su tipo de dato) el collider del candy que detecte

        if (hit.collider != null) //Si hit.collider es diferente de null, o sea, que si detecto un candy...
        {
            return hit.collider.gameObject; //... retorna el game object del collider que detecto, o sea, el candy que detecto
        }
        else //Si no detecto nada...
        {
            return null; //... retorna null
        }
    }

    private List<GameObject> GetAllNeighbors() //Funcion que obtiene una lista de los candys vecinos, se llama en CanSwipe()
    {
        List<GameObject> neighbors = new List<GameObject>(); //Variable de la lista de los candys vecinos que guardara, se declara vacia al inicio

        foreach(Vector2 direction in adjacentDirections) //Para cada direction(variable local) en cada adjacentDirections(enum tipo array 2D en este script). NOTA: En los foreach, la primer variable de los parametros es local, y se refiere al singular de la segunda variable, que sera plural, o sea, que contenga varios elementos
        {
            neighbors.Add(GetNeighbor(direction)); //Ejecuta la funcion GetNeighbor()(con el direction de esta funcion como parametro), y agrega a la lista el candy vecino que encuentre en cada direccion
        }

        return neighbors; //Y retorna la lista de game objects de neighbors
    }
    
    private bool CanSwipe() //Funcion que indica con true o false si los candys se pueden intercambiar
    {
        return GetAllNeighbors().Contains(previousSelected.gameObject); //Checa si la lista que retorna GetAllNeighbors() contiene el primer candy seleccionado, si si lo contiene retorna true, si no, false
    }

    private List<GameObject> FindMatch(Vector2 direction) //Funcion para encontrar coincidencias de candys, guardarlos y retornar una lista que los contenga, se llama en ClearMatch(). El parametro es la direccion en un solo sentido en la que se buscaran los vecinos(el parametro que se le pasa es por medio de ClearMatch al especificarlos en las variables bools de FindAllMatches)
    {
        List<GameObject> matchingCandies = new List<GameObject>(); //Lista vacia donde se guardaran las coincidencias

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction); //Detecta y guarda el game object con el que choca el raycast

        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite) //Mientras haya detectado un collider y el sprite de ese sea igual al de este, o sea, que hay un candy vecino y ese candy tiene la misma imagen que este...
        {
            matchingCandies.Add(hit.collider.gameObject); //... agrega el game object del candy detectado a la lista matchingCandies...
            hit = Physics2D.Raycast(hit.collider.transform.position, direction); //... y vuelve a trazar un rayo pero esta vez desde el candy detectado(el vecino) para detectar si hay otro candy ugual, usando la misma variable hit, porque esta se consulta en el while
        }
        
        return matchingCandies; //Devuelve la lista de los candys que coinciden
    }

    private bool ClearMatch(Vector2[] directions) //"Elimina" los candys que coincidan(desactiva sus sprites), retornara true o false y por parametro se le pasa un array de direcciones 2d(referente a uno de los 4 puntos cardinales, los parametros se le pasan al usar ClearMatch() en las variables bools de la funcion FindAllMatches)
    {
        List<GameObject> matchignCandies = new List<GameObject>(); //Lista donde se guardaran los candys que coincidan. NOTA: No importa que sea igual a la de FindMatch() porque estan en funciones diferentes

        foreach (Vector2 direction in directions) //Para cada direction en directions...
        {
            matchignCandies.AddRange(FindMatch(direction)); //Agrega a matchignCandies la lista de candys encontrados que devuelve FindMatch
        }

        if (matchignCandies.Count >= BoardManager.MinNeighborCandiesToMatch) //Si la cantidad de elementos en matchignCandies es mayor o igual a MinNeighborCandiesToMatch en BoardManager...
        {
            foreach (GameObject candy in matchignCandies) //Para cada candy en matchignCandies...
            {
                candy.GetComponent<SpriteRenderer>().sprite = null; //Obtiene el componente SpriteRenderer de cada candy y lo desactiva
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void FindAllMatches() //Activa el encontrar y eliminar las coincidencias de caramelos empezando por usar aqui la funcion ClearMatch() y dentro de ella la funcion FindMatch(). Esta funcion se activa desde cualquiera de los dos candys que hacen swipe
    {
        if (spriteRenderer.sprite == null) //Si el segundo caramelo seleccionado no tiene imagen...
        {
            return; //... no hara nada
        }

        bool hMatch = ClearMatch(new Vector2[2] { Vector2.left, Vector2.right }); //Busca y elimina los caramelos a los lados, y si lo hace retorna true

        bool vMatch = ClearMatch(new Vector2[2] { Vector2.up, Vector2.down }); //Busca y elimina los caramelos arriba y abajo, y si lo hace retorna true

        if (hMatch || vMatch) //Si cualquiera de los dos o ambos retornan true, o sea, si se eliminan candys en cualquiera de las direcciones o en ambas...
        {
            spriteRenderer.sprite = null; //... tambien desactiva el sprite del candy que hace match...
            StopCoroutine(BoardManager.sharedInstance.FindNullCandies()); //... por si acaso esta activa detiene la corutina que busca y baja los candys que quedan arriba de los que se eliminaron...
            StartCoroutine(BoardManager.sharedInstance.FindNullCandies()); //... y la inicia de nuevo para que vaya encontrando matches en otros lados al ir bajando los candys
        }
    }
}