﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartWait.Helpers
{
    public static class ExpressionExtension
    {
        public static Expression<Func<TIn, TIn, TOut>>? Get<TIn, TOut>(Expression<Func<TIn, TIn, TOut>> expression) => PartialEval(expression) as Expression<Func<TIn, TIn, TOut>>;

        public static Expression? Get(Expression? expression) => PartialEval(expression);

        private static Expression? PartialEval(Expression? expression, Func<Expression, bool> fnCanBeEvaluated) => new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);

        private static Expression? PartialEval(Expression? expression) => PartialEval(expression, CanBeEvaluatedLocally);

        private static bool CanBeEvaluatedLocally(Expression? expression) => expression?.NodeType != ExpressionType.Parameter;

        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> _candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates) => _candidates = candidates;

            internal Expression? Eval(Expression? exp) => Visit(exp);

            public override Expression? Visit(Expression? exp)
            {
                if (exp is null) return exp;

                return _candidates.Contains(exp) ? Evaluate(exp) : base.Visit(exp);
            }

            private static Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant) return e;

                var lambda = Expression.Lambda(e, Array.Empty<ParameterExpression>());
                var fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _fnCanBeEvaluated;
            private readonly HashSet<Expression> _candidates= new();
            private bool _cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                _fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression? expression)
            {
                Visit(expression);
                return _candidates;
            }

            public override Expression? Visit(Expression? expression)
            {
                if (expression is null) return expression;
                var saveCannotBeEvaluated = _cannotBeEvaluated;

                _cannotBeEvaluated = false;

                base.Visit(expression);

                if (!_cannotBeEvaluated)
                {
                    if (_fnCanBeEvaluated(expression))
                        _candidates.Add(expression);

                    else
                        _cannotBeEvaluated = true;
                }

                _cannotBeEvaluated |= saveCannotBeEvaluated;

                return expression;
            }
        }
    }
}