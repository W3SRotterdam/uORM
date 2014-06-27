using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace W3S.uORM {
    /// <summary>
    /// Do nothing with uOrm
    /// </summary>
    public class NoUmbraco : Attribute { }
    /// <summary>
    /// Is Umbraco media object and find data
    /// </summary>
    public class UmbracoMedia : Attribute { }
    /// <summary>
    /// Is Umbraco content object and find data
    /// </summary>
    public class UmbracoContent : Attribute { }
    /// <summary>
    /// Is Umbraco link object and find data
    /// </summary>
    public class UmbracoLink : Attribute { }
    /// <summary>
    /// Is Umbraco descendants and find data with a contenttypealias and optional ancestororself level
    /// </summary>
    public class UmbracoDescendants : Attribute {
        public String ContentTypeAlias { get; set; }
        public Int32 AncestorOrSelf { get; set; }

        public UmbracoDescendants(String contentTypeAlias) {
            this.ContentTypeAlias = contentTypeAlias;
        }
        public UmbracoDescendants(String contentTypeAlias, Int32 ancestorOrSelf) {
            this.ContentTypeAlias = contentTypeAlias;
            this.AncestorOrSelf = ancestorOrSelf;
        }
    }

}