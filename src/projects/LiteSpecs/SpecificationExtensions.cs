using System.Collections.Generic;
using System.Linq;

namespace LiteSpecs
{
    public static class SpecificationExtensions
    {
        public static IEnumerable<T> ApplyOn<T>(this Specification<T> spec, IEnumerable<T> src)
            => src.Where(i => spec.IsSatisfiedBy(i).IsSatisfied);

        public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
        {
            if (left is AllSpecification<T>)
                return right;

            if (right is AllSpecification<T>)
                return left;

            return AndSpecification<T>.Create(left, right);
        }

        public static Specification<T> AndAlso<T>(this Specification<T> left, Specification<T> right)
        {
            if (left is AllSpecification<T>)
                return right;

            if (right is AllSpecification<T>)
                return left;

            return AndAlsoSpecification<T>.Create(left, right);
        }

        public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
        {
            if (left is AllSpecification<T>)
                return left;

            if (right is AllSpecification<T>)
                return right;

            return OrSpecification<T>.Create(left, right);
        }

        public static Specification<T> Not<T>(this Specification<T> spec, string reason)
            => NotSpecification<T>.Create(spec, reason);
    }
}