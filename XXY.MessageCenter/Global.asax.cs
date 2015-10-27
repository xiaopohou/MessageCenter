﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XXY.Common;
using XXY.Common.MVC;
using XXY.MessageCenter.Metadatas;

namespace XXY.MessageCenter {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            HttpRuntimeSetting.SameAppDomainAppId();

            RouteTable.Routes.MapMvcAttributeRoutes();
            //没有Area 不需要这一句
            //AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngingSetting.Config();
            LangConfig.Config();
            DisplayModelConfig.Config();

            ModelBinders.Binders.DefaultBinder = new SmartModelBinder();
            AnnorationHelper.AutoMap();
        }
    }
}
