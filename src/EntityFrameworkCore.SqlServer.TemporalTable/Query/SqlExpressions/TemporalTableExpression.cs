using EntityFrameworkCore.SqlServer.TemporalTable.Query;
using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.SqlExpressions
{
    internal sealed class TemporalTableExpression : TableExpressionBase
    {
        public string Name { get; }
        public string Schema { get; }

        public TemporalQueryType TemporalQueryType { get; private set; }

        /// <summary>
        /// Gets and sets the parameter used to constrain a query to a specific temporal period.
        /// </summary>
        public Expression AsOfDate { get; private set; }

        public Expression StartDate { get; private set; }
        public Expression EndDate { get; private set; }

        public void SetQueryTypeToAll()
        {
            this.AsOfDate = null;
            this.StartDate = null;
            this.EndDate = null;
            this.TemporalQueryType = TemporalQueryType.All;
        }

        public void SetAsOfDate(Expression parameter)
        {
            this.AsOfDate = parameter;
            this.TemporalQueryType = TemporalQueryType.AsOf;

            this.StartDate = null;
            this.EndDate = null;
        }

        public void SetFromToDate(Expression startDate, Expression endDate)
        {
            SetDateRangeCore(startDate, endDate, TemporalQueryType.FromTo);
        }

        public void SetBetweenAndDate(Expression startDate, Expression endDate)
        {
            SetDateRangeCore(startDate, endDate, TemporalQueryType.BetweenAnd);
        }

        public void SetContainedInDate(Expression startDate, Expression endDate)
        {
            SetDateRangeCore(startDate, endDate, TemporalQueryType.ContainedIn);
        }

        private void SetDateRangeCore(Expression startDate, Expression endDate, TemporalQueryType temporalQueryType)
        {
            this.AsOfDate = null;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.TemporalQueryType = temporalQueryType;
        }

        public TemporalTableExpression(string name, string schema, string alias)
            : base(alias)
        {
            Name = name;
            Schema = schema;
        }

        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            if (!string.IsNullOrEmpty(Schema))
            {
                expressionPrinter.Append(Schema).Append(".");
            }

            expressionPrinter.Append(Name);

            switch (this.TemporalQueryType)
            {
                case TemporalQueryType.None:
                    break;
                case TemporalQueryType.AsOf:
                    expressionPrinter.Append(" FOR SYSTEM_TIME AS OF ");
                    expressionPrinter.Append(this.AsOfDate.ToString());
                    break;
                case TemporalQueryType.FromTo:
                    expressionPrinter.Append(" FOR SYSTEM_TIME FROM ");
                    expressionPrinter.Append(this.StartDate.ToString());
                    expressionPrinter.Append(" TO ");
                    expressionPrinter.Append(this.EndDate.ToString());
                    break;
                case TemporalQueryType.BetweenAnd:
                    expressionPrinter.Append(" FOR SYSTEM_TIME BETWEEN ");
                    expressionPrinter.Append(this.StartDate.ToString());
                    expressionPrinter.Append(" AND ");
                    expressionPrinter.Append(this.EndDate.ToString());
                    break;
                case TemporalQueryType.ContainedIn:
                    expressionPrinter.Append(" FOR SYSTEM_TIME CONTAINED IN(");
                    expressionPrinter.Append(this.StartDate.ToString());
                    expressionPrinter.Append(", ");
                    expressionPrinter.Append(this.EndDate.ToString());
                    expressionPrinter.Append(")");
                    break;
                case TemporalQueryType.All:
                    expressionPrinter.Append(" FOR SYSTEM_TIME ALL ");
                    break;
                default:
                    break;
            }

            expressionPrinter.Append(" AS ").Append(Alias);
        }

        public override bool Equals(object obj)
             // This should be reference equal only.
             => obj != null && ReferenceEquals(this, obj);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Name, Schema);

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
                        return new TemporalTableExpression(this.Name, this.Schema, this.Alias)
                        {
                            TemporalQueryType = TemporalQueryType.AsOf,
                            AsOfDate = _VisitedAsOf,
                        };
                    }

                    break;
                case TemporalQueryType.FromTo:
                case TemporalQueryType.BetweenAnd:
                case TemporalQueryType.ContainedIn:

                    var _VisitedStartDate = visitor.Visit(this.StartDate);
                    var _VisitedEndDate = visitor.Visit(this.EndDate);

                    if (this.StartDate != _VisitedStartDate || this.EndDate != _VisitedEndDate)
                    {
                        return new TemporalTableExpression(this.Name, this.Schema, this.Alias)
                        {
                            TemporalQueryType = TemporalQueryType.AsOf,
                            StartDate = _VisitedStartDate,
                            EndDate = _VisitedEndDate
                        };
                    }
                    break;
                case TemporalQueryType.All:
                    break;
                default:
                    break;
            }

            return this;
        }
    }
}
