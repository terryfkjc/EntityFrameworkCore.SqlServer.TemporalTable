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
                            SysEndDate = _Entity.GetEndDateColumnName()
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
                    SysEndDate = _TargetEntity.GetEndDateColumnName()
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
            
            return disableTemporal.Concat(leftovers).Concat(enableTemporal).ToList();
        }
    }
}
