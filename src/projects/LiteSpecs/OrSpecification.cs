using System;
using System.Linq.Expressions;

namespace LiteSpecs
{
    public sealed class OrSpecification<T> : Specification<T>
    {
        private OrSpecification(Expression<Func<T, SpecificationResult>> predicate) : base(predicate) { }

        internal static OrSpecification<T> Create(Specification<T> spec1, Specification<T> spec2, string reason)
        {
            var pred1 = (Expression<Func<T, SpecificationResult>>)spec1;
            var pred2 = (Expression<Func<T, SpecificationResult>>)spec2;

            var p = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(pred1.Parameters[0], p);
            var left = leftVisitor.Visit(pred1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(pred2.Parameters[0], p);
            var right = rightVisitor.Visit(pred2.Body);

            var leftAsBool = Expression.Convert(left, typeof(bool));
            var rightAsBool = Expression.Convert(right, typeof(bool));

            var or = Expression.OrElse(leftAsBool, rightAsBool);
            var satisfiedOrNot = Expression.Condition(or,
                Expression.Constant(SpecificationResult.Satisfied),
                Expression.Constant(SpecificationResult.NotSatisfied(reason)));

            return new OrSpecification<T>(
                Expression.Lambda<Func<T, SpecificationResult>>(satisfiedOrNot, p));
        }
    }
}