﻿using Siteimprove.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Siteimprove.NotificationHandlers
{
    public class DatabaseMigrationAppStartingHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public DatabaseMigrationAppStartingHandler(IScopeProvider scopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _scopeProvider = scopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (notification.RuntimeLevel >= RuntimeLevel.Run)
            {
                var plan = new MigrationPlan("Siteimprove");

                plan.From(string.Empty)
                    .To<MigrationSiteImproveSettings>("9.9.9")
                    .To<MigrationSiteImproveUrlMap>("10.0.0");

                var upgrader = new Upgrader(plan);

                upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
            }
        }
    }
}
