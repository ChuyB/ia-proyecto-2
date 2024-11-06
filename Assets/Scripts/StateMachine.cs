using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class StateMachine
{
    private State initialState;
    private State currentState;

    public StateMachine(State initialState)
    {
        this.initialState = initialState;
        currentState = initialState;
    }

    public List<System.Action> Update()
    {
        Transition triggered = null;

        foreach (Transition transition in currentState.GetTransitions())
        {
            if (transition.IsTriggered())
            {
                triggered = transition;
                break;
            }
        }

        if (triggered != null)
        {
            State targetState = triggered.GetTargetState();
            List<System.Action> actions = currentState.Exit();
            actions.AddRange(triggered.GetActions());
            actions.AddRange(targetState.Enter());

            currentState = targetState;
            return actions;
        }
        else
        {
            return currentState.Update();
        }
    }
}

abstract class State
{
    protected List<Transition> transitions;

    public State()
    {
        transitions = new List<Transition>();
    }

    public virtual List<System.Action> Enter()
    {
        return new List<System.Action>();
    }
    public virtual List<System.Action> Exit()
    {
        return new List<System.Action>();
    }
    public virtual List<System.Action> Update()
    {
        return new List<System.Action>();
    }
    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }

    public List<Transition> GetTransitions()
    {
        return transitions;
    }
}

class Transition
{
    private State targetState;
    private System.Func<bool> condition;
    private List<System.Action> actions;

    public Transition(State targetState, System.Func<bool> condition)
    {
        this.targetState = targetState;
        this.condition = condition;
        this.actions = new List<System.Action>();
    }

    public bool IsTriggered()
    {
        return condition();
    }

    public State GetTargetState()
    {
        return targetState;
    }

    public List<System.Action> GetActions()
    {
        return actions;
    }

    public void AddAction(System.Action action)
    {
        actions.Add(action);
    }
}