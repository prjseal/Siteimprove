using Siteimprove.Models;
using Siteimprove.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Siteimprove.Migrations
{
    public class MigrationSiteImproveUrlMap : MigrationBase
    {
        private readonly ISiteImproveSettingsService _siteImproveSettingsService;

        public MigrationSiteImproveUrlMap(IMigrationContext context, ISiteImproveSettingsService siteImproveSettingsService)
            : base(context)
        {
            _siteImproveSettingsService = siteImproveSettingsService;
        }

        protected override void Migrate()
        {
            if (!TableExists(Constants.SiteImproveUrlMapDbTable))
                Create.Table<SiteImproveUrlMap>().Do();
        }
    }
}
