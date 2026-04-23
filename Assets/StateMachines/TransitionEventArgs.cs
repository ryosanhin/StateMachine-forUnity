namespace StateMachines.StateMachinesAsync{
    public readonly struct TransitionEventArgs<TEnum> where TEnum : System.Enum
    {
        public Transition Type{get;}
        public IState<TEnum> State{get;}

        public TransitionEventArgs(Transition type, IState<TEnum> state)
        {
            Type=type;
            State=state;
        }
    }
}