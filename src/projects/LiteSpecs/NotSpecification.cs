using System;
using System.Linq.Expressions;

namespace LiteSpecs
{
    public sealed class NotSpecification<T> : Specification<T>
    {
        private NotSpecification(Expression<Func<T, SpecificationResult>> predicate) : base(predicate) { }

        public static NotSpecification<T> Create(Specification<T> spec, string reason)
        {
            var pred = (Expression<Func<T, SpecificationResult>>)spec;

            var p = Expression.Parameter(typeof(T));

            var visitor = new ReplaceExpressionVisitor(pred.Parameters[0], p);
            var body = visitor.Visit(pred.Body);

            var bodyAsBool = Expression.Convert(body, typeof(bool));

            var not = Expression.Not(bodyAsBool);
            var satisfiedOrNot = Expression.Condition(not,
                Expression.Constant(SpecificationResult.Satisfied),
                Expression.Constant(SpecificationResult.NotSatisfied(reason)));

            return new NotSpecification<T>(
                Expression.Lambda<Func<T, SpecificationResult>>(satisfiedOrNot, p));
        }
    }
}