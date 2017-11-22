using System.Collections.Generic;
using System.Linq;

namespace LiteSpecs
{
    public static class SpecificationExtensions
    {
        public static IEnumerable<T> Matching<T>(this IEnumerable<T> src, ISpecification<T> specification)
            => src.Where(i => specification.Eval(i).IsSatisfied);

        public static IEnumerable<T> ApplyOn<T>(this ISpecification<T> spec, IEnumerable<T> src)
            => src.Where(i => spec.Eval(i).IsSatisfied);

        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            if (left is AllSpecification<T>)
                return right;

            if (right is AllSpecification<T>)
                return left;

            return AndSpecification<T>.Create(left, right);
        }

        public static ISpecification<T> AndAlso<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            if (left is AllSpecification<T>)
                return right;

            if (right is AllSpecification<T>)
                return left;

            return AndAlsoSpecification<T>.Create(left, right);
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            if (left is AllSpecification<T>)
                return left;

            if (right is AllSpecification<T>)
                return right;

            return OrSpecification<T>.Create(left, right);
        }

        public static ISpecification<T> Not<T>(this ISpecification<T> spec, string reason)
            => NotSpecification<T>.Create(spec, reason);
    }
}