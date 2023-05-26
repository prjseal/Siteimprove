using Newtonsoft.Json.Linq;
using Siteimprove.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Siteimprove.Services
{
    public class SiteImproveSettingsService : ISiteImproveSettingsService
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoVersion _umbracoVersion;

        public SiteImproveSettingsService(IScopeProvider scopeProvider, IUmbracoVersion umbracoVersion)
        {
            _scopeProvider = scopeProvider;
            _umbracoVersion = umbracoVersion;
        }

        public SiteImproveSettingsModel SelectTopRow()
        {
            try
            {
                using (var scope = _scopeProvider.CreateScope(autoComplete: true))
                {
                    var sql = scope.SqlContext.Sql().SelectTop(1).From<SiteImproveSettingsModel>();
                    var selectResult = scope.Database.ExecuteScalar<SiteImproveSettingsModel>(sql);
                    return selectResult;
                }
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public void Insert(SiteImproveSettingsModel row)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.Database.Insert(row);
            }
        }

        public void Update(SiteImproveSettingsModel row)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var sql = scope.Database.Update(row);
            }
        }

        /// <summary>
        /// Returns the token that exist in the first row
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public async Task<string> GetToken()
        {
            var result = SelectTopRow();
            if (result == null)
            {
                // Token did not exist in database, fetch from SiteImprove
                string token = await RequestTokenAsync();

                var row = new SiteImproveSettingsModel { Token = token };
                Insert(row);

                return token;
            }

            return result.Token;
        }

        /// <summary>
        /// Updates the token in the first row, if row not created => create it
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public async Task<string> GetNewToken()
        {
            var row = SelectTopRow();
            if (row == null)
            {
                return await GetToken();
            }

            row.Token = await RequestTokenAsync();
            Update(row);

            return row.Token;
        }

        private async Task<string> RequestTokenAsync()
        {
            using (var client = new HttpClient())
            {
                string response = await client.GetStringAsync(Constants.SiteImproveTokenUrl + "?cms=" + _umbracoVersion.Version);
                return JObject.Parse(response).GetValue("token").ToString();
            }
        }

        public string RequestToken()
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync(Constants.SiteImproveTokenUrl + "?cms=" + _umbracoVersion.Version).Result;
                return JObject.Parse(response).GetValue("token").ToString();
            }
        }
    }
}
