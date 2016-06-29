using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using eQuiz.Web.Models;
using Microsoft.Owin.Security.Facebook;
using Owin.Security.Providers.LinkedIn;

namespace eQuiz.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            var facebookAuthenticationOptions = new FacebookAuthenticationOptions
            {
                AppId = "1740017632953675",
                AppSecret = "23770d968b1b333d14c909637b9517b9",
      
            };

            app.UseFacebookAuthentication(facebookAuthenticationOptions);

            //app.UseLinkedInAuthentication(new LinkedInAuthenticationOptions
            //{
            //    ClientId = "Your API Key",
            //    ClientSecret = "Your secret key",
            //});

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "947709765832-34vvl7d7b3t1203dj59ok0bkf33hg6qn.apps.googleusercontent.com",
                ClientSecret = "WF2cmZ2uQzKltaWho0heAYSv",
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = async context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("GoogleAccessToken", context.AccessToken));
                        foreach (var claim in context.User)
                        {
                            var claimType = string.Format("urn:google:{0}", claim.Key);
                            string claimValue = claim.Value.ToString();
                            if (!context.Identity.HasClaim(claimType, claimValue))
                                context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Google"));
                        }
                    }
                }
            });
        }
    }
}