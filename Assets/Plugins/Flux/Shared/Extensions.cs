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

        //---[System loop]----------------------------------------------------------------------------------------------/

        internal static bool Insert(this UpdateRelay parent, out UpdateRelay output, string[] chain, int index = 0)
        {
            if (index == chain.Length)
            {
                output = null;
                return false;
            }

            output = parent.Childs.FirstOrDefault(child => child.Name == chain[index]);
            if (output == null)
            {
                output = new UpdateRelay(chain[index]);
                parent.Add(output);
            }

            if (Insert(output, out var subOutput, chain, index + 1)) output = subOutput;
            return true;
        }
        internal static bool TryFind(this UpdateRelay value, string name, out UpdateRelay result)
        {
            if (value.Name == name)
            {
                result = value;
                return true;
            }

            foreach (var child in value.Childs)
            {
                if (child.TryFind(name, out result)) return true;
            }

            result = null;
            return false;
        }
        internal static string Print(this UpdateRelay value, string output = "", int depth = 0)
        {
            for (var i = 0; i < depth; i++)
            {
                for (var j = 0; j < 3; j++) output += "-";
            }
            output += $"|{value.Name}\n";

            foreach (var child in value.Childs) output = child.Print(output, depth + 1);

            return output;
        }

        //---[Player loop]----------------------------------------------------------------------------------------------/
        
        internal static PlayerLoopSystem InsertAt<T>(this PlayerLoopSystem loop, PlayerLoopSystem value)
        {
            InsertAt<T>(ref loop, value);
            return loop;
        }
        internal static bool InsertAt<T>(ref PlayerLoopSystem loop, PlayerLoopSystem value)
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