using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Query
{
    internal class TemporalQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
    {
        public TemporalQueryTranslationPreprocessorFactory(
            QueryTranslationPreprocessorDependencies queryTranslationPreprocessorDependencies,
            RelationalQueryTranslationPreprocessorDependencies relationalQueryTranslationPreprocessorDependencies)
        {
            QueryTranslationPreprocessorDependencies = queryTranslationPreprocessorDependencies ?? throw new ArgumentNullException(nameof(queryTranslationPreprocessorDependencies));
            RelationalQueryTranslationPreprocessorDependencies = relationalQueryTranslationPreprocessorDependencies ?? throw new ArgumentNullException(nameof(relationalQueryTranslationPreprocessorDependencies));
        }

        public QueryTranslationPreprocessorDependencies QueryTranslationPreprocessorDependencies { get; }
        public RelationalQueryTranslationPreprocessorDependencies RelationalQueryTranslationPreprocessorDependencies { get; }

        public QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        {
            return new TemporalQueryQueryTranslationPreprocessor(
                this.QueryTranslationPreprocessorDependencies,
                this.RelationalQueryTranslationPreprocessorDependencies,
                queryCompilationContext);
        }
    }
}
