# W3S Umbraco Object Relation Mapper

## Basic usage

Wouldn't it be nice if you can work strong typed in Umbraco? So no more code like this:

public ActionResult HomePage(RenderModel model) {
    return View(model);
}

But like this:

public ActionResult HomePage(HomePageModel model) {
    return View(model);
}

And that you get your own object with all the properties set! Wouldn't that be nice.. now you can! Introducing W3S uORM.

Install the uORM.dll and just make your document type in Umbraco (in this example just with a title alias). 
Then create a strong typed object like this:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using W3S.uORM;

namespace ProjectName.Models {
    public class HomePageModel : uModel<HomePageModel> {
        public String Title { get; set; }
    }
}

And there you go! A nice object with all the properties set ready for your use!

p.s. you need to use route hijacking for this http://our.umbraco.org/documentation/Reference/Mvc/custom-controllers




