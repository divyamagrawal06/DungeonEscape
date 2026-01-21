
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    RollDie,
    SelectDice,
    Perform

}

public enum Abilities
{
    Attack,
    Spell,
    Heal
}

public class Player : MonoBehaviour
{
    public bool playerTurn;
    public PlayerState currentState;
    [SerializeField]
    private GameObject[] Die;

    [SerializeField]
    private GridSystem gridSystem;

    Animator animator;

    [SerializeField]
    private float speed;
    bool rolling;

    float timer;

    int actionType;

    int moveDice;
    int abilityDice;

    Vector3 target = Vector3.up * 100f;
    void Start()
    {
        foreach (GameObject Dice in Die)
            Dice.GetComponentInChildren<Button>().enabled = false;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerTurn)
        {
            switch (currentState)
            {
                case PlayerState.RollDie:
                    RollDie();
                    break;
                case PlayerState.SelectDice:
                    SelectDice();
                    break;
                case PlayerState.Perform:
                    Perform();
                    break;
            }
        }
    }

    void RollDie()
    {

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            Debug.Log(timer);
        }
        else
        {
            if (rolling)
            {
                foreach (GameObject Dice in Die)
                {
                    Dice.GetComponent<Animator>().SetBool("roll", false);
                    Dice.GetComponentInChildren<Button>().enabled = true;
                }
                moveDice = Random.Range(0, 3);
                abilityDice = Random.Range(0, 3);
                currentState = PlayerState.SelectDice;
                rolling = false;
            }
        }


    }
    void SelectDice()
    {

    }
    void Perform()
    {
        if (actionType == 0)
        {


            if (target != Vector3.up * 100f)
            {
                if (transform.position != target)
                    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                else
                {
                    currentState = PlayerState.RollDie;
                    gridSystem.updatePlayercell();
                    target = Vector3.up * 100f;
                    animator.SetInteger("type", -1);
                }


            }
            else
            {
                gridSystem.pathMatrix(2, (Paths)moveDice);
                target = gridSystem.tileSelection();
                if (target != Vector3.up * 100f)
                {
                    animator.SetInteger("type", 0);
                    animator.SetTrigger("perform");
                }
            }


        }
        else
        {
            animator.SetInteger("type", 1);
            animator.SetTrigger("perform");
            animator.SetInteger("action", abilityDice);
            currentState = PlayerState.RollDie;

        }
    }

    public void roll()
    {
        if (currentState == PlayerState.RollDie)
        {
            foreach (GameObject Dice in Die)
                Dice.GetComponent<Animator>().SetBool("roll", true);
            rolling = true;
            timer = 5f;
        }
    }
    public void selectDice(int type)
    {
        actionType = type;
        currentState = PlayerState.Perform;
    }

    public void AbilityEffect()
    {
        animator.SetInteger("type", -1);

    }

}
