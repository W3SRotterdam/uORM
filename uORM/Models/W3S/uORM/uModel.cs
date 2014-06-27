using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace W3S.uORM {
    public class uModel<T> where T : new() {
        [ScriptIgnore(), XmlIgnore()]
        public IPublishedContent Content { get; set; }
        /// <summary>
        /// Fills the object with content from Umbraco
        /// </summary>
        /// <param name="model">IPublishedContent out of Umbraco </param>
        /// <returns></returns>
        public virtual T Load(IPublishedContent model) {
            return (T) uOrm.InvokeGenericMethod("ToModel", typeof(T), new Object[] { model });
        }
        /// <summary>
        /// Save existing object to database
        /// </summary>
        /// <param name="model">model to be saved</param>
        /// <returns>Succesfull save</returns>
        public virtual Boolean Save() {
            if(this.Content == null || String.IsNullOrEmpty(this.Content.DocumentTypeAlias)) {
                return false;
            }
            return uOrm.Save<T>(ApplicationContext.Current, null, this, this.Content.DocumentTypeAlias, 0, 0, false);
        }
        /// <summary>
        /// Save and publish existing object to database
        /// </summary>
        /// <param name="model">model to be saved</param>
        /// <returns>Succesfull save</returns>
        public virtual Boolean SaveAndPublish() {
            if(this.Content == null || String.IsNullOrEmpty(this.Content.DocumentTypeAlias)) {
                return false;
            }
            return uOrm.Save<T>(ApplicationContext.Current, null, this, this.Content.DocumentTypeAlias, 0, 0, true);
        }
        /// <summary>
        /// Save existing object to database with user admin (0)
        /// </summary>
        /// <param name="nodeName">Name of node</param>
        /// <param name="documentTypeAlias">Document type alias in Umbraco </param>
        /// <param name="parentId">Parent node in tree</param>
        /// <returns>Succesfull save</returns>
        public virtual Boolean Save(String nodeName, String documentTypeAlias, Int32 parentId) {
            return uOrm.Save<T>(ApplicationContext.Current, nodeName, this, documentTypeAlias, parentId, 0, false);
        }
        /// <summary>
        /// Save existing object to database
        /// </summary>
        /// <param name="nodeName">Name of node</param>
        /// <param name="documentTypeAlias">Document type alias in Umbraco </param>
        /// <param name="parentId">Parent node in tree</param>
        /// <param name="userId">UserId that saves the node</param>
        /// <returns>Succesfull save</returns>
        public virtual Boolean Save(String nodeName, String documentTypeAlias, Int32 parentId, Int32 userId) {
            return uOrm.Save<T>(ApplicationContext.Current, nodeName, this, documentTypeAlias, parentId, userId, false);
        }
        /// <summary>
        /// Save and publish existing object to database
        /// </summary>
        /// <param name="nodeName">Name of node</param>
        /// <param name="documentTypeAlias">Document type alias in Umbraco </param>
        /// <param name="parentId">Parent node in tree</param>
        /// <param name="userId">UserId that saves the node</param>
        /// <returns>Succesfull save</returns>
        public virtual Boolean SaveAndPublish(String nodeName, String documentTypeAlias, Int32 parentId, Int32 userId) {
            return uOrm.Save<T>(ApplicationContext.Current, nodeName, this, documentTypeAlias, parentId, userId, true);
        }
    }
}