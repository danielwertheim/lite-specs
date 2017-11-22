using System;

namespace LiteSpecs
{
    public sealed class AndAlsoSpecification<T> : Specification<T>
    {
        private AndAlsoSpecification(Func<T, SpecificationResult> predicate) : base(predicate) { }

        internal static AndAlsoSpecification<T> Create(Specification<T> spec1, Specification<T> spec2)
        {
            var pred1 = (Func<T, SpecificationResult>)spec1.IsSatisfiedBy;
            var pred2 = (Func<T, SpecificationResult>)spec2.IsSatisfiedBy;

            SpecificationResult Pred(T i)
            {
                var pred1Result = pred1(i);
                if (!pred1Result.IsSatisfied)
                    return pred1Result;

                var pred2Result = pred2(i);
                if (!pred2Result.IsSatisfied)
                    return pred2Result;

                return SpecificationResult.Satisfied;
            }

            return new AndAlsoSpecification<T>(Pred);
        }
    }
}