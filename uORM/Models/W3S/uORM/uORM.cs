using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using W3S.UmbracoModels;
using W3S.uORM;

namespace W3S.uORM {
   
    public class uOrm {
        #region finders
        /// <summary>
        /// Find related content items
        /// </summary>
        /// <param name="JsonId">comma separated value with id's</param>
        /// <returns>list with related IPublishedContent</returns>
        public static IEnumerable<IPublishedContent> GetRelatedList(Object JsonId) {
            if(JsonId == null) return null;
            UmbracoHelper help = new UmbracoHelper(UmbracoContext.Current);
            return help.TypedContent(JsonId.ToString().Split(',')).OfType<IPublishedContent>();
        }
        /// <summary>
        /// Find related content item
        /// </summary>
        /// <param name="JsonId">id from item</param>
        /// <returns>related IPublishedContent</returns>
        public static IPublishedContent GetRelatedItem(Object JsonId) {
            if(JsonId == null) return null;
            UmbracoHelper help = new UmbracoHelper(UmbracoContext.Current);
            return help.TypedContent(Convert.ToInt32(JsonId));
        }
        /// <summary>
        /// Find related media items
        /// </summary>
        /// <param name="JsonId">comma separated value with id's</param>
        /// <returns>list with related IPublishedContent</returns>
        public static IEnumerable<IPublishedContent> GetRelatedListMedia(Object JsonId) {
            if(JsonId == null) return null;
            UmbracoHelper help = new UmbracoHelper(UmbracoContext.Current);
            return help.TypedMedia(JsonId.ToString().Split(','));
        }
        /// <summary>
        /// Find related media item
        /// </summary>
        /// <param name="JsonId">id from item</param>
        /// <returns>related IPublishedContent</returns>
        public static IPublishedContent GetRelatedItemMedia(Object JsonId) {
            if(JsonId == null) return null;
            UmbracoHelper help = new UmbracoHelper(UmbracoContext.Current);
            return help.TypedMedia(Convert.ToInt32(JsonId));
        }
        #endregion
        #region helpers
        public static List<T> CurrentDataList<T>(IPublishedContent pCurrentPage, String pDescendants, Int32 pTop = 0) where T : new() {
            UmbracoHelper help = new UmbracoHelper(UmbracoContext.Current);
            List<T> outPut = new List<T>();
            if(pTop != 0) {
                outPut = ToListModel<T>(help.TypedContent(pCurrentPage.Id).Descendants(pDescendants).OrderBy(p => p.SortOrder).ToList().Take(pTop));
            } else {
                outPut = ToListModel<T>(help.TypedContent(pCurrentPage.Id).Descendants(pDescendants).OrderBy(p => p.SortOrder).ToList());
            }
            return outPut;
        }
        public static T CurrentData<T>(IPublishedContent pCurrentPage, String pDescendants) where T : new() {
            UmbracoHelper help = new UmbracoHelper(UmbracoContext.Current);
            return ToModel<T>(help.TypedContent(pCurrentPage.Id).Descendant(pDescendants));
        }
        /// <summary>
        /// method to find all related list objects
        /// </summary>
        /// <typeparam name="T">object for in list</typeparam>
        /// <param name="list">IPublishedContent IEnumerable with content</param>
        /// <returns>list of objects that in the model</returns>
        public static List<T> ToListModel<T>(IEnumerable<IPublishedContent> list) where T : new() {
            List<T> listTypes = new List<T>();
            if(list != null) {
                foreach(IPublishedContent content in list) {
                    listTypes.Add(ToModel<T>(content));
                }
            }
            return listTypes;
        }
        /// <summary>
        /// Invoke generic method on runtime to determine object type
        /// </summary>
        /// <param name="method">method to invoke</param>
        /// <param name="itemType">T object to add as generic</param>
        /// <param name="parameters">object[] with parameters for to invoke object</param>
        /// <returns>object with data from invoked object</returns>
        public static object InvokeGenericMethod(string method, Type itemType, Object[] parameters) {
            if(itemType != null) {
                MethodInfo methodCall = typeof(uOrm).GetMethod(method);
                MethodInfo generic = methodCall.MakeGenericMethod(itemType);
                return generic.Invoke(null, parameters);
            }
            return null;
        }
        public static object InvokeMethod(string method, Type itemType, Type objectType, Object[] parameters) {
            // Create generic type
            Type[] typeArgs = { itemType };
            //Type constructed = objectType.MakeGenericType(typeArgs);
            Type constructed = itemType;

            // Create instance of generic type
            var myClassInstance = Activator.CreateInstance(constructed);

            // Find GetAll() method and invoke
            MethodInfo getAllMethod = constructed.GetMethod(method);
            return getAllMethod.Invoke(myClassInstance, parameters);
        }

        /// <summary>
        /// Check if class is assignable
        /// </summary>
        /// <param name="type">type to find</param>
        /// <returns></returns>
        public static bool IsGenericEnumerable(Type type, Type ofType) {
            var genArgs = type.GetGenericArguments();
            if(genArgs.Length == 1 &&
                    ofType.MakeGenericType(genArgs).IsAssignableFrom(type))
                return true;
            else
                return type.BaseType != null && IsGenericEnumerable(type.BaseType, ofType);
        }
        #endregion
        /// <summary>
        /// place all the data from IPublishedContent in the object
        /// </summary>
        /// <typeparam name="T">object that is related to content</typeparam>
        /// <param name="content">IPublishedContent from Umbraco</param>
        /// <returns>model with data</returns>
        public static T ToModel<T>(IPublishedContent content) where T : new() {
            PropertyInfo[] propertyInfoArr = typeof(T).GetProperties();
            T newT = new T();
            if(content != null) {
                ICollection<IPublishedProperty> propertyCollection = content.Properties;
                Dictionary<String, String> aliasCollection = new Dictionary<String, String>();
                //get all aliases from Umbraco
                foreach(IPublishedProperty property in propertyCollection) {
                    aliasCollection.Add(property.PropertyTypeAlias.ToLower(), property.PropertyTypeAlias);
                }

                foreach(PropertyInfo propertyInfo in propertyInfoArr) {
                    if(!Attribute.IsDefined(propertyInfo, typeof(NoUmbraco))) {
                        //fill content with node parameters
                        String propName = null;
                        if(aliasCollection.ContainsKey(propertyInfo.Name.ToLower())) {
                            propName = aliasCollection[propertyInfo.Name.ToLower()];
                        }
                        Type type = propertyInfo.PropertyType;

                        if("content".Contains(propertyInfo.Name.ToLower())) {
                            propertyInfo.SetValue(newT, content);
                        } else if(Attribute.IsDefined(propertyInfo, typeof(UmbracoDescendants))) {
                            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                                UmbracoDescendants umbracoDescendants = propertyInfo.GetCustomAttribute<UmbracoDescendants>();
                                Type itemType = type.GetGenericArguments()[0];
                                if(umbracoDescendants.AncestorOrSelf == 0) {
                                    propertyInfo.SetValue(newT, InvokeGenericMethod("ToListModel", itemType, new Object[] { content.DescendantsOrSelf(umbracoDescendants.ContentTypeAlias) }));
                                } else {
                                    propertyInfo.SetValue(newT, InvokeGenericMethod("ToListModel", itemType, new Object[] { content.AncestorOrSelf(umbracoDescendants.AncestorOrSelf).DescendantsOrSelf(umbracoDescendants.ContentTypeAlias) }));
                                }
                            }
                        } else if(content[propName] != null && content.GetProperty(propName).HasValue) {
                            //is umbraco media type
                            if(Attribute.IsDefined(propertyInfo, typeof(UmbracoMedia))) {
                                //get filename from filetype must be name of umbracoFile!
                                if(propertyInfo.PropertyType == typeof(String)) {
                                    IPublishedContent file = GetRelatedItemMedia(content.GetProperty(propName).Value);
                                    if(file != null && file.GetProperty("umbracoFile") != null && file.GetProperty("umbracoFile").HasValue) {
                                        propertyInfo.SetValue(newT, file.GetProperty("umbracoFile").Value.ToString());
                                    }
                                } else {
                                    //other types
                                    if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                                        //get list object type
                                        Type itemType = type.GetGenericArguments()[0];
                                        propertyInfo.SetValue(newT, InvokeGenericMethod("ToListModel", itemType, new Object[] { GetRelatedListMedia(content.GetProperty(propName).Value) }));
                                    } else {
                                        propertyInfo.SetValue(newT, InvokeGenericMethod("ToModel", type, new Object[] { GetRelatedItemMedia(content.GetProperty(propName).Value) }));
                                    }
                                }
                                //is umbraco content
                            } else if(Attribute.IsDefined(propertyInfo, typeof(UmbracoContent))) {
                                if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                                    //get list object type
                                    Type itemType = type.GetGenericArguments()[0];
                                    propertyInfo.SetValue(newT, InvokeGenericMethod("ToListModel", itemType, new Object[] { GetRelatedList(content.GetProperty(propName).Value) }));
                                } else {
                                    propertyInfo.SetValue(newT, InvokeGenericMethod("ToModel", type, new Object[] { GetRelatedItem(content.GetProperty(propName).Value) }));
                                }
                                //is umbraco link
                            } else if(Attribute.IsDefined(propertyInfo, typeof(UmbracoLink))) {
                                if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                                    propertyInfo.SetValue(newT, LinkModel.List(content.GetProperty(propName).Value));
                                } else {
                                    propertyInfo.SetValue(newT, LinkModel.Get(content.GetProperty(propName).Value));
                                }
                            } else {
                                //base types
                                if(content.GetProperty(propName).HasValue) {
                                    propertyInfo.SetValue(newT, content.GetProperty(propName).Value);
                                }
                            }
                        } else {
                            if(propertyInfo.GetSetMethod() != null) {
                                propertyInfo.SetValue(newT, null);
                            }
                        }
                    }
                }
            }
            return newT;
        }
        /// <summary>
        /// Save and/or publish existing object to database
        /// </summary>
        /// <typeparam name="T">Object to be saved</typeparam>
        /// <param name="applicationContext">Current applicationContext</param>
        /// <param name="nodeName">Name of node</param>
        /// <param name="model">Model to be saved</param>
        /// <param name="documentTypeAlias">Document type alias in Umbraco </param>
        /// <param name="parentId">Parent node in tree</param>
        /// <param name="userId">UserId that saves the node</param>
        /// <param name="publish"></param>
        /// <returns>Succesfull save</returns>
        public static Boolean Save<T>(ApplicationContext applicationContext, String nodeName, Object model, String documentTypeAlias, Int32 parentId, Int32 userId, Boolean publish) {
            PropertyInfo[] propertyInfoArr = typeof(T).GetProperties();
            Type objectType = typeof(T);
            Boolean changed = false;
            IContent current = null;

            if(String.IsNullOrEmpty(nodeName)) {
                IPublishedContent node = (IPublishedContent)objectType.GetProperty("Content").GetValue(model, null);
                if(node == null) {
                    return false;
                }
                current = applicationContext.Services.ContentService.GetById(node.Id);

                foreach(IPublishedProperty publishedProperty in node.Properties) {
                    PropertyInfo propertyInfo = objectType.GetProperty(publishedProperty.PropertyTypeAlias, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if(propertyInfo != null) {
                        if(propertyInfo.PropertyType == typeof(HtmlString)) {
                            current.SetValue(publishedProperty.PropertyTypeAlias, (String)propertyInfo.GetValue(model, null));
                        } else {
                            current.SetValue(publishedProperty.PropertyTypeAlias, propertyInfo.GetValue(model, null));
                        }
                        changed = true;
                    }
                }
            } else {
                IContent parent = applicationContext.Services.ContentService.GetById(parentId);
                current = applicationContext.Services.ContentService.CreateContent(nodeName, parent, documentTypeAlias, userId);
                PropertyCollection propertyCollection = current.Properties;

                foreach(Property property in propertyCollection) {
                    PropertyInfo propertyInfo = objectType.GetProperty(property.Alias, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if(propertyInfo != null) {
                        if(propertyInfo.PropertyType == typeof(HtmlString)) {
                            current.Properties[property.Alias].Value = (String)propertyInfo.GetValue(model, null);
                        } else {
                            current.Properties[property.Alias].Value = propertyInfo.GetValue(model, null);
                        }
                        changed = true;
                    }
                }
            }
            if(changed) {
                if(publish) {
                    applicationContext.Services.ContentService.SaveAndPublishWithStatus(current);
                } else {
                    applicationContext.Services.ContentService.Save(current);
                }
            }
            return changed;
        }
    }
}