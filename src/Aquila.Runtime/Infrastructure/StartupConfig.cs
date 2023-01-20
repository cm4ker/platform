using System.ComponentModel.DataAnnotations;
using Aquila.Data;
using Aquila.Data.Tools;

namespace Aquila.Core
{
    /// <summary>
    /// Configuration for create instance
    /// </summary>
    public class StartupConfig
    {
        /// <summary>
        /// Connection string to the database (universal connection string <see cref="UniversalConnectionStringBuilder"/>)
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Database type
        /// </summary>
        [Required]
        public SqlDatabaseType DatabaseType { get; set; }

        /// <summary>
        /// Instance name
        /// </summary>
        [Required]
        public string InstanceName { get; set; }
    }
}