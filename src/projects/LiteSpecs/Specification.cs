using System;
using System.Linq.Expressions;

namespace LiteSpecs
{
    public static class Specification
    {
        public static Specification<T> All<T>() => Specification<T>.All;

        public static Specification<T> Generic<T>(Expression<Func<T, SpecificationResult>> predicate)
            => new Specification<T>(predicate);
    }

    public class Specification<T>
    {
        public static readonly Specification<T> All = AllSpecification<T>.Create();

        private readonly Expression<Func<T, SpecificationResult>> _predicate;
        private readonly Lazy<Func<T, SpecificationResult>> _compiled;

        protected internal Specification(Expression<Func<T, SpecificationResult>> predicate)
        {
            _predicate = predicate;
            _compiled = new Lazy<Func<T, SpecificationResult>>(() => _predicate.Compile());
        }

        public SpecificationResult IsSatisfiedBy(T item)
            => _compiled.Value(item);

        public static implicit operator Func<T, bool>(Specification<T> spec)
            => i => spec._compiled.Value(i);

        public static explicit operator Expression<Func<T, SpecificationResult>>(Specification<T> spec)
            => spec._predicate;
    }
}