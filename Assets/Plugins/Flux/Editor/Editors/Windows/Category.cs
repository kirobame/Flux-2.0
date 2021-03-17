using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Flux.Editor
{
    public class Category<T>
    {
        public Category(string name)
        {
            Name = name;
            
            subCategories = new List<Category<T>>();
            values = new List<T>();
        }
        
        //---[Data]-----------------------------------------------------------------------------------------------------/
        
        public string Name { get; private set; }
        public IEnumerable<T> Values => values;

        private List<T> values;
        private List<Category<T>> subCategories;
        
        //---[Resource handling]----------------------------------------------------------------------------------------/

        public void Add(T value) => values.Add(value);
        public void Add(Category<T> subCategory) => subCategories.Add(subCategory);
        
        public bool TryGet(string subSource, out Category<T> output)
        {
            var splitSubSource = subSource.Split('/').ToList();
            splitSubSource.Insert(0, Name);
            
            return TryGet(splitSubSource.ToArray(), 0, out output);
        }
        private bool TryGet(string[] splitSubSource, int index, out Category<T> output)
        {
            if (splitSubSource[index] == Name)
            {
                if (index == splitSubSource.Length - 1)
                {
                    output = this;
                    return true;
                }
                
                foreach (var subCategory in subCategories)
                {
                    if (!subCategory.TryGet(splitSubSource, index + 1, out output)) continue;
                    return true;
                }

                index++;
                var previous = this;
                var current = new Category<T>(splitSubSource[index]);
                
                previous.Add(current);

                for (var i = index + 1; i < splitSubSource.Length; i++)
                {
                    previous = current;
                    current = new Category<T>(splitSubSource[i]);
                    
                    previous.Add(current);
                }

                output = current;
                return true;
            }

            output = null;
            return false;
        }
        
        //---[Recursion accesses]---------------------------------------------------------------------------------------/
        
        public void Relay(Action<Category<T>,int> predicate, int depth, bool use = true)
        {
            if (use) predicate(this, depth);
            foreach (var category in subCategories) category.Relay(predicate, depth + 1);
        }
    }
}