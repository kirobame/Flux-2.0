using System;

namespace Flux.Event
{
    public class WrapperArgs<T1> : EventArgs, IWrapper<T1>
    {
        public WrapperArgs(T1 argOne) => ArgOne = argOne;
        
        public T1 ArgOne { get; private set; }

        T1 IWrapper<T1>.Value => ArgOne;
    }

    public class WrapperArgs<T1,T2> : EventArgs
    {
        public WrapperArgs(T1 argOne, T2 argTwo)
        {
            ArgOne = argOne;
            ArgTwo = argTwo;
        }

        public T1 ArgOne { get; private set; }
        public T2 ArgTwo { get; private set; }
    }

    public class WrapperArgs<T1,T2,T3> : EventArgs
    {
        public WrapperArgs(T1 argOne, T2 argTwo, T3 argThree)
        {
            ArgOne = argOne;
            ArgTwo = argTwo;
            ArgThree = argThree;
        }

        public T1 ArgOne { get; private set; }
        public T2 ArgTwo { get; private set; }
        public T3 ArgThree { get; private set; }
    }

    public class WrapperArgs<T1,T2,T3,T4> : EventArgs
    {
        public WrapperArgs(T1 argOne, T2 argTwo, T3 argThree, T4 argFour)
        {
            ArgOne = argOne;
            ArgTwo = argTwo;
            ArgThree = argThree;
            ArgFour = argFour;
        }

        public T1 ArgOne { get; private set; }
        public T2 ArgTwo { get; private set; }
        public T3 ArgThree { get; private set; }
        public T4 ArgFour { get; private set; }
    }
}