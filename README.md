# W3S Umbraco Object Relation Mapper

## Idea?

Want to use strong typed objecs in Umbraco with Dependency Injection? Like this:

<pre>
public ActionResult HomePage(HomePageModel model) {
    return View(model);
}
</pre>

And not the default Umbraco way like this (route hijacking http://our.umbraco.org/documentation/Reference/Mvc/custom-controllers):
<pre>
public ActionResult HomePage(RenderModel model) {
    return View(model);
}
</pre>

We at W3S liked that idea so we build W3S Umbraco Object Relation Mapper (in short uORM). 

## Basic usage
Install the uORM.dll and make your document type in Umbraco. Properties in that document type must be the same as your object. (in this example a title alias (case don't matter)). 

Then create a strong typed object like this:
<pre>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using W3S.uORM;

namespace ProjectName.Models {
    public class HomePageModel : uModel&lt;HomePageModel&gt; {
        public String Title { get; set; }
    }
}
</pre>
And there you go! A nice object filled with all the properties set for your use!

##Relations?

But what about relations? Lets take a bigger object.

<pre>
public class HomePageModel : uModel&lt;HomePageModel&gt; {
    public String Title { get; set; }
    [W3S.UmbracoMedia]
    public List&lt;BannerModel&gt;  Highlight { get; set; }
    [W3S.UmbracoDescendants("NewsDetail", 1)]
    public List&lt;NewsDetailModel&gt;  NewsItems { get; set; } 
}
</pre>

Here's a document type containing a Title and a Highlight that is multiple media picker. In the object you see that properties back. By adding a attribute [W3S.UmbracoMedia] the uORM knows that it needs to find the relation in the media. If you have a single picker (no multiple) just remove the List&lt;BannerModel&gt; and add BannerModel. This is also possible with [W3S.UmbracoContent], same principe!

The last item in the object are NewsDetail items that need to be displayed on the homepage. [W3S.UmbracoDescendants("NewsDetail", 1)] finds all the descendants with the document type "NewsDetail" from ancestorofself(1). It's that easy!

W3S uOrm is recursive so also a complex object with nested objects are set (so look out not to create a loop!).

These are the diffent attributes

* [W3S.UmbracoMedia] find the comma separeted values in media;
* [W3S.UmbracoContent] find the comma separeted values in content;
* [W3S.NoUmbraco] do nothing with this attribute;
* [W3S.UmbracoLink] this is a related link object (need to be converted from JSON to object);
* [W3S.UmbracoDescendants(String contentTypeAlias)] find all the descendants from current node with a specifiek document type alias;
* [W3S.UmbracoDescendants(String contentTypeAlias, Int32 ancestorOrSelf)] find all the descendants from a ancestor or self with a specifiek document type alias.

##More then only load the object?
If you want more then a object to be loaded you have to override the load functions like this: 

<pre>
public class HomePageModel : uModel&lt;HomePageModel&gt; {
        public String Title { get; set; }

        public override HomePageModel Load(Umbraco.Core.Models.IPublishedContent model) {
            return base.Load(model);
        }
    }
</pre>

##Save method
If you got a form in Umbraco and want to save the content to Umbraco you can use the save method. 
<pre>
[HttpPost]
public ActionResult IntakeFormPost(IntakeFormFieldsModel model) {
    if(!ModelState.IsValid) {
        return CurrentUmbracoPage();
    }
    model.Save(model.Email, "SignUp", 4817);

    return RedirectToCurrentUmbracoPage();          
}
</pre>

Just use the Umbraco way to post data (http://our.umbraco.org/documentation/Reference/Mvc/forms) and after that use the save method to save. In this example a made a new node with as title the email, document type "SignUp" and with parentnode 4817. A existing object can be save without any parameters like this:

<pre>
public ActionResult HomePage(HomePageModel model) {
            model.Title = "Change title";
            model.Save();
            return View(model);
        }
</pre>

These are the methods:

* Save(), saves the current object;
* Save(String nodeName, String documentTypeAlias, Int32 parentId), saves the current object;
* Save(String nodeName, String documentTypeAlias, Int32 parentId, Int32 userId) , saves the current object;
* SaveAndPublish(), saves and publish the current object;
* SaveAndPublish(String nodeName, String documentTypeAlias, Int32 parentId), saves and publish a new object with the document type and parentNodeId;
* SaveAndPublish(String nodeName, String documentTypeAlias, Int32 parentId, Int32 userId), saves and publish a new object with the document type, parentNodeId by a custom user.

## Todo

* Endless loop need to be detected and must throw a execption;
* Save function can only save document type;
* Save is not recursive, can only save simple object;
 
## History 
First reflection part was written by Pawel Choroszcak from W3S. Edwin van Koppen (also W3S) extended the object with recursive methods and save functions.

## Common error's
<pre>
Object of type 'System.Web.HtmlString' cannot be converted to type 'System.String'.
</pre>
Rich text fields need to be of the type HtmlString


