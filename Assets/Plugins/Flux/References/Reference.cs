﻿using System;
using UnityEngine;

namespace Flux
{
    public abstract class Reference : MonoBehaviour
    {
        public Enum Flag => cachedFlag;
        public abstract object Value { get; }

        [SerializeField] private DynamicFlag flag;
        
        private Enum cachedFlag;
        private bool hasAwoke;
        
        void Awake()
        {
            cachedFlag = flag.Value;
            Repository.Register(Flag, Value);
        }

        void OnEnable()
        {
            if (!hasAwoke)
            {
                hasAwoke = true;
                return;
            }
            
            Repository.Register(Flag, Value);
        }
        void OnDisable() => Repository.Unregister(Flag);
    }
}