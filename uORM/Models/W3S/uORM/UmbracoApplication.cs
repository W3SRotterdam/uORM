using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Models.Umbraco {
    public class UmbracoApplication : IApplicationEventHandler {
        public void OnApplicationInitialized(UmbracoApplicationBase httpApplication, ApplicationContext applicationContext) {
        }
        public void OnApplicationStarting(UmbracoApplicationBase httpApplication, ApplicationContext applicationContext) {
        }
        public void OnApplicationStarted(UmbracoApplicationBase httpApplication, ApplicationContext applicationContext) {
            ModelBinders.Binders.DefaultBinder = new W3S.uORM.RenderModelBinder();
        }
    }
}