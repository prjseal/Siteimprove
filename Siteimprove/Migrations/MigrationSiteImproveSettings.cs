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
    public class MigrationSiteImproveSettings : MigrationBase
    {
        private readonly ISiteImproveSettingsService _siteImproveSettingsService;

        public MigrationSiteImproveSettings(IMigrationContext context, ISiteImproveSettingsService siteImproveSettingsService)
            : base(context)
        {
            _siteImproveSettingsService = siteImproveSettingsService;
        }

        protected override void Migrate()
        {
            if (!TableExists(Constants.SiteImproveDbTable))
            {
                Create.Table<SiteImproveSettingsModel>(false).Do();
                _siteImproveSettingsService.Insert(GenerateDefaultModel());
                return;
            }

            var row = _siteImproveSettingsService.SelectTopRow();
            if (row != null && !row.Installed)
            {
                Create.Table<SiteImproveSettingsModel>(true).Do();
            }

            if (row == null)
            {
                _siteImproveSettingsService.Insert(GenerateDefaultModel());
            }
        }

        private SiteImproveSettingsModel GenerateDefaultModel()
        {
            return new SiteImproveSettingsModel
            {
                Installed = true,
                Token = _siteImproveSettingsService.RequestToken()
            };
        }
    }
}
