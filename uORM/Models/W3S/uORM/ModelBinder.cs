using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Web.Models;

namespace W3S.uORM {
    public class RenderModelBinder : DefaultModelBinder {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            if(controllerContext.RouteData.DataTokens.ContainsKey("umbraco")) {
                RenderModel model = controllerContext.RouteData.DataTokens["umbraco"] as RenderModel;
                Type type = bindingContext.ModelType;

                if(uOrm.IsGenericEnumerable(type, typeof(uModel<>)) && controllerContext.RequestContext.HttpContext.Request.HttpMethod == "GET") {
                    return uOrm.InvokeMethod("Load", type, typeof(uModel<>), new Object[] { model.Content });
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
}