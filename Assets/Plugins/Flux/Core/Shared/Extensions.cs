using System;
using System.Linq;
using UnityEngine.LowLevel;

namespace Flux
{
    public static class Extensions
    {
        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            foreach (var attribute in type.GetCustomAttributes(true))
            {
                if (attribute is T cast) return cast;
            }

            return null;
        }
        
        //---[Player loop]----------------------------------------------------------------------------------------------/
        
        public static PlayerLoopSystem InsertAt<T>(this PlayerLoopSystem loop, PlayerLoopSystem value)
        {
            InsertAt<T>(ref loop, value);
            return loop;
        }
        public static bool InsertAt<T>(ref PlayerLoopSystem loop, PlayerLoopSystem value)
        {
            if (loop.type == typeof(T))
            {
                if (loop.subSystemList == null) loop.subSystemList = new PlayerLoopSystem[] { value };
                else
                {
                    Array.Resize(ref loop.subSystemList, loop.subSystemList.Length + 1);
                    loop.subSystemList[loop.subSystemList.Length - 1] = value;
                }

                return true;
            }

            if (loop.subSystemList != null)
            {
                for (var i = 0; i < loop.subSystemList.Length; i++)
                {
                    var subLoop = loop.subSystemList[i];
                    if (InsertAt<T>(ref subLoop, value))
                    {
                        loop.subSystemList[i] = subLoop;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}