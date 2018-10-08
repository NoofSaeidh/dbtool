namespace DBTool.Core
{
    public static class Scripts
    {
        public static string Join(params string[] scripts) => string.Join("; ", scripts);

        #region Scripts

        public static string GetVersion => Texts.GET_VERSION;

        public static string AlterToMulti(string database) => Texts.ALTER_TO_MULTI.FormatWith(database);

        public static string AlterToSingle(string database) => Texts.ALTER_TO_SINGLE.FormatWith(database);

        public static string BackupToDisk(string database, string fileName) => Texts.BACKUP_TO_DISK.FormatWith(database, fileName);

        public static string GetLdf(string database) => Texts.GET_LDF.FormatWith(database);

        public static string GetMdf(string database) => Texts.GET_MDF.FormatWith(database);

        public static string GetRestoreScript(string database, string filePath, string mdf, string ldf, bool newVersion = false)
        {
            return newVersion
                ?
                Texts.GET_RESTORE_SCRIPT.FormatWith(database, filePath, mdf, ldf)
                :
                Texts.GET_RESTORE_SCRIPT_OLD.FormatWith(database, filePath, mdf, ldf);
        }

        public static string Use(string database) => Texts.USE.FormatWith(database);

        #endregion

        #region Texts

        public static class Texts
        {
            public const string ALTER_TO_MULTI = "ALTER DATABASE [{0}] SET Multi_User";
            public const string ALTER_TO_SINGLE = "ALTER DATABASE [{0}] SET Single_User WITH Rollback Immediate";
            public const string BACKUP_TO_DISK = "BACKUP DATABASE [{0}] TO DISK = '{1}' WITH INIT, NAME = 'FULL', FORMAT";
            public const string GET_VERSION = "select SERVERPROPERTY('productversion')";
            public const string USE = "USE {0}";

            #region Restore Scripts

            public const string GET_LDF =
@"SELECT
    Physical_Name
FROM
    sys.master_files mf
INNER JOIN
    sys.databases db ON db.database_id = mf.database_id
where db.name = '{0}'
and type_desc = 'LOG'";

            public const string GET_MDF =
    @"SELECT
    Physical_Name
FROM
    sys.master_files mf
INNER JOIN
    sys.databases db ON db.database_id = mf.database_id
where db.name = '{0}'
and type_desc = 'ROWS'";

            public const string GET_RESTORE_SCRIPT =
    @"DECLARE @db VARCHAR(1000) = '{0}',
		@Path VARCHAR(1000) = '{1}',
        @mdf VARCHAR(1000) = '{2}',
		@ldf VARCHAR(1000) = '{3}'

DECLARE @Table TABLE
    (
      LogicalName VARCHAR(128) ,
      [PhysicalName] VARCHAR(128) ,
      [Type] VARCHAR ,
      [FileGroupName] VARCHAR(128) ,
      [Size] VARCHAR(128) ,
      [MaxSize] VARCHAR(128) ,
      [FileId] VARCHAR(128) ,
      [CreateLSN] VARCHAR(128) ,
      [DropLSN] VARCHAR(128) ,
      [UniqueId] VARCHAR(128) ,
      [ReadOnlyLSN] VARCHAR(128) ,
      [ReadWriteLSN] VARCHAR(128) ,
      [BackupSizeInBytes] VARCHAR(128) ,
      [SourceBlockSize] VARCHAR(128) ,
      [FileGroupId] VARCHAR(128) ,
      [LogGroupGUID] VARCHAR(128) ,
      [DifferentialBaseLSN] VARCHAR(128) ,
      [DifferentialBaseGUID] VARCHAR(128) ,
      [IsReadOnly] VARCHAR(128) ,
      [IsPresent] VARCHAR(128) ,
      [TDEThumbprint] VARCHAR(128),
      [SnapshotURL] VARCHAR(360)
    )

DECLARE @LogicalNameData VARCHAR(128) ,
    @LogicalNameLog VARCHAR(128)
INSERT  INTO @table
        EXEC ( '
RESTORE FILELISTONLY
   FROM DISK=''' + @Path + '''
   '
            )

DECLARE @restoreScript NVARCHAR(max)='RESTORE DATABASE ['+ @db +'] FROM DISK =''' + @Path + ''' WITH FILE = 1 '

SELECT  @restoreScript +=CHAR(10) + ' ,MOVE  ''' +  LogicalName + ''' TO ''' +
        @mdf  + ''''
                         FROM   @Table
                         WHERE  Type = 'D'

 SELECT  @restoreScript += ' ,MOVE  ''' +  LogicalName + ''' TO ''' + @ldf + ''''
                        FROM    @Table
                        WHERE   Type = 'L'

SET @restoreScript += ' , NOUNLOAD, REPLACE, STATS = 10 '
SELECT  @restoreScript";

            public const string GET_RESTORE_SCRIPT_OLD =
    @"DECLARE @db VARCHAR(1000) = '{0}',
		@Path VARCHAR(1000) = '{1}',
        @mdf VARCHAR(1000) = '{2}',
		@ldf VARCHAR(1000) = '{3}'

DECLARE @Table TABLE
    (
      LogicalName VARCHAR(128) ,
      [PhysicalName] VARCHAR(128) ,
      [Type] VARCHAR ,
      [FileGroupName] VARCHAR(128) ,
      [Size] VARCHAR(128) ,
      [MaxSize] VARCHAR(128) ,
      [FileId] VARCHAR(128) ,
      [CreateLSN] VARCHAR(128) ,
      [DropLSN] VARCHAR(128) ,
      [UniqueId] VARCHAR(128) ,
      [ReadOnlyLSN] VARCHAR(128) ,
      [ReadWriteLSN] VARCHAR(128) ,
      [BackupSizeInBytes] VARCHAR(128) ,
      [SourceBlockSize] VARCHAR(128) ,
      [FileGroupId] VARCHAR(128) ,
      [LogGroupGUID] VARCHAR(128) ,
      [DifferentialBaseLSN] VARCHAR(128) ,
      [DifferentialBaseGUID] VARCHAR(128) ,
      [IsReadOnly] VARCHAR(128) ,
      [IsPresent] VARCHAR(128) ,
      [TDEThumbprint] VARCHAR(128)
    )

DECLARE @LogicalNameData VARCHAR(128) ,
    @LogicalNameLog VARCHAR(128)
INSERT  INTO @table
        EXEC ( '
RESTORE FILELISTONLY
   FROM DISK=''' + @Path + '''
   '
            )

DECLARE @restoreScript NVARCHAR(max)='RESTORE DATABASE ['+ @db +'] FROM DISK =''' + @Path + ''' WITH FILE = 1 '

SELECT  @restoreScript +=CHAR(10) + ' ,MOVE  ''' +  LogicalName + ''' TO ''' +
        @mdf  + ''''
                         FROM   @Table
                         WHERE  Type = 'D'

 SELECT  @restoreScript += ' ,MOVE  ''' +  LogicalName + ''' TO ''' + @ldf + ''''
                        FROM    @Table
                        WHERE   Type = 'L'

SET @restoreScript += ' , NOUNLOAD, REPLACE, STATS = 10 '
SELECT  @restoreScript";

            #endregion
        }

        #endregion
    }
}