using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionSimulator.Domain.Config
{
    public class AppSettings
    {
        public ConnectionStringsSettings ConnectionStrings { get; set; } = new();
        public DatabaseConfigSettings DatabaseConfig { get; set; } = new();
        public BankPolicySettings BankPolicy { get; set; } = new();
        public PagingConfigSettings PagingConfig { get; set; } = new();
        public CorsConfigSettings CorsConfig { get; set; } = new();
        public LoggingSettings Logging{ get; set; } = new();
        public class ConnectionStringsSettings
        {
            public string DefaultConnection { get; set; } = string.Empty;
        }
        public class LoggingSettings
        {
            public string LogLevel { get; set; } = "Information";
            public string LogPath { get; set; } = "Logs/api-log.txt";
        }
        public class CorsConfigSettings
        {
            public string PolicyName { get; set; } = "AllowReactApp";
            public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
        }
        public class DatabaseConfigSettings
        {
            public string SchemaName { get; set; } = "shva";
            public string RegionsTable { get; set; } = "Regions";
            public string OpeningHoursTable { get; set; } = "OpeningHours";
            public string TransactionsTable { get; set; } = "Transactions";
        }
        public class BankPolicySettings
        {
            public string DefaultOpeningTime { get; set; } = "8";
            public string DefaultClosingTime { get; set; } = "18";
            public bool IsWeekendClosed { get; set; }
            public bool SupportGlobalTimeZones { get; set; }
        }
        public class PagingConfigSettings
        {
            public int DefaultPageNumber { get; set; } = 1; 
            public int DefaultPageSize { get; set; } = 10;
            public int MaxPageSize { get; set; } = 50;
            public bool EnablePagination { get; set; } = true;
        }
    }
}
