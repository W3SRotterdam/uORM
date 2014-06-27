using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using W3S;
using W3S.uORM;

namespace W3S.UmbracoModels {
    public class ImageModel {
        public String UmbracoFile { get; set; }
        public String UmbracoExtension { get; set; }
        public String UmbracoBytes { get; set; }
        public String UmbracoWidth { get; set; }
        public String UmbracoHeight { get; set; }
    }
}