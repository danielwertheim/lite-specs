using System;

namespace LiteSpecs
{
    public sealed class NotSpecification<T> : Specification<T>
    {
        private NotSpecification(Func<T, SpecificationResult> predicate) : base(predicate) { }

        public static NotSpecification<T> Create(Specification<T> spec, string reason)
        {
            var pred = (Func<T, SpecificationResult>)spec.IsSatisfiedBy;

            SpecificationResult Pred(T i)
            {
                var predResult = pred(i);
                return predResult.IsSatisfied
                    ? SpecificationResult.NotSatisfied(reason)
                    : SpecificationResult.Satisfied;
            }

            return new NotSpecification<T>(Pred);
        }
    }
}