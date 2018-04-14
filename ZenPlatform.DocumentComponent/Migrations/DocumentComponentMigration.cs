using System;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Core;
using ZenPlatform.DataComponent;
using ZenPlatform.DataComponent.Migrations;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.DocumentComponent.Migrations
{
    public class DocumentComponentMigration : DataComponentMigrationBase<PDocumentObjectType>
    {
        public DocumentComponentMigration(SystemSession session) : base(session)
        {
        }
    }

    public class DocumentObjectMigration : DataComponentObjectMigrationBase<PDocumentObjectType>
    {
        public DocumentObjectMigration(SystemSession session, PDocumentObjectType newObjectType, PDocumentObjectType oldObjectType) : base(session, newObjectType, oldObjectType)
        {
            _migrationScript = new DBBatch();
        }

        private DBBatch _migrationScript;

        public override void Prepare()
        {
            if (MigrationStatus == ObjectMigrationStatus.Prepared)
                return;

            CheckTableName();

            this.MigrationStatus = ObjectMigrationStatus.Prepared;
        }


        private void CheckTableName()
        {
            if (NewObjectType.RelTableName != OldObjectType.RelTableName)
            {
                _migrationScript.AddQuery(new DBRenameTableQuery(OldObjectType.RelTableName, NewObjectType.RelTableName));
            }
        }

        private void CheckProperties()
        {
            foreach (var newProp in NewObjectType.Properties)
            {
                var oldProp = OldObjectType.Properties.FirstOrDefault(x => x.Id == newProp.Id);
                if (oldProp != null)
                {

                }
            }
        }

        private void CheckTypes(PProperty oldProp, PProperty newProp)
        {
            if (oldProp.Types.Count == 1 && newProp.Types.Count == 1)
            {
                var oldType = oldProp.Types[0];
                var newType = newProp.Types[0];

                if (oldType != newType)
                {
                    if (newType is PObjectType)
                    {
                        /*
                         * Это объект, необходимо добавить следующие поля:
                         *
                         * Uniqieidentifier - id объкта
                         *
                         * В случае, если у нас несколько типов необходимо добавить ещё
                         * одно поле:
                         * Int              - id типа
                         *
                         */


                    }

                    if (newType is PPrimetiveType)
                    {
                        /*
                         * Это примитиывный тип
                         *
                         * Необходимо сгенерировать колонку соответсвующего типа
                         *
                         * Guid, DateTime, Numeric, Varchar, Boolean 
                         *
                         */

                        DBType dbType;
                        string columnName;
                        int columnSize;
                        int numericPricision;
                        int numericScale;
                        bool isIdenty;
                        bool isKey;
                        bool isUnique;
                        bool isNullable;

                        var alter = new DBAlterTableQuery(NewObjectType.RelTableName);

                        switch (newType)
                        {
                            case newType is PBoolean:
                            {
                                dbType = DBType.Bit;
                                break;
                            }

                        }

                        alter.AlterColumn(dbType, columnName, columnSize, numericPricision, numericScale, isIdenty,
                            isKey, isUnique, isNullable);
                    }
                }
            }
            else
            {

            }
        }


        public override void Migrate()
        {
            var context = Session.DataContextManger.GetContext();

            try
            {
                context.BeginTransaction();
                using (var cmd = context.CreateCommand())
                {
                    cmd.CommandText = _migrationScript.Compile();

                    cmd.ExecuteNonQuery();
                }

                context.CommitTransaction();
            }
            catch (Exception ex)
            {
                context.RollbackTransaction();
            }
        }
    }
}
