using System;
using System.Linq;

namespace LiteSpecs
{
    public sealed class AndSpecification<T> : Specification<T>
    {
        private AndSpecification(Func<T, SpecificationResult> predicate) : base(predicate) { }

        internal static AndSpecification<T> Create(Specification<T> spec1, Specification<T> spec2)
        {
            var pred1 = (Func<T, SpecificationResult>)spec1.IsSatisfiedBy;
            var pred2 = (Func<T, SpecificationResult>)spec2.IsSatisfiedBy;

            SpecificationResult Pred(T i)
            {
                var pred1Result = pred1(i);
                var pred2Result = pred2(i);

                if (!pred1Result.IsSatisfied && !pred2Result.IsSatisfied)
                    return SpecificationResult.NotSatisfied(pred1Result.Reasons.Concat(pred2Result.Reasons).ToArray());

                if (!pred1Result.IsSatisfied)
                    return pred1Result;

                if (!pred2Result.IsSatisfied)
                    return pred2Result;

                return SpecificationResult.Satisfied;
            }

            return new AndSpecification<T>(Pred);
        }
    }
}