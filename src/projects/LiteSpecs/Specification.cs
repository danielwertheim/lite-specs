using System;

namespace LiteSpecs
{
    public static class Specification
    {
        public static Specification<T> All<T>() => Specification<T>.All;

        public static Specification<T> Generic<T>(Func<T, SpecificationResult> predicate)
            => new Specification<T>(predicate);
    }

    public class Specification<T>
    {
        public static readonly Specification<T> All = AllSpecification<T>.Create();

        private readonly Func<T, SpecificationResult> _predicate;

        protected internal Specification(Func<T, SpecificationResult> predicate)
        {
            _predicate = predicate;
        }

        public SpecificationResult IsSatisfiedBy(T item)
            => _predicate(item);

        public static implicit operator Func<T, bool>(Specification<T> spec)
            => i => spec._predicate(i).IsSatisfied;

        public static Specification<T> operator !(Specification<T> spec) => new Specification<T>(i =>
        {
            var r = spec._predicate(i);

            if(!r.IsSatisfied)
                return SpecificationResult.Satisfied;

            return SpecificationResult.NotSatisfied();
        });
    }
}