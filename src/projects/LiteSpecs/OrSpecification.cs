using System;
using System.Linq;

namespace LiteSpecs
{
    public sealed class OrSpecification<T> : Specification<T>
    {
        private OrSpecification(Func<T, ISpecificationResult> predicate) : base(predicate) { }

        internal static OrSpecification<T> Create(ISpecification<T> spec1, ISpecification<T> spec2)
        {
            var pred1 = (Func<T, ISpecificationResult>)spec1.Eval;
            var pred2 = (Func<T, ISpecificationResult>)spec2.Eval;

            ISpecificationResult Pred(T i)
            {
                var pred1Result = pred1(i);
                if (pred1Result.IsSatisfied)
                    return pred1Result;

                var pred2Result = pred2(i);
                if (pred2Result.IsSatisfied)
                    return pred2Result;

                return SpecificationIs.NotSatisfied(pred1Result.Reasons.Concat(pred2Result.Reasons).ToArray());
            }

            return new OrSpecification<T>(Pred);
        }
    }
}