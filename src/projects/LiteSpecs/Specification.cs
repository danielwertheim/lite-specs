using System;

namespace LiteSpecs
{
    public static class Specification
    {
        public static Specification<T> All<T>() => Specification<T>.All;

        public static Specification<T> Generic<T>(Func<T, bool> predicate, string reason)
            => new Specification<T>(predicate, reason);
    }

    public class Specification<T> : ISpecification<T>
    {
        public static readonly Specification<T> All = AllSpecification<T>.Create();

        private readonly Func<T, ISpecificationResult> _predicate;

        protected internal Specification(Func<T, bool> predicate, string reason)
            : this(i => predicate(i) ? SpecificationIs.Satisfied : SpecificationIs.NotSatisfied(reason)) { }

        protected internal Specification(Func<T, ISpecificationResult> predicate)
        {
            _predicate = predicate;
        }

        public ISpecificationResult Eval(T item)
            => _predicate(item);

        public static implicit operator Func<T, bool>(Specification<T> spec)
            => i => spec._predicate(i).IsSatisfied;

        public static Func<T, bool> operator !(Specification<T> spec)
            => i => !spec._predicate(i).IsSatisfied;
    }
}