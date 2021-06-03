using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager sharedInstance;
    public Text scoreText, movesText; //Variables que referencian los textos de la UI en el editor
    private int score; //Variable privada del score con variable autocomputada, esto es que el valor de la variable original esta encapsulado junto con algun metodo que se requiere que se ejecute automaticamente, en este caso actualizara la UI automaticamente
    private int moves;
    
    //Variable autocomputada de score, se llama igual pero con la primer letra en mayuscula, se aumenta desde otro script
    public int Score
    {
        get //Cuando pide el valor de Score...
        {
            return score; //... retorna el de score que es la variable privada
        }
        set //Reasigna el valor de score...
        {
            score = value; //... por el value que tome Score, porque score es el value de Score...
            scoreText.text = "Score: " + score; //... y atomaticamente actualiza el score en la UI
        }
    }

    public int Moves
    {
        get
        {
            return moves;
        }
        set
        {
            moves = value;
            movesText.text = "Moves: " + moves;
            if (moves <= 0)
            {
                moves = 0;
                StartCoroutine(GameOver());
            }
        }
    }

    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        score = 0; //Al iniciar el juego score esta en 0
        moves = 30;
        scoreText.text = "Score: " + score; //Indica que al iniciar el juego el score en la UI sera el que se indica aqui en el start
        movesText.text = "Moves: " + moves;
    }
    
    private IEnumerator GameOver()
    {
        yield return new WaitUntil(() => BoardManager.sharedInstance.isShifting == false); //Espera hasta(por WaitUntil(() => *codigo*)) que isShifting en el BoardManager sea false...
        yield return new WaitForSeconds(0.25f);

        //TODO: Invocar la pantalla del game over
    }
}