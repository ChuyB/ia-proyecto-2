using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Prayer : CharacterMover
{
    private int currentTargetIndex;
    private List<Vector3> targetPositions;
    private Vector3 initialPosition;
    private bool playerCollided;
    private StateMachine stateMachine;
    private PrefabCollector prefabCollector;

    private MoveState moveState;
    private WaitState waitState;
    private ResetState resetState;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        prefabCollector = new PrefabCollector("Monument");
        targetPositions = prefabCollector.CollectPrefabPositions();
        playerCollided = false;

        // Create states
        moveState = new MoveState(this);
        waitState = new WaitState(this);
        resetState = new ResetState(this);

        // Transitions
        Transition moveToWait = new Transition(waitState, () => transform.position == targetPositions[currentTargetIndex]);
        Transition waitToMove = new Transition(moveState, () => !waitState.Waiting);
        Transition anyToReset = new Transition(resetState, () => playerCollided);
        Transition resetToWait = new Transition(waitState, () => transform.position.x == initialPosition.x && transform.position.y == initialPosition.y);

        moveToWait.AddAction(() => StartCoroutine(WaitCoroutine(waitState)));

        moveState.AddTransition(moveToWait);
        moveState.AddTransition(anyToReset);
        waitState.AddTransition(waitToMove);
        waitState.AddTransition(anyToReset);
        resetState.AddTransition(resetToWait);

        stateMachine = new StateMachine(waitState);
    }

    // Update is called once per frame
    void Update()
    {
        List<Action> actions = stateMachine.Update();

        foreach (var action in actions)
        {
            action();
        }
    }

    public class MoveState : State
    {
        private Prayer character;
        public MoveState(Prayer character)
        {
            this.character = character;
        }

        public override List<Action> Update()
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, character.path[character.currentStep].position, character.speed * Time.deltaTime);
            if (character.transform.position == character.path[character.currentStep].position) character.currentStep++;
            return new List<Action>();
        }
    }

    public class WaitState : State
    {
        public bool Waiting;
        private Prayer character;

        public WaitState(Prayer character)
        {
            this.character = character;
            Waiting = false;
        }

        public override List<Action> Exit()
        {
            character.currentTargetIndex = (character.currentTargetIndex + 1) % character.targetPositions.Count;
            Vector2 targetPosition = character.targetPositions[character.currentTargetIndex];
            character.GeneratePathMovement(targetPosition);
            return new List<Action>();
        }
    }

    public class ResetState : State
    {
        private Prayer character;

        public ResetState(Prayer character)
        {
            this.character = character;
        }

        public override List<Action> Enter()
        {
            character.GeneratePathMovement(character.initialPosition);
            character.playerCollided = false;
            return new List<Action>();
        }

        public override List<Action> Exit()
        {
            character.currentTargetIndex = 0;
            return new List<Action>();
        }

        public override List<Action> Update()
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, character.path[character.currentStep].position, character.speed * Time.deltaTime);
            if (character.transform.position == character.path[character.currentStep].position) character.currentStep++;
            return new List<Action>();
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerCollided = true;
        }
    }

    private IEnumerator WaitCoroutine(WaitState waitState)
    {
        waitState.Waiting = true;
        yield return new WaitForSeconds(3);
        waitState.Waiting = false;
    }
}
