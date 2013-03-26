﻿using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Signum.Engine;
using Signum.Engine.Authorization;
using Signum.Engine.Basics;
using Signum.Engine.Chart;
using Signum.Engine.ControlPanel;
using Signum.Engine.Disconnected;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Exceptions;
using Signum.Engine.Mailing;
using Signum.Engine.Maps;
using Signum.Engine.Operations;
using Signum.Engine.UserQueries;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Entities.Basics;
using Signum.Entities.Chart;
using Signum.Entities.ControlPanel;
using Signum.Entities.Disconnected;
using Signum.Entities.Mailing;
using Signum.Entities.UserQueries;
using Signum.Utilities;
using Signum.Utilities.ExpressionTrees;
using Southwind.Entities;
using Southwind.Services;
using Signum.Engine.Processes;
using Signum.Entities.Processes;
using Signum.Engine.Alerts;
using Signum.Engine.Notes;
using Signum.Engine.Cache;
using Signum.Engine.Profiler;

namespace Southwind.Logic
{

    //Starts-up the engine for Southwind Entities, used by Web and Load Application
    public static partial class Starter
    {
        public static void Start(string connectionString)
        {
            string logPostfix = Connector.TryExtractCatalogPostfix(ref connectionString, "_Log");

            SchemaBuilder sb = new SchemaBuilder(DBMS.SqlServer2012);
            sb.Schema.Version = typeof(Starter).Assembly.GetName().Version;
            sb.Schema.ForceCultureInfo = CultureInfo.GetCultureInfo("en-US");
            sb.Schema.Settings.OverrideAttributes((ExceptionDN ua) => ua.User, new ImplementedByAttribute(typeof(UserDN)));
            sb.Schema.Settings.OverrideAttributes((OperationLogDN ua) => ua.User, new ImplementedByAttribute(typeof(UserDN)));
            sb.Schema.Settings.OverrideAttributes((UserDN ua) => ua.Related, new ImplementedByAttribute(typeof(EmployeeDN)));
            sb.Schema.Settings.OverrideAttributes((UserQueryDN uq) => uq.Related, new ImplementedByAttribute(typeof(UserDN), typeof(RoleDN)));
            sb.Schema.Settings.OverrideAttributes((UserChartDN uc) => uc.Related, new ImplementedByAttribute(typeof(UserDN), typeof(RoleDN)));
            sb.Schema.Settings.OverrideAttributes((ControlPanelDN cp) => cp.Related, new ImplementedByAttribute(typeof(UserDN), typeof(RoleDN)));
            sb.Schema.Settings.OverrideAttributes((ProcessExecutionDN cp) => cp.ProcessData, new ImplementedByAttribute(typeof(PackageDN), typeof(PackageOperationDN)));
            sb.Schema.Settings.OverrideAttributes((PackageLineDN cp) => cp.Package, new ImplementedByAttribute(typeof(PackageDN), typeof(PackageOperationDN)));

            DynamicQueryManager dqm = new DynamicQueryManager();

            Connector.Default = new SqlConnector(connectionString, sb.Schema, dqm);

            CacheLogic.Start(sb);

            OperationLogic.Start(sb, dqm);

            EmailLogic.Start(sb, dqm);

            AuthLogic.Start(sb, dqm, "System", null);
            
            ResetPasswordRequestLogic.Start(sb, dqm);
            AuthLogic.StartAllModules(sb, dqm, typeof(IServerSouthwind));
            UserTicketLogic.Start(sb, dqm);
            SessionLogLogic.Start(sb, dqm);

            ProcessLogic.Start(sb, dqm, 1, userProcessSession: true);
            PackageLogic.Start(sb, dqm, true, true);

            QueryLogic.Start(sb);
            UserQueryLogic.Start(sb, dqm);
            UserQueryLogic.RegisterUserTypeCondition(sb, SouthwindGroups.UserEntities);
            UserQueryLogic.RegisterRoleTypeCondition(sb, SouthwindGroups.RoleEntities);
            ChartLogic.Start(sb, dqm);
            UserChartLogic.RegisterUserTypeCondition(sb, SouthwindGroups.UserEntities);
            UserChartLogic.RegisterRoleTypeCondition(sb, SouthwindGroups.RoleEntities);
            ControlPanelLogic.Start(sb, dqm);
            ControlPanelLogic.RegisterUserTypeCondition(sb, SouthwindGroups.UserEntities);
            ControlPanelLogic.RegisterRoleTypeCondition(sb, SouthwindGroups.RoleEntities);

            ExceptionLogic.Start(sb, dqm);

            AlertLogic.Start(sb, dqm, new []{typeof(PersonDN), typeof(CompanyDN), typeof(OrderDN)} );
            NoteLogic.Start(sb, dqm, new[] { typeof(PersonDN), typeof(CompanyDN), typeof(OrderDN) });

            EmployeeLogic.Start(sb, dqm);
            ProductLogic.Start(sb, dqm);
            CustomerLogic.Start(sb, dqm); 
            OrderLogic.Start(sb, dqm);
            ShipperLogic.Start(sb, dqm);

            TypeConditionLogic.Register<OrderDN>(SouthwindGroups.UserEntities,
                o => o.Employee.RefersTo((EmployeeDN)UserDN.Current.Related));

            TypeConditionLogic.Register<EmployeeDN>(SouthwindGroups.UserEntities,
                e => e == (EmployeeDN)UserDN.Current.Related);

            TypeConditionLogic.Register<OrderDN>(SouthwindGroups.CurrentCompany,
                o => o.Customer == CompanyDN.Current);

            TypeConditionLogic.Register<OrderDN>(SouthwindGroups.CurrentPerson,
               o => o.Customer == PersonDN.Current);

            DisconnectedLogic.Start(sb, dqm);
            DisconnectedLogic.BackupFolder = @"D:\SouthwindTemp\Backups";
            DisconnectedLogic.BackupNetworkFolder = @"D:\SouthwindTemp\Backups";
            DisconnectedLogic.DatabaseFolder = @"D:\SouthwindTemp\Database";

            SetupDisconnectedStrategies(sb);

            ProfilerLogic.Start(sb, dqm, 
                timeTracker: true, 
                heavyProfiler: true, 
                overrideSessionTimeout: true);

            SetupCache(sb);

            if (logPostfix.HasText())
                SetLogDatabase(sb.Schema, new DatabaseName(null, ((SqlConnector)Connector.Current).DatabaseName() + logPostfix));

            sb.ExecuteWhenIncluded();
        }

        private static void SetupCache(SchemaBuilder sb)
        {
            CacheLogic.CacheTable<ShipperDN>(sb);
            CacheLogic.CacheTable<ProductDN>(sb);
        }

        
        public static void SetLogDatabase(Schema schema, DatabaseName logDatabaseName)
        {
            schema.Table<OperationLogDN>().ToDatabase(logDatabaseName);
            schema.Table<ExceptionDN>().ToDatabase(logDatabaseName);
        }
    }
}
