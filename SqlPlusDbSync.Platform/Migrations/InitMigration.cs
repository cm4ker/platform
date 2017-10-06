using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator;

namespace SqlPlusDbSync.Platform.Migrations
{
    [Migration(1)]
    public class InitMigration : Migration
    {
        public override void Up()
        {
            var cmd = @"
DECLARE @SNAPSHOT_STATE varchar(3)
SELECT @SNAPSHOT_STATE = snapshot_isolation_state_desc from sys.databases where name = DB_NAME()

IF(@SNAPSHOT_STATE = 'OFF')
    BEGIN
        declare @cmd1 varchar(1000) = 'ALTER DATABASE '+DB_NAME()+' SET ALLOW_SNAPSHOT_ISOLATION ON'
        declare @cmd2 varchar(1000) = 'ALTER DATABASE '+DB_NAME()+' SET READ_COMMITTED_SNAPSHOT ON'
        EXEC sp_sqlexec @cmd1
        EXEC sp_sqlexec @cmd2
    END

IF(OBJECT_ID('dbo.__Versions') IS NOT NULL)
    GOTO Flag1

CREATE TABLE [dbo].[__Versions](
	[TableName] [varchar](30) NULL,
	[Id] [uniqueidentifier] NULL,
	[Version] [timestamp] NOT NULL,
) ON [PRIMARY]

Flag1:

IF(OBJECT_ID('dbo.__Syncronization') IS NOT NULL)
    GOTO Flag2
CREATE TABLE [dbo].[__Syncronization](
	[PointId] [uniqueidentifier] NOT NULL,
	[SyncronizedVersion] [varbinary](64) NULL,
	[SyncronizationDate] [datetime] NULL,
    [SyncronizationUncommitedVersion] [varbinary](8) NULL,
    [UncommitedDate] [datetime]
) ON [PRIMARY]
Flag2:

IF(OBJECT_ID('dbo.__Rules') IS NOT NULL)
    GOTO Flag3
CREATE TABLE [dbo].[__Rules](
	[RulesText] nvarchar(max) NOT NULL
) ON [PRIMARY]

Flag3:
";
            Execute.Sql(cmd);
        }

        public override void Down()
        {

        }
    }

    [Migration(2)]
    public class SecondMigration : Migration
    {
        public override void Up()
        {
            var cmd = @"
IF(OBJECT_ID('dbo.__Metadata') IS NOT NULL)
    GOTO Flag2
CREATE TABLE [dbo].[__Metadata](
	[Id] [uniqueidentifier] NOT NULL,
	[Metadata] [xml]
) ON [PRIMARY]
Flag2:
";
            Execute.Sql(cmd);
        }

        public override void Down()
        {

        }
    }

}
