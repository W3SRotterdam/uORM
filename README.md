# W3S Umbraco Object Relation Mapper

## Idea!

We wanted to use strong typed objecs in Umbraco with Dependency Injection:

<pre>
public ActionResult HomePage(HomePageModel model) {
    return View(model);
}
</pre>

Instead of the default Umbraco way (route hijacking http://our.umbraco.org/documentation/Reference/Mvc/custom-controllers):
<pre>
public ActionResult HomePage(RenderModel model) {
    return View(model);
}
</pre>

As a solution we build the W3S Umbraco Object Relation Mapper (in short uORM).

## Installation Umbraco Object Relation Mapper
Use NuGet Package Manager Console:
PM> Install-Package W3S-uORM 

Or download W3S.uORM.dll and add a reference your project. (you will also need, interfaces.dll, Umbraco.Core.dll and umbraco.dll as reference).

Create your own Umbraco ApplicationEventHandler and add the binder for the Dependency Injection:

<pre>
public class UmbracoApplication : ApplicationEventHandler {
        public UmbracoApplication() { }
        protected override void ApplicationStarted(UmbracoApplicationBase httpApplication, ApplicationContext applicationContext) {
            base.ApplicationStarting(httpApplication, applicationContext);

            ModelBinders.Binders.DefaultBinder = new W3S.uORM.RenderModelBinder(); 
        }
    }
</pre>


## Basic usage 
Install the uORM.dll and create your document type in Umbraco. Properties in that document type must be the same as your object. (in this simple example we've only created the title alias (case doesn't matter)). 

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

And your controller:

<pre>
public ActionResult HomePage(HomePageModel model) {
    return View(model);
}
</pre>
And there you go! A nice object with all the properties set for your use! If you still need to use the current IPublishedContent you can find it in the property Content (model.Content).

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

Here's a document type containing a Title and a Highlight that a multiple media picker. In the object you define these properties the same a the document type. By adding an attribute [W3S.UmbracoMedia] the uORM knows that it needs to find the relation in the media. If you have a single picker (no multiple) just remove the List&lt;BannerModel&gt; and add BannerModel. This is also possible with [W3S.UmbracoContent], same principe!

The last item in the object are NewsDetail items that need to be displayed on the homepage. [W3S.UmbracoDescendants("NewsDetail", 1)] finds all the descendants with the document type "NewsDetail" from ancestorofself(1). It's that easy!

W3S uORM is recursive, which means that complex objects with nested objects can also be set (watch out not to create a loop!).

These are the diffent attributes

* [W3S.uORM.UmbracoMedia] finds the comma separeted values in media;
* [W3S.uORM.UmbracoContent] finds the comma separeted values in content;
* [W3S.uORM.NoUmbraco] does nothing with this attribute;
* [W3S.uORM.UmbracoLink] this is a object for related link property;
* [W3S.uORM.UmbracoDescendants(String contentTypeAlias)] find all the descendants from current node with a specifiek document type alias;
* [W3S.uORM.UmbracoDescendants(String contentTypeAlias, Int32 ancestorOrSelf)] find all the descendants from a ancestor or self with a specifiek document type alias.

##Helper objects
As some media types / json objects are already defined in Umbraco we added those objects in the uORM.

* W3S.UmbracoModels.FileModel, Umbraco Media type File;
* W3S.UmbracoModels.ImageModel, Umbraco Media type Image;
* W3S.UmbracoModels.LinkModel, object for related links ([W3S.UmbracoLink]);

##More then only load the object?
If you want more than just an object to be loaded you have to override the load functions like this: 

<pre>
public class HomePageModel : uModel&lt;HomePageModel&gt; {
        public String Title { get; set; }

        public override HomePageModel Load(Umbraco.Core.Models.IPublishedContent model) {
            return base.Load(model);
        }
    }
</pre>

##Save method
If you've got a form in Umbraco and want to save the content to Umbraco you can use the save method. 
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

Just use the Umbraco way to post data (http://our.umbraco.org/documentation/Reference/Mvc/forms) and after that use the save method to save. In this example we made a new node with email as title, document type "SignUp" and with parentnode 4817. An existing object can be saved without any parameters like this:

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
* SaveAndPublish(), saves and publishes the current object;
* SaveAndPublish(String nodeName, String documentTypeAlias, Int32 parentId), saves and publishes a new object with the document type and parentNodeId;
* SaveAndPublish(String nodeName, String documentTypeAlias, Int32 parentId, Int32 userId), saves and publishes a new object with the document type, parentNodeId by a custom user.

## Todo

* Endless loops need to be detected and must throw a execption;
* Save function can only save document type;
* Save is not recursive, it can only save simple object;
 
## History 
The first reflection part was written by Pawel Choroszcak from W3S. Edwin van Koppen (also W3S) extended the object with recursive methods and save functions.

## Common error's
<pre>
Object of type 'System.Web.HtmlString' cannot be converted to type 'System.String'.
</pre>
Rich text fields need to be of the type HtmlString


