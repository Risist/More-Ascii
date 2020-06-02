using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Blackboard allows different modules to work on the same data without knowing about each other
 */
namespace Ai
{

    /*
     * Blackboard that supports any type
     */
    public class GenericBlackboard
    {
        Dictionary<Type, Dictionary<string, object>> values = new Dictionary<Type, Dictionary<string, object>>();

        public delegate T InitializeMethod<T>();

        public BoxValue<T> InitValue<T>(string key, InitializeMethod<T> initializeMethod)
        {
            if (values.TryGetValue(typeof(T), out var hashtable))
            {
                if (hashtable.TryGetValue(key, out var value))
                {
                    return (BoxValue<T>) value;
                }
                else
                {
                    var newValue = new BoxValue<T>(initializeMethod());
                    hashtable.Add(key, newValue);
                    return newValue;
                }
            }
            else
            {
                hashtable = new Dictionary<string, object>();
                values.Add(typeof(T), hashtable);

                var newValue = new BoxValue<T>(initializeMethod());
                hashtable.Add(key, newValue);
                return newValue;
            }
        }
        public BoxValue<T> InitValue<T>(string key) where T : new()
        {
            if (values.TryGetValue(typeof(T), out var hashtable))
            {
                if (hashtable.TryGetValue(key, out var value))
                {
                    return (BoxValue<T>) value;
                }
                else
                {
                    var newValue = new BoxValue<T>(new T());
                    hashtable.Add(key, newValue);
                    return newValue;
                }
            }
            else
            {
                hashtable = new Dictionary<string, object>();
                values.Add(typeof(T), hashtable);

                var newValue = new BoxValue<T>(new T());
                hashtable.Add(key, newValue);
                return newValue;
            }
        }

        // returns null if value with key not found
        public BoxValue<T> GetValue<T>(string key)
        {
            if (values.TryGetValue(typeof(T), out var hashtable))
            {
                if (hashtable.TryGetValue(key, out var value))
                {
                    return (BoxValue<T>)value;
                }
            }

            return null;
        }
    }
}

