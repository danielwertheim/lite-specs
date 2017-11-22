using System;
using System.Linq;

namespace LiteSpecs
{
    public sealed class OrSpecification<T> : Specification<T>
    {
        private OrSpecification(Func<T, SpecificationResult> predicate) : base(predicate) { }

        internal static OrSpecification<T> Create(Specification<T> spec1, Specification<T> spec2)
        {
            var pred1 = (Func<T, SpecificationResult>)spec1.IsSatisfiedBy;
            var pred2 = (Func<T, SpecificationResult>)spec2.IsSatisfiedBy;

            SpecificationResult Pred(T i)
            {
                var pred1Result = pred1(i);
                if (pred1Result.IsSatisfied)
                    return pred1Result;

                var pred2Result = pred2(i);
                if (pred2Result.IsSatisfied)
                    return pred2Result;

                return SpecificationResult.NotSatisfied(pred1Result.Reasons.Concat(pred2Result.Reasons).ToArray());
            }

            return new OrSpecification<T>(Pred);
        }
    }
}