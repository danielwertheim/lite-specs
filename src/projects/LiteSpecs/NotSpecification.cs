using System;

namespace LiteSpecs
{
    public sealed class NotSpecification<T> : Specification<T>
    {
        private NotSpecification(Func<T, ISpecificationResult> predicate) : base(predicate) { }

        public static NotSpecification<T> Create(ISpecification<T> spec, string reason)
        {
            var pred = (Func<T, ISpecificationResult>)spec.Eval;

            ISpecificationResult Pred(T i)
            {
                var predResult = pred(i);
                return predResult.IsSatisfied
                    ? SpecificationIs.NotSatisfied(reason)
                    : SpecificationIs.Satisfied;
            }

            return new NotSpecification<T>(Pred);
        }
    }
}