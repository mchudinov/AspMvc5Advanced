﻿using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NLog;
using Repositories;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using UseCases;

namespace Gui
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            #region SimpleInjector

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);
            container.Register<IWidgetRepository, WidgetRepository>(Lifestyle.Scoped);
            container.Register<IUserCase, UserCase>(Lifestyle.Scoped);
            container.Register<IWidgetCase, WidgetCase>(Lifestyle.Scoped);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

            #endregion
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                StringBuilder err = new StringBuilder();
                err.Append("Error caught in Application_Error event\n");
                err.Append("Error in: " + (Context.Session == null ? string.Empty : Request.Url.ToString()));
                err.Append("\nError Message:" + ex.Message);
                if (null != ex.InnerException)
                    err.Append("\nInner Error Message:" + ex.InnerException.Message);
                err.Append("\n\nStack Trace:" + ex.StackTrace);
                Server.ClearError();

                if (null != Context.Session)
                {
                    //Session[SessionIdHolder.FEILSIDE_CALLSTACK] = Regex.Replace(err.ToString(), @"\r\n?|\n", "<br />");
                    //HttpContext.Current.Response.StatusCode = (int) HttpStatusCode.Moved;
                    //HttpContext.Current.Response.Redirect(url, true);

                    err.Append(
                        $"Session_Start. Identity name:[{Thread.CurrentPrincipal.Identity.Name}] IsAuthenticated:{Thread.CurrentPrincipal.Identity.IsAuthenticated}");
                }

                var routeData = new RouteData();
                routeData.Values.Add("controller", "ErrorPage");
                routeData.Values.Add("action", "Error");
                routeData.Values.Add("exception", ex);

                if (ex.GetType() == typeof(HttpException))
                {
                    routeData.Values.Add("statusCode", ((HttpException)ex).GetHttpCode());
                }
                else
                {
                    routeData.Values.Add("statusCode", 500);
                }
                _log.Error(err.ToString());
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var identity = Thread.CurrentPrincipal.Identity;
            _log.Info("Session_Start. Identity name:[{0}] IsAuthenticated:{1}", identity.Name, identity.IsAuthenticated);
        }

        protected void Session_End(object sender, EventArgs e) {}
    }
}
