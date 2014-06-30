using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace W3S.uORM {
    public class uApplication : ApplicationEventHandler {
        public uApplication() { }
        protected override void ApplicationStarted(UmbracoApplicationBase httpApplication, ApplicationContext applicationContext) {
            base.ApplicationStarting(httpApplication, applicationContext);

            ModelBinders.Binders.DefaultBinder = new W3S.uORM.RenderModelBinder();
        }
    }
}