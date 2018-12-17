﻿using System;

namespace Functional
{
    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly MaybeValueWrapper _value;

        public T Value
        {
            get
            {
                if (HasNoValue)
                    throw new InvalidOperationException();
                return _value.Value;
            }
        }

        public static Maybe<T> None => new Maybe<T>();

        public bool HasValue => _value != null;

        public bool HasNoValue => !HasValue;

        private Maybe(T value)
        {
            _value = (object) value == null ? null : new MaybeValueWrapper(value);
        }

        public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);

        public static Maybe<T> From(T obj) => new Maybe<T>(obj);

        public static bool operator ==(Maybe<T> maybe, T value)
        {
            if (maybe.HasNoValue)
                return false;
            return maybe.Value.Equals(value);
        }

        public static bool operator !=(Maybe<T> maybe, T value) => !(maybe == value);

        public static bool operator ==(Maybe<T> first, Maybe<T> second) => first.Equals(second);

        public static bool operator !=(Maybe<T> first, Maybe<T> second) => !(first == second);

        public override bool Equals(object obj)
        {
            if (obj is T)
                obj = new Maybe<T>((T) obj);
            if (!(obj is Maybe<T>))
                return false;
            return Equals((Maybe<T>) obj);
        }

        public bool Equals(Maybe<T> other)
        {
            if (HasNoValue && other.HasNoValue)
                return true;
            if (HasNoValue || other.HasNoValue)
                return false;
            return _value.Value.Equals(other._value.Value);
        }

        public override int GetHashCode()
        {
            if (HasNoValue)
                return 0;
            return _value.Value.GetHashCode();
        }

        public override string ToString()
        {
            if (HasNoValue)
                return "No value";
            return Value.ToString();
        }

        private class MaybeValueWrapper
        {
            internal readonly T Value;

            public MaybeValueWrapper(T value)
            {
                Value = value;
            }
        }
    }
}
