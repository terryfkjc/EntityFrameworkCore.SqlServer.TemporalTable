using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalQueryRootExpression : QueryRootExpression
    {
        public TemporalQueryType TemporalQueryType { get; private set; }

        /// <summary>
        /// Gets and sets the parameter used to constrain a query to a specific temporal period.
        /// </summary>
        public Expression AsOfDate { get; private set; }
        public Expression StartDate { get; private set; }
        public Expression EndDate { get; private set; }

        public TemporalQueryRootExpression(IEntityType entityType)
            : base(entityType)
        {
            this.TemporalQueryType = TemporalQueryType.All;
        }

        public TemporalQueryRootExpression(IEntityType entityType, Expression asOfDate)
            : base(entityType)
        {
            TemporalQueryType = TemporalQueryType.AsOf;
            AsOfDate = asOfDate;
        }

        public TemporalQueryRootExpression(IEntityType entityType,
            Expression startDate, Expression endDate, TemporalQueryType temporalQueryType)
           : base(entityType)
        {
            if (temporalQueryType == TemporalQueryType.All || temporalQueryType == TemporalQueryType.AsOf)
            {
                throw new ArgumentException("Invalid TemporalQueryType.");
            }

            TemporalQueryType = temporalQueryType;
            AsOfDate = null;
            StartDate = startDate;
            EndDate = endDate;
        }

        public TemporalQueryRootExpression(
            IAsyncQueryProvider asyncQueryProvider, IEntityType entityType)
            : base(asyncQueryProvider, entityType)
        {
            this.TemporalQueryType = TemporalQueryType.All;
        }

        public TemporalQueryRootExpression(
            IAsyncQueryProvider asyncQueryProvider, IEntityType entityType, Expression asOfDate)
            : base(asyncQueryProvider, entityType)
        {
            TemporalQueryType = TemporalQueryType.AsOf;
            AsOfDate = asOfDate;
        }

        public TemporalQueryRootExpression(
            IAsyncQueryProvider asyncQueryProvider, IEntityType entityType,
            Expression startDate, Expression endDate, TemporalQueryType temporalQueryType)
            : base(asyncQueryProvider, entityType)
        {
            if (temporalQueryType == TemporalQueryType.All || temporalQueryType == TemporalQueryType.AsOf)
            {
                throw new ArgumentException("Invalid TemporalQueryType.");
            }

            TemporalQueryType = temporalQueryType;
            AsOfDate = null;
            StartDate = startDate;
            EndDate = endDate;
        }

        public override Expression DetachQueryProvider()
        {
            switch (this.TemporalQueryType)
            {
                case TemporalQueryType.None:
                    break;
                case TemporalQueryType.AsOf:
                    return new TemporalQueryRootExpression(this.EntityType, this.AsOfDate);
                case TemporalQueryType.FromTo:
                case TemporalQueryType.BetweenAnd:
                case TemporalQueryType.ContainedIn:
                    return new TemporalQueryRootExpression(
                        this.EntityType, this.StartDate, this.EndDate, this.TemporalQueryType);
                case TemporalQueryType.All:
                    return new TemporalQueryRootExpression(this.EntityType);
                default:
                    break;
            }

            throw new Exception("Unknown TemporalQueryType.");
        }

        public override bool CanReduce => false;

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            switch (this.TemporalQueryType)
            {
                case TemporalQueryType.None:
                    break;
                case TemporalQueryType.AsOf:
                    var _VisitedAsOf = visitor.Visit(this.AsOfDate);
                    if (_VisitedAsOf != this.AsOfDate)
                    {
                        if (this.QueryProvider == null)
                        {
                            return new TemporalQueryRootExpression(this.EntityType, _VisitedAsOf);
                        }
                        else
                        {
                            return new TemporalQueryRootExpression(this.QueryProvider, this.EntityType, _VisitedAsOf);
                        }
                    }
                    break;
                case TemporalQueryType.FromTo:
                case TemporalQueryType.BetweenAnd:
                case TemporalQueryType.ContainedIn:

                    var _VisitedStartDate = visitor.Visit(this.StartDate);
                    var _VisitedEndDate = visitor.Visit(this.EndDate);

                    if (this.StartDate != _VisitedStartDate || this.EndDate != _VisitedEndDate)
                    {
                        if (this.QueryProvider == null)
                        {
                            return new TemporalQueryRootExpression(
                                this.EntityType, _VisitedStartDate, _VisitedEndDate, this.TemporalQueryType);
                        }
                        else
                        {
                            return new TemporalQueryRootExpression(
                                this.QueryProvider, this.EntityType,
                                _VisitedStartDate, _VisitedEndDate, this.TemporalQueryType);
                        }
                    }

                    break;
                case TemporalQueryType.All:
                    break;
                default:
                    break;
            }

            return this;
        }

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            base.Print(expressionPrinter);
            expressionPrinter.AppendLine($".TemporalQuery({this.TemporalQueryType})");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && (ReferenceEquals(this, obj)
                    || obj is TemporalQueryRootExpression queryRootExpression
                    && Equals(queryRootExpression));
        }
    }
}
