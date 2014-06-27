using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace W3S.UmbracoModels {
    public class LinkModel {
        public string Caption { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }

        public static List<LinkModel> List(object field) {
            List<LinkModel> links = new List<LinkModel>();
            UmbracoHelper helper = new UmbracoHelper(UmbracoContext.Current);

            foreach(var item in (JArray)field) {
                LinkModel link = new LinkModel();
                link.Url = (item.Value<bool>("isInternal")) ? helper.NiceUrl(item.Value<int>("internal")) : item.Value<string>("link");
                link.Target = item.Value<bool>("newWindow") ? " target=\"_blank\"" : string.Empty;
                link.Caption = (item.Value<string>("caption"));

                links.Add(link);
            }
            if(links.Count() > 0) {
                return links;
            } else {
                return null;
            }
        }
        public static LinkModel Get(object field) {
            List<LinkModel> list = List(field);
            if(list != null) {
                return list.First();
            } else {
                return null;
            }
        }
    }
}