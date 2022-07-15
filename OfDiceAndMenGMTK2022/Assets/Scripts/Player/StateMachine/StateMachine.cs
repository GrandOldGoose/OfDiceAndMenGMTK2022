using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class StateMachine
    {
        #region Fields
        private PlayerBaseState _currentSubState;

        private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> _currentSubStateTransistions = new List<Transition>();
        private List<Transition> _currentSuperStateTransistions = new List<Transition>();
        private List<Transition> _currentTransistions = new List<Transition>();
        private List<Transition> _anyTransitions = new List<Transition>();

        private static List<Transition> _emptyTransitions = new List<Transition>(capacity: 0);
        #endregion



        #region Public Methods
        public void LogicUpdate()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                SetState(transition.To);
            }
            _currentSubState?.LogicUpdate();
        }

        public void PhysicsUpdate()
        {
            _currentSubState?.PhysicsUpdate();
        }

        public void SetState(PlayerBaseState state)
        {
            if (state == _currentSubState)
            {
                return;
            }

            _currentSubState?.OnExit();
            _currentSubState = state;

            //clear all transitions from old state.
            _currentTransistions.Clear();
            //Adds in all transitions from the current subState into _currentSubStateTransistions List.
            _transitions.TryGetValue(_currentSubState.GetType(), out _currentSubStateTransistions);
            if (_currentSubStateTransistions == null) { _currentSubStateTransistions = _emptyTransitions; }
            //Adds in all transitions from the current subState's superState into _currentSuperStateTransistions List.
            _transitions.TryGetValue(_currentSubState.GetType().BaseType, out _currentSuperStateTransistions);
            if (_currentSuperStateTransistions == null) { _currentSuperStateTransistions = _emptyTransitions; }
            //Add in both lists into _currentTransistions.
            _currentTransistions.AddRange(_currentSubStateTransistions);
            _currentTransistions.AddRange(_currentSuperStateTransistions);
            if (_currentTransistions == null) { _currentTransistions = _emptyTransitions; }


            _currentSubState.OnEnter();
        }

        public void AddTransition(PlayerBaseState from, PlayerBaseState to, Func<bool> predicate)
        {
            if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<Transition>();
                _transitions[from.GetType()] = transitions;
            }

            transitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(PlayerBaseState to, Func<bool> predicate)
        {
            _anyTransitions.Add(new Transition(to, predicate));
        }
        #endregion



        #region Private Methods
        private Transition GetTransition()
        {
            foreach (var transition in _anyTransitions)
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }

            foreach (var transition in _currentTransistions)
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }

            return null;
        }
        #endregion



        #region Nested Transiton Class Def
        private class Transition
        {
            public Func<bool> Condition { get; }
            public PlayerBaseState To { get; }

            public Transition(PlayerBaseState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }
        #endregion Region
    }
}