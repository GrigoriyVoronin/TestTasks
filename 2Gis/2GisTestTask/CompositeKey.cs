using System;

namespace _2GisTestTask
{
    public readonly struct CompositeKey<TId, TName>
    {
        public CompositeKey(TId id, TName name)
        {
            Id = id;
            Name = name;
        }

        public TId Id { get; }
        public TName Name { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public override bool Equals(object? obj)
        {
            return obj is CompositeKey<TId, TName> key && Equals(Id, key.Id) && Equals(Name, key.Name);
        }

        public static implicit operator CompositeKey<TId, TName>((TId Id, TName Name) key)
        {
            return new(key.Id, key.Name);
        }
    }
}