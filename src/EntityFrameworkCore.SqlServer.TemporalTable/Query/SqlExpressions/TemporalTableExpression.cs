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
        public ParameterExpression AsOfDate { get; private set; }

        public ParameterExpression StartDate { get; private set; }
        public ParameterExpression EndDate { get; private set; }

        public void SetQueryTypeToAll()
        {
            this.AsOfDate = null;
            this.StartDate = null;
            this.EndDate = null;
            this.TemporalQueryType = TemporalQueryType.All;
        }

        public void SetAsOfDate(ParameterExpression parameter)
        {
            this.AsOfDate = parameter;
            this.TemporalQueryType = TemporalQueryType.AsOf;

            this.StartDate = null;
            this.EndDate = null;
        }

        public void SetFromToDate(ParameterExpression startDate, ParameterExpression endDate)
        {
            SetDateRangeCore(startDate, endDate, TemporalQueryType.FromTo);
        }

        public void SetBetweenAndDate(ParameterExpression startDate, ParameterExpression endDate)
        {
            SetDateRangeCore(startDate, endDate, TemporalQueryType.BetweenAnd);
        }

        public void SetContainedInDate(ParameterExpression startDate, ParameterExpression endDate)
        {
            SetDateRangeCore(startDate, endDate, TemporalQueryType.ContainedIn);
        }

        private void SetDateRangeCore(ParameterExpression startDate, ParameterExpression endDate, TemporalQueryType temporalQueryType)
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

            expressionPrinter.Append(Name).Append(" AS ").Append(Alias);
        }

        public override bool Equals(object obj)
             // This should be reference equal only.
             => obj != null && ReferenceEquals(this, obj);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Name, Schema);
    }
}
