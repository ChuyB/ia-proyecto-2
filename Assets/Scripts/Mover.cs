using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Prayer;
using static Unpacker;

class Mover : CharacterMover
{
    public GameObject chargePrefab;
    public GameObject player;
    public int detectionRadius;

    private Vector3 initialPosition;
    private Vector3 platformPosition;
    private PrefabCollector prefabCollector;
    private Vector3 burrowPosition;
    private StateMachine stateMachine;

    private GoToPlatformState goToPlatformState;
    private UnloadChargeState unloadChargeState;
    private ReturnToHomeState returnToHomeState;
    private PlayerNearbyState playerNearbyState;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);
        initialPosition = transform.position;
        prefabCollector = new PrefabCollector("Platform");
        platformPosition = prefabCollector.CollectPrefabPositions()[0];
        prefabCollector = new PrefabCollector("Safe Pillar");
        burrowPosition = prefabCollector.CollectPrefabPositions()[0];
        burrowPosition = new Vector3(Mathf.RoundToInt(burrowPosition.x), Mathf.RoundToInt(burrowPosition.y), 0);

        goToPlatformState = new GoToPlatformState(this);
        unloadChargeState = new UnloadChargeState(this);
        returnToHomeState = new ReturnToHomeState(this);
        playerNearbyState = new PlayerNearbyState(this);

        Transition platformToUnload = new Transition(unloadChargeState, () => transform.position == platformPosition);
        Transition unloadToReturn = new Transition(returnToHomeState, () => true);
        Transition homeToPlatform = new Transition(goToPlatformState, () => transform.position.x == initialPosition.x && transform.position.y == initialPosition.y);
        Transition anyToPlayerNearby = new Transition(playerNearbyState, () => PlayerIsNearby());
        Transition playerNearbyToPlatform = new Transition(goToPlatformState, () => transform.position == burrowPosition);

        goToPlatformState.AddTransition(platformToUnload);
        goToPlatformState.AddTransition(anyToPlayerNearby);
        unloadChargeState.AddTransition(unloadToReturn);
        unloadChargeState.AddTransition(anyToPlayerNearby);
        returnToHomeState.AddTransition(homeToPlatform);
        returnToHomeState.AddTransition(anyToPlayerNearby);
        playerNearbyState.AddTransition(playerNearbyToPlatform);

        GeneratePathMovement(platformPosition);

        stateMachine = new StateMachine(goToPlatformState);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public class GoToPlatformState : State
    {
        private Mover character;

        public GoToPlatformState(Mover character)
        {
            this.character = character;
        }

        public override List<Action> Enter()
        {
            character.GeneratePathMovement(character.platformPosition);
            return new List<Action>();
        }

        public override List<Action> Update()
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, character.path[character.currentStep].position, character.speed * Time.deltaTime);
            if (character.transform.position == character.path[character.currentStep].position) character.currentStep++;
            return new List<Action>();
        }
    }

    public class UnloadChargeState : State
    {
        private Mover character;

        public UnloadChargeState(Mover character)
        {
            this.character = character;
        }

        public override List<Action> Exit()
        {
            GameObject chest = GameObject.Instantiate(character.chargePrefab, character.transform.position, Quaternion.identity);
            chest.tag = "Box";
            return new List<Action>();
        }
    }

    public class ReturnToHomeState : State
    {
        private Mover character;

        public ReturnToHomeState(Mover character)
        {
            this.character = character;
        }

        public override List<Action> Enter()
        {
            character.GeneratePathMovement(character.initialPosition);
            return new List<Action>();
        }

        public override List<Action> Update()
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, character.path[character.currentStep].position, character.speed * Time.deltaTime);
            if (character.transform.position == character.path[character.currentStep].position) character.currentStep++;
            return new List<Action>();
        }
    }
    public class PlayerNearbyState : State
    {
        private Mover character;

        public PlayerNearbyState(Mover character)
        {
            this.character = character;
        }

        public override List<Action> Enter()
        {
            character.GeneratePathMovement(character.burrowPosition);
            return new List<Action>();
        }

        public override List<Action> Update()
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, character.path[character.currentStep].position, character.speed * Time.deltaTime);
            if (character.transform.position == character.path[character.currentStep].position) character.currentStep++;
            return new List<Action>();
        }
    }

    public bool PlayerIsNearby()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= detectionRadius;
    }
}
