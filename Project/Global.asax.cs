using Project.Data;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Project
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Bootstrapper.Initialise();

            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

            //MyDbContext dbContext = MyDbContext.GetDbContext(); dbContext.AddToDb();
        }

        protected void FormsAuthentication_OnAuthenticate(Object sender, FormsAuthenticationEventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;

                        //let us extract the roles from our own custom cookie
                        MyDbContext dbContext = MyDbContext.GetDbContext();
                        User user = dbContext.Users.Find(email);

                        roles = user.Role;

                        //Let us set the Pricipal with our user specific details
                        e.User = new System.Security.Principal.GenericPrincipal(
                                    new System.Security.Principal.GenericIdentity(email, "Forms"), roles.Split(';'));

                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
                else
                {
                    string roles = string.Empty;
                    roles = "Public";
                    e.User = new System.Security.Principal.GenericPrincipal(
                                new System.Security.Principal.GenericIdentity("Public", "Forms"), roles.Split(';'));
                }
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        string email = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
                        string roles = string.Empty;

                        MyDbContext dbContext = MyDbContext.GetDbContext();
                        User user = dbContext.Users.Find(email);
                        roles = user.Role;
                        //let us extract the roles from our own custom cookie


                        //Let us set the Pricipal with our user specific details
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                                                   new System.Security.Principal.GenericIdentity(email, "Forms"), roles.Split(';'));

                    }
                    catch (Exception)
                    {
                        //somehting went wrong
                    }
                }
                else
                {
                    string roles = string.Empty;
                    roles = "Public";
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                                               new System.Security.Principal.GenericIdentity("Public", "Forms"), roles.Split(';'));
                }
            }
        } 

        protected void Session_Start()
        {
            //AddToDb();
            Session["Cart"] = new List<Order>();
            Session["Restaurant"] = new Restaurant();
            Session["Bill"] = 0;
        }
    }
}
