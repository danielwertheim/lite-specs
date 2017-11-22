using System;

namespace LiteSpecs
{
    public sealed class AndAlsoSpecification<T> : Specification<T>
    {
        private AndAlsoSpecification(Func<T, ISpecificationResult> predicate) : base(predicate) { }

        internal static AndAlsoSpecification<T> Create(ISpecification<T> spec1, ISpecification<T> spec2)
        {
            var pred1 = (Func<T, ISpecificationResult>)spec1.Eval;
            var pred2 = (Func<T, ISpecificationResult>)spec2.Eval;

            ISpecificationResult Pred(T i)
            {
                var pred1Result = pred1(i);
                if (!pred1Result.IsSatisfied)
                    return pred1Result;

                var pred2Result = pred2(i);
                if (!pred2Result.IsSatisfied)
                    return pred2Result;

                return SpecificationIs.Satisfied;
            }

            return new AndAlsoSpecification<T>(Pred);
        }
    }
}