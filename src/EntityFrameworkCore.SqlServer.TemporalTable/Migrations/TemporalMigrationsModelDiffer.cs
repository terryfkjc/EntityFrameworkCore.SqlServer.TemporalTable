using EntityFrameworkCore.SqlServer.TemporalTable.Metadata;
using EntityFrameworkCore.SqlServer.TemporalTable.Migrations.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Collections.Generic;
using System.Linq;

namespace EntityFrameworkCore.SqlServer.TemporalTable.Migrations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    public class TemporalMigrationsModelDiffer : MigrationsModelDiffer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
        public TemporalMigrationsModelDiffer(
            IRelationalTypeMappingSource typeMappingSource,
            IMigrationsAnnotationProvider migrationsAnnotations,
            IChangeDetector changeDetector,
            IUpdateAdapterFactory updateAdapterFactory,
            CommandBatchPreparerDependencies commandBatchPreparerDependencies)
            : base(typeMappingSource, migrationsAnnotations, changeDetector, updateAdapterFactory, commandBatchPreparerDependencies)
        {
            
        }


        protected override IEnumerable<MigrationOperation> Remove(ITable source, DiffContext diffContext)
        {
            List<MigrationOperation> _Operations = new List<MigrationOperation>();
            var _Entity = source.Model.Model.FindEntity(source.Name, source.Schema);
            var _IsTemporal = _Entity.HasTemporalTable();
            
            if (_IsTemporal)
            {
                _Operations.Add(new DisableTemporalTableOperation()
                {
                    Name = _Entity.GetTableName(),
                    Schema = _Entity.GetSchema()
                });

                var _HistoryTable = _Entity.GetHistoryTableName();
                var _HistoryTableSchema = _Entity.GetHistoryTableSchema();

                _Operations.Add(new DropHistoryTableOperation()
                {
                    Name = _HistoryTable,
                    Schema = _HistoryTableSchema
                });

                //Note:
                //  The history table is not tracked by EntityFramework, so do not
                //  use DropTableOperation else later will get NullReferenceException from 
                //
                // Microsoft.EntityFrameworkCore.Migrations.Internal.MigrationsModelDiffer.Sort(IEnumerable`1 operations, DiffContext diffContext)


                //_Operations.Add(new DropTableOperation()
                //{
                //    Name = _HistoryTable,
                //    Schema = _HistoryTableSchema ?? TemporalAnnotationNames.DefaultSchema, 
                //    IsDestructiveChange = true
                //});
            }

            _Operations.AddRange(base.Remove(source, diffContext).ToArray());
            return _Operations;
        }

        protected override IEnumerable<MigrationOperation> Add(ITable target, DiffContext diffContext)
        {
            var _Entity = target.Model.Model.FindEntity(target.Name, target.Schema);
            var _IsTemporal = _Entity.HasTemporalTable();

            List<MigrationOperation> _Operations = new List<MigrationOperation>();
            _Operations.AddRange(base.Add(target, diffContext).ToArray());

            if (_IsTemporal == true)
            {
                for (int i = 0; i < _Operations.Count; i++)
                {
                    if (_Operations[i] is CreateTableOperation createTableOperation)
                    {
                        string _HistorySchema = _Entity.GetHistoryTableSchema();

                        //Ensure schema for history table
                        if (_HistorySchema != null)
                        {
                            _Operations.AddRange(base.Add(_HistorySchema, diffContext));
                        }

                        //on top of "CREATE TABLE" operaion, we add additional operation to convert
                        //normal table into temporal table.
                        EnableTemporalTableOperation enableTemporalTableOperation = new EnableTemporalTableOperation()
                        {
                            Name = _Entity.GetTableName(),
                            Schema = _Entity.GetSchema(),
                            HistoryTable = _Entity.GetHistoryTableName(),
                            HistorySchema = _Entity.GetHistoryTableSchema(),
                            SysStartDate = _Entity.GetStartDateColumnName(),
                            SysEndDate = _Entity.GetEndDateColumnName(),
                            DataConsistencyCheck = _Entity.DataConsistencyCheck()
                        };

                        _Operations.Add(enableTemporalTableOperation);
                    }
                }
            }

            return _Operations;
        }

        protected override IEnumerable<MigrationOperation> Diff(ITable source, ITable target, DiffContext diffContext)
        {
            var _SourceEntity = source.Model.Model.FindEntity(source.Name, source.Schema);
            var _TargetEntity = target.Model.Model.FindEntity(target.Name, target.Schema);

            var _SourceIsTemporal = _SourceEntity.HasTemporalTable();
            var _TargetIsTemporal = _TargetEntity.HasTemporalTable();

            List<MigrationOperation> _Operations = new List<MigrationOperation>();
            _Operations.AddRange(base.Diff(source, target, diffContext).ToArray());

            if (_SourceIsTemporal == false && _TargetIsTemporal == true)
            {
                EnableTemporalTableOperation enableTemporal = new EnableTemporalTableOperation()
                {
                    Name = _TargetEntity.GetTableName(),
                    Schema = _TargetEntity.GetSchema(),
                    HistoryTable = _TargetEntity.GetHistoryTableName(),
                    HistorySchema = _TargetEntity.GetHistoryTableSchema(),
                    SysStartDate = _TargetEntity.GetStartDateColumnName(),
                    SysEndDate = _TargetEntity.GetEndDateColumnName(),
                    DataConsistencyCheck = _TargetEntity.DataConsistencyCheck()
                };

                if (enableTemporal.HistorySchema != null)
                {
                    //Ensure schema for history table.
                    _Operations.AddRange(base.Add(enableTemporal.HistorySchema, diffContext));
                }

                _Operations.Add(enableTemporal);
            }
            else if (_SourceIsTemporal == true && _TargetIsTemporal == false)
            {
                DisableTemporalTableOperation disableTemporal = new DisableTemporalTableOperation()
                {
                    Name = _SourceEntity.GetTableName(),
                    Schema = _SourceEntity.GetSchema()
                };

                ////Don't drop the Start/End date column when we detect history tracking is disabled.
                //var _TemporalStartColumn = _SourceEntity.GetStartDateColumnName();
                //var _TemporalEndColumn = _SourceEntity.GetEndDateColumnName();

                //for (int i = 0; i < _Operations.Count; i++)
                //{
                //    if (_Operations[i] is DropColumnOperation dropColumnOperation)
                //    {
                //        if (dropColumnOperation.Table == _SourceEntity.GetTableName() && 
                //            (dropColumnOperation.Name == _TemporalStartColumn || dropColumnOperation.Name == _TemporalEndColumn))
                //        {
                //            _Operations.RemoveAt(i);
                //            i--;
                //        }
                //    }
                //}

                _Operations.Add(disableTemporal);
            }

            return _Operations;
        }

        protected override IReadOnlyList<MigrationOperation> Sort(IEnumerable<MigrationOperation> operations, DiffContext diffContext)
        {
            //Default implementation will cause custom operations located at the last of the list.
            //This will cause trouble when we want to disable temporal table.
            //Disabling a temporal table will cause
            //  1. Drop start/end date column
            //  2. remove system time period.
            //We need to ensure system time period is dropped before drop the start/end date

            var _Sorted = base.Sort(operations, diffContext).ToArray();

            var enableTemporal = new List<EnableTemporalTableOperation>();
            var disableTemporal = new List<DisableTemporalTableOperation>();

            var leftovers = new List<MigrationOperation>();

            for (int i = 0; i < _Sorted.Length; i++)
            {
                if (_Sorted[i] is EnableTemporalTableOperation enable)
                {
                    enableTemporal.Add(enable);
                }
                else if (_Sorted[i] is DisableTemporalTableOperation disable)
                {
                    disableTemporal.Add(disable);
                }
                else
                {
                    leftovers.Add(_Sorted[i]);
                }
            }
            
            //If we found there are operations that adding Start/End date column on those tables 
            //to be enabled temporal again, we need to make sure the start/end date are properly 
            //configured with default values
            foreach (var _EnableTemporal in enableTemporal)
            {
                var _AddStartColumnQuery = from c in leftovers.OfType<AddColumnOperation>()
                                           where
                                            BothAreMatchedOrNull(c.Name, _EnableTemporal.SysStartDate) &&
                                            BothAreMatchedOrNull(c.Table, _EnableTemporal.Name) &&
                                            BothAreMatchedOrNull(c.Schema, _EnableTemporal.Schema)
                                           select c;

                var _AddStartColumn = _AddStartColumnQuery.FirstOrDefault();
                if (_AddStartColumn != null)
                {
                    _AddStartColumn.DefaultValueSql = "SYSUTCDATETIME()";
                }

                var _AddEndColumnQuery = from c in leftovers.OfType<AddColumnOperation>()
                                         where
                                           BothAreMatchedOrNull(c.Name, _EnableTemporal.SysEndDate) &&
                                           BothAreMatchedOrNull(c.Table, _EnableTemporal.Name) &&
                                           BothAreMatchedOrNull(c.Schema, _EnableTemporal.Schema)
                                         select c;

                var _AddEndColumn = _AddEndColumnQuery.FirstOrDefault();
                if (_AddEndColumn != null)
                {
                    _AddEndColumn.DefaultValueSql = "'9999-12-31 23:59:59.9999999'";
                }
            }

            return disableTemporal.Concat(leftovers).Concat(enableTemporal).ToList();
        }

        private bool BothAreMatchedOrNull(string a, string b)
		{
            return (a == null && b == null) || string.Equals(a, b, System.StringComparison.OrdinalIgnoreCase);
		}
    }
}
