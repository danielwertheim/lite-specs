using System;
using System.Linq;

namespace LiteSpecs
{
    public sealed class AndSpecification<T> : Specification<T>
    {
        private AndSpecification(Func<T, ISpecificationResult> predicate) : base(predicate) { }

        internal static AndSpecification<T> Create(ISpecification<T> spec1, ISpecification<T> spec2)
        {
            var pred1 = (Func<T, ISpecificationResult>)spec1.Eval;
            var pred2 = (Func<T, ISpecificationResult>)spec2.Eval;

            ISpecificationResult Pred(T i)
            {
                var pred1Result = pred1(i);
                var pred2Result = pred2(i);

                if (!pred1Result.IsSatisfied && !pred2Result.IsSatisfied)
                    return SpecificationIs.NotSatisfied(pred1Result.Reasons.Concat(pred2Result.Reasons).ToArray());

                if (!pred1Result.IsSatisfied)
                    return pred1Result;

                if (!pred2Result.IsSatisfied)
                    return pred2Result;

                return SpecificationIs.Satisfied;
            }

            return new AndSpecification<T>(Pred);
        }
    }
}