//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Sample.Hubs;
//using Sample.Models;
//using TableDependency.SqlClient.Base.Enums;
//using TableDependency.SqlClient;
//using Microsoft.AspNetCore.SignalR;

//namespace Sample.TableDependencies
//{
//    public class UserTableDependency
//    {
//        private readonly string _connectionString;
//        private readonly IHubContext<TableDependencyHub> _hubContext;

//        public UserTableDependency(IConfiguration configuration, ILogger<UserTableDependency> logger, ChatHub chatHub)
//        {
//            _configuration = configuration;
//            _logger = logger;
//            _chatHub = chatHub;
//        }

//        public void StartTableDependency()
//        {
//            string connectionString = _configuration.GetConnectionString("DefaultConnection");

//            _tableDependency = new SqlTableDependency<AppUser>(connectionString);
//            _tableDependency.OnChanged += TableDependency_OnChanged;
//            _tableDependency.OnError += TableDependency_OnError;
//            _tableDependency.Start();

//            _logger.LogInformation("User table dependency started.");
//        }

//        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<AppUser> e)
//        {
//            if (e.ChangeType != ChangeType.None)
//            {
//                _logger.LogInformation($"User table change detected: {e.ChangeType}");
//                _chatHub.GetAllUsers();
//            }
//        }

//        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
//        {
//            _logger.LogError($"Table dependency error: {e.Error.Message}");
//        }
//    }
//}