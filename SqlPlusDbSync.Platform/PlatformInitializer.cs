//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using SqlPlusDbSync.Data.Database;
//using SqlPlusDbSync.Platform.Configuration;
//using SqlPlusDbSync.Shared;

//namespace SqlPlusDbSync.Platform
//{
//    public class PlatformInitializer
//    {
//        public void Init(AsnaDatabaseContext context)
//        {
//            Logger.LogInfo("Check database isolation transaction level");
//            var cmd = @"
//            DECLARE @SNAPSHOT_STATE varchar(3)
//            SELECT @SNAPSHOT_STATE = snapshot_isolation_state_desc from sys.databases where name = DB_NAME()

//            IF(@SNAPSHOT_STATE = 'OFF')
//                BEGIN
//                    declare @cmd1 varchar(1000) = 'ALTER DATABASE '+DB_NAME()+' SET ALLOW_SNAPSHOT_ISOLATION ON'
//                    declare @cmd2 varchar(1000) = 'ALTER DATABASE '+DB_NAME()+' SET READ_COMMITTED_SNAPSHOT ON'
//                    EXEC sp_sqlexec @cmd1
//                    EXEC sp_sqlexec @cmd2
//                END";
//            context.CreateCommand(cmd).ExecuteNonQuery();
//        }

//        /// <summary>
//        /// tableName without schema
//        /// </summary>
//        /// <param name="tableName"></param>
//        public void RegisterTrigger(string tableName, string uniqueField, AsnaDatabaseContext context)
//        {
//            var cmd = $@"
//DECLARE @cmd varchar(2000) = 
//'CREATE TRIGGER [dbo].[vUpdate{tableName}]
//   ON  [dbo].[{tableName}]
//   AFTER INSERT,UPDATE,DELETE
//AS 
//BEGIN
//	SET NOCOUNT ON;
//    IF( CONTEXT_INFO() = 0x1234 ) RETURN;
//    MERGE __Versions V
//	USING  (SELECT {uniqueField} FROM inserted UNION SELECT {uniqueField} FROM deleted) S
//	ON (V.Id = S.{uniqueField}) and V.TableName = ''{tableName}''
//	WHEN 
//		NOT MATCHED BY TARGET THEN INSERT(Id,TableName) VALUES (S.{uniqueField}, ''{tableName}'')
//	WHEN
//		MATCHED THEN UPDATE SET Id = Id;
//END'

//IF(OBJECT_ID('dbo.vUpdate{tableName}') IS NULL)
//    EXEC sp_sqlexec @cmd
//";
//            context.CreateCommand(cmd).ExecuteNonQuery();
//        }

//        public void MetadataTrigger(string tableName, string uniqueField, AsnaDatabaseContext context)
//        {
//            var cmd = $@"
//DECLARE @cmd varchar(2000) = 
//'CREATE TRIGGER [dbo].[vMetadata{tableName}]
//   ON  [dbo].[{tableName}]
//   AFTER DELETE
//AS 
//BEGIN
//	SET NOCOUNT ON;
//    IF( CONTEXT_INFO() = 0x1234 ) RETURN;
//    MERGE __Metadata M
//	USING  (SELECT {uniqueField} as UKey, (SELECT * FROM deleted D2 WHERE D2.{uniqueField} = D1.{uniqueField} FOR XML PATH(''''), ROOT(''Object'')) AS Metadata FROM deleted D1) S
//	ON (M.Id = S.UKey)
//	WHEN 
//		NOT MATCHED BY TARGET THEN INSERT(Id, Metadata) VALUES (S.UKey, s.Metadata)
//	WHEN
//		MATCHED THEN UPDATE SET Id = Id;
//END'

//IF(OBJECT_ID('dbo.vMetadata{tableName}') IS NULL)
//    EXEC sp_sqlexec @cmd
//";
//            context.CreateCommand(cmd).ExecuteNonQuery();
//        }
//    }
//}