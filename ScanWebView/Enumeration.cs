using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ScanWebView
{
    //References: https://www.codeproject.com/Articles/20805/Enhancing-C-Enums
    public abstract class Enumeration<TKey> : IComparable where TKey : IComparable
    {
        private readonly TKey key;
        private readonly string value;

        protected Enumeration()
        {
        }

        protected Enumeration(TKey key, string displayName)
        {
            this.key = key;
            value = displayName;
        }

        public TKey Key
        {
            get { return key; }
        }

        public string Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            return Value;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration<TKey>, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        public static IEnumerable<Enumeration<TKey>> GetAllByType(Type T)
        {
            var type = T;
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = Activator.CreateInstance(T);
                var locatedValue = (Enumeration<TKey>)info.GetValue(instance);

                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration<TKey>;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = key.Equals(otherValue.Key);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }



        public static T FromKey<T>(TKey value) where T : Enumeration<TKey>, new()
        {
            var matchingItem = Parse<T, TKey>(value, "value", item => item.Key.Equals(value));
            return matchingItem;
        }

        public static T FromValue<T>(string displayName) where T : Enumeration<TKey>, new()
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Value == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration<TKey>, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
                throw new ApplicationException(message);
            }

            return matchingItem;
        }

        public int CompareTo(object other)
        {
            return Key.CompareTo(((Enumeration<TKey>)other).Key);
        }
    }
}