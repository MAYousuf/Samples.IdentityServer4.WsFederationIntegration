// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer.Configuration;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.WsFederation.Configuration;
using Rsk.WsFederation.Models;
using System.Collections.Generic;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace DuendeIdP
{
    public class Startup
    {
        private static readonly Client RelyingParty = new Client
        {
            ClientId = "rp1",
            AllowedScopes = { "openid", "profile" },
            RedirectUris = { "https://localhost:5001/signin-wsfed" },
            RequireConsent = false,
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        };

        private static readonly Client RelyingParty1 = new Client
        {
            ClientId = "https://localhost:44313/",
            AllowedScopes = { "openid", "profile" },
            RedirectUris = { "https://localhost:44313/signin-wsfed" },
            RequireConsent = false,
            ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new[] {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiResources(new List<ApiResource>())
                .AddInMemoryClients(new List<Client> { RelyingParty, RelyingParty1 })
                .AddWsFederationPlugin(options =>
                {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDQtMTFUMDA6MDA6MDMuOTcwNzIyOSswMTowMCIsImlhdCI6IjIwMjEtMDMtMTJUMDA6MDA6MDMiLCJvcmciOiJERU1PIiwiYXVkIjozfQ==.Q2ilORcBkIwwAMCcDkkc4JsRKaCJnFkOkxycgEhK9XKu/D+jHjrHiWUHz302BIPVzvS5JHL2FJHQ7QtF3zcgoo5lVzm7VQVfl5I6GHjx1QCUdhVAFEwtJu3005gQlJ1iCMjandIljSbo7hFnffpS06B1aQTD8TGojF1hJP3xLfzBrnIZ4L5CRWk3gWQBrESpJ2H6TH4j2iSZAZ2NvFgjtbcebjcrSZfmUjhNcMdUrmgpQDPpPZm4KRU1BlWHW8BSyW9XIq+HpyMyifKI4Jkf+CWqI2/AWo+R/sufKnKfJWlXd2gCp9gUUi5BRV1JBTOvkDyvpCnRHCSuITB8Tn5zOC5KuzCg5Nbeg/hELMIS8VcC3B6Qx+gUfePESnNsn5C7xsLRRfDu6FaCVt/F0QWhi8JtSg1rgWvEJyYqnt+78bcDZ4lLva3xKTK66N3+bgYaujF2enp5bgOGGvRT7j/ZSFnt8UbDYgXRfIQNTj4XXlTparehEZ0n7s1EqJBy2H0EBPXLYcruj1bz3UmAdsqj+cS2W03ERWm3qIPiR40oirXT+RKTdp0cfRLSaVzhUE0RUypmscNF7NlLuMiIKqM3iLZLGryUHQRSlpJ0GnDBYHvdleXhgHyK9zaoBu53ivtsmkkDEkaLaNHEfA9U+IibL2Elj+JgSV23u2dAKC4N0N4=";
                })
                .AddInMemoryRelyingParties(new List<RelyingParty>());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer()
                .UseIdentityServerWsFederationPlugin();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}