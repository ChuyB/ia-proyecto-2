using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Unpacker : CharacterMover
{
    public GameObject player;
    public int detectionRadius;

    private Vector3 initialPosition;
    private Vector3 platformPosition;
    private PrefabCollector prefabCollector;
    private StateMachine stateMachine;
    private Vector3 burrowPosition;
    private bool unpacked;

    private CheckForPrefabState checkForPrefabState;
    private GoToPlatformState goToPlatformState;
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

        checkForPrefabState = new CheckForPrefabState(this);
        goToPlatformState = new GoToPlatformState(this);
        returnToHomeState = new ReturnToHomeState(this);
        playerNearbyState = new PlayerNearbyState(this);

        Transition platformToUnpack = new Transition(checkForPrefabState, () => transform.position == platformPosition);
        Transition unpackToPlatform = new Transition(returnToHomeState, () => unpacked);
        Transition homeToPlatform = new Transition(goToPlatformState, () => transform.position.x == initialPosition.x && transform.position.y == initialPosition.y);
        Transition anyToPlayerNearby = new Transition(playerNearbyState, () => PlayerIsNearby());
        Transition playerNearbyToPlatform = new Transition(goToPlatformState, () =>
        {
            Debug.Log(transform.position + " " + burrowPosition);
            return transform.position == burrowPosition;
        });

        goToPlatformState.AddTransition(platformToUnpack);
        goToPlatformState.AddTransition(anyToPlayerNearby);
        checkForPrefabState.AddTransition(unpackToPlatform);
        checkForPrefabState.AddTransition(anyToPlayerNearby);
        returnToHomeState.AddTransition(homeToPlatform);
        returnToHomeState.AddTransition(anyToPlayerNearby);
        playerNearbyState.AddTransition(playerNearbyToPlatform);

        GeneratePathMovement(platformPosition);

        stateMachine = new StateMachine(goToPlatformState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public class GoToPlatformState : State
    {
        private Unpacker character;

        public GoToPlatformState(Unpacker character)
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

    public class CheckForPrefabState : State
    {
        private Unpacker character;
        private float checkInterval = 3.0f; // Interval to check for prefab
        private float checkTimer = 0.0f;

        public CheckForPrefabState(Unpacker character)
        {
            this.character = character;
        }

        public override List<Action> Update()
        {
            checkTimer -= Time.deltaTime;
            if (checkTimer <= 0)
            {
                checkTimer = checkInterval;
                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(character.transform.position, character.GetComponent<BoxCollider2D>().size, 0);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Box"))
                    {
                        // Delete the prefab
                        GameObject.Destroy(hitCollider.gameObject);
                        character.unpacked = true;
                        return new List<Action>();
                    }
                }
            }
            return new List<Action>();
        }
    }

    public class ReturnToHomeState : State
    {
        private Unpacker character;

        public ReturnToHomeState(Unpacker character)
        {
            this.character = character;
        }

        public override List<Action> Enter()
        {
            character.GeneratePathMovement(character.initialPosition);
            return new List<Action>();
        }

        public override List<Action> Exit()
        {
            character.unpacked = false;
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
        private Unpacker character;

        public PlayerNearbyState(Unpacker character)
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
