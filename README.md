AutoProxy
=========

Small tool for auto generation of javascript proxy classes for consume a web api  

If you are a web developer probably we share some toughts like:  
  
*  Web API's are extremely powerful and easy to work
*  jQuery is awesome and it'll be in the most cases the choosen tool
*  Scripting is also great, but its extreme flexibility sometimes make us lose the concept of 'order'

The symptom
-----------  

Maybe because that flexibility we mentioned, we are not used to be that careful about the order  of the code
on the client side as we are on the server side.  

Several like this is common:  

    $.ajax(
    {
        url: 'theUrl',
        type: 'GET/POST/PUT/DELETE',
        contentType: 'application/json',
        data: dataIfApplies,
        context: context,
        success: function (response) {
            //NOTES:  
            //Handling the response here compromises the readability of the execution flow and  
            //makes this segment itself difficult to reuse
        },
        failure: failCallback
    });  
  
    function failCallback() {  
    }  

The problems mentioned on the notes can be solved just doing the same thing that the failure handler is doing  
but we are still reapiting all that "configuration" code.  

jQuery shorthand methods are fine but they don't help us reaching a standard on the client side.  

The idea
--------

AutoProxy is a javascript proxy generator. It works like this:  

*  Search for all the api controllers into a given set of assemblies defined on your server side
*  Creates one proxy class for each controller found. This file is non minified (handled by configuration)  
*  Creates one single minified file containing all the generated clases (handled by configuration)  

The goal is implement OOP on the client side as well as creates a library which help us to standarize the  
way we interact with our server.

Having a controller like this:  

    public IEnumerable<Person> GetAll()
    {
        ...
    }
  
    public Person Get(string id)
    {
        ...
    }
  
    [HttpPost]
    public Response Save([FromBody]Person value)
    {
        ...
    }
  
    [HttpPost]
    public Response OtherThing([FromBody]SomeRequest value)
    {
        ...
    }  
  
Your calls can be implemented like this:  

    $(document).ready(function() {
      var proxy = new PersonProxy('yourWebApiRootAddress');
    
      //Get all
      proxy.GetAll(callback, this);
    
      //Get one
      var request = { id: 2 };
      proxy.Get(request, callback, this);
    
      //New
      var newPerson = { ID: 10, Name: 'Jhon' };
      proxy.Save(newPerson, callback, this);
      
      //Other
      var otherRequest = {};
      proxy.OtherThing(otherRequest, callback, this);
      
      function callback(response) {
          console.log(response);
      }
    });  

About the generation
--------------------

AutoProxy is going to create a single javascript file for each api controller and/or one single minified file.  
This can be handled by configuration. Only the default generator constructor uses the config file.

Once you have set you desired configuration, you just need yo create the generator and call the `ResolveProxies()`:  

    var assemblies = new List<Assembly>() { Assembly.GetExecutingAssembly() };
    var proxyGenerator = new AutoProxy.ProxyGenerator(assemblies);
    proxyGenerator.ResolveProxies();

Keeping those lines in your Global.asax makes your files to be updated everytime the api gets up, but you can
choose your own strategy and invoke the generator when you need it.

Dependencies
------------

AutoProxy depends on this packages:  

*  [jQuery](https://nuget.org/packages/jQuery/) on the client side
*  [AjaxMin](https://nuget.org/packages/AjaxMin/) on the server side
