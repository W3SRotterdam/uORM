# W3S Umbraco Object Relation Mapper

## Basic usage

Wouldn't it be nice if you can work strong typed in Umbraco? So no more code like this:

<pre>
public ActionResult HomePage(RenderModel model) {
    return View(model);
}
</pre>
But like this:
<pre>
public ActionResult HomePage(HomePageModel model) {
    return View(model);
}
</pre>
And that you get your own object with all the properties set! Wouldn't that be nice.. now you can! Introducing W3S uORM.

Install the uORM.dll and just make your document type in Umbraco with in this example just with a title alias (alias and property must be the same name (case don't matter). 

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
And there you go! A nice object with all the properties set ready for your use!

p.s. you need to use route hijacking for this http://our.umbraco.org/documentation/Reference/Mvc/custom-controllers

##Relations?

But what about relations? Lets see a somewhat bigger object.

<pre>
public class HomePageModel : uModel&lt;HomePageModel&gt; {
    public String Title { get; set; }
    [W3S.UmbracoMedia]
    public List&lt;BannerModel&gt;  Highlight { get; set; }
    [W3S.UmbracoDescendants("NewsDetail", 1)]
    public List&lt;NewsDetailModel&gt;  NewsItems { get; set; } 
}
</pre>

Here you see a document type containing a Title and a Highlight that is multiple media picker. In the object you see that properties back. By adding a attribute [W3S.UmbracoMedia] the uORM knows that it needs to find the relation in the media. If you have a single picker (no multiple) just remove the List&lt;BannerModel&gt; and add BannerModel. This is also possible with [W3S.UmbracoContent], same principe!

The last item in the object are NewsDetail items that need to be displayed on the homepage. [W3S.UmbracoDescendants("NewsDetail", 1)] finds all the descendants with the document type "NewsDetail" from ancestorofself(1). It's that easy!
