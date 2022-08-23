using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class StateMachine<MainClass> where MainClass : MonoBehaviour
    {
        protected MainClass m_owner;
        public MainClass Owner { get { return m_owner; } }

        private BaseState m_currentState;
        public BaseState CurrentState { get { return m_currentState; } }
        private BaseState m_nextState;

        private Dictionary<int, BaseState> m_stateList = new Dictionary<int, BaseState>();
        ////////////////////////////////////////////////////////////////////////////////
        public StateMachine(MainClass _owner)
        {
            m_owner = _owner;
        }
        ////////////////////////////////////////////////////////////////////////////////
        public void RegisterState(BaseState _state)
        {
            m_stateList.Add(_state.GetID(), _state);
        }
        ////////////////////////////////////////////////////////////////////////////////
        public void Update()
        {
            if (m_nextState != null) //need to change new state
            {
                if (m_currentState != null)
                {
                    m_currentState.OnExit(m_owner);
                }

                m_currentState = m_nextState;

                m_currentState.OnEnter(m_owner);

                m_nextState = null; //remove ref to next state
            }

            if (m_currentState != null)
            {
                m_currentState.OnUpdate(m_owner);
            }
        }

        public void FixedUpdate() 
        {
            if (m_currentState != null)
            {
                m_currentState.OnFixedUpdate(m_owner);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change to state by it's ID with was registered
        /// </summary>
        public bool ChangeState(int _nextStateId)
        {
            BaseState nextState = m_stateList[_nextStateId];
            if (nextState != null)
            {
                return ChangeState(nextState);
            }
            return false;
        }

        protected bool ChangeState(BaseState _nextState)
        {
            if (m_currentState != _nextState)
            {
                m_nextState = _nextState;
                return true;
            }
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////
        public BaseState FindState(int _stateID)
        {
            if (m_stateList.ContainsKey(_stateID))
            {
                return m_stateList[_stateID];
            }
            return null;
        }
    }
}