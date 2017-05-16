AutoProxy
=========

Tool for auto generation of javascript proxy libraries to consume web api's  

If you are a web developer probably we share some toughts like:  
  
*  Web API's are extremely powerful and easy to work
*  jQuery is awesome and it'll be in the most cases the choosen tool on the client side
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
but we are still repeating all that "configuration" code.  

jQuery shorthand methods are fine but they don't help us reaching a standard on the client side.  

The idea
--------

AutoProxy is a javascript proxy generator. Its intention is to create javascript proxy libraries based on the definition of a web api and the specified needs of the user. Some characteristics:  

*  You can define the subset of the web api controllers to take in consideration when creating the library
*  If you do not define a controllers subset, it will look for all the api controllers into all the dll's of the current app domain
*  You can configure also if you want to save the library on the file system, the file name/output, if the file should be compressed or not and the *namespace* of your web api (we will discuss the importance of this later)   
*  You can include custom javascript files into the final library
*  Due its philosophy is the OOP paradigm, the client side configuration can be done both at instance level and at *class* (prototype) level, o even at a library level
*  It allows the convivence of several proxy libraries for those sites which needs to consume several web api's

The goal is implement OOP on the client side as well as creates a library/libraries which help us to standarize the way we interact with our servers.

Having a controller like this:  

    public class PersonsController : ApiController
    {
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
    }
  
Your calls can be implemented like this:  

    $(document).ready(function() {
      var proxy = new PersonsProxy();
    
      //Get all
      proxy.GetAll(callback);
    
      //Get one
      var request = { id: 2 };
      proxy.Get(request).done(callback);
    
      //New
      var newPerson = { ID: 10, Name: 'Jhon' };
      proxy.Save(newPerson).done(callback);
      
      //Other
      var otherRequest = {};
      proxy.OtherThing(otherRequest).done(callback);
      
      function callback(response) {
          console.log(response);    
      }
    });
    
About the generation
--------------------

Once you have set you desired configuration, you just need yo create the generator and call the `ResolveProxies()`:  

    //Note: the default constructor is using the configuration file as input
    var proxyGenerator = new AutoProxy.ProxyGenerator();
    proxyGenerator.ResolveProxies();

Keeping those lines in the Application_Start of your Global.asax makes your library to be updated everytime the api gets up, but you can
choose your own strategy and invoke the generator when you need it.

Alias support
-------------

The tool uses by default the controller name and each function name to define the prototype name and members,
but you can change that behavior by decorating those elements with the `[AutoProxyAlias("YourDesiredAlias")]` attribute,
just like this:

    [AutoProxyAlias("MyController")]
    public class PersonsController : ApiController
    {
        [AutoProxyAlias("MyFunction")]
        public IEnumerable<Person> GetAll()
        {
            ...
        }
    }

And then we would be talking about something like this:

    $(document).ready(function() {
      var proxy = new MyController();
    
      //"Get all"
      proxy.MyFunction().done(callback);
      
      function callback(response) {
          console.log(response);
      }
    });

The AutoProxyAlias attribute is defined in the AutoProxy.Annotations namespace.

Ignoring elements
-----------------

There is also this `AutoProxyIgnore` attribute that you can use in order to ignore controllers and/or action when creating your proxy library

    public class PersonsController : ApiController
    {
      [HttpPost]
	  [AutoProxyIgnore]
      public Response OtherThing([FromBody]SomeRequest value)
      {
          //This action will be ignored by AutoProxy
      }
    }
	
Or:

	[AutoProxyIgnore]
	public class PersonsController : ApiController
    {
      //The entire controller will be ignored by AutoProxy and it won't be part of your proxy library
    }

The library architecture
----------------
Each library created by autoproxy follows a two-level hierarchy:

    class BaseProxy //(*)
        class ControllerProxy

This means: each proxy in this library inherits from a base local proxy. 

This give us three available levels of configuration on the client side:
* At library level: all controllers into this library will inherit this configuration
* At proxy class level: all instances of this proxy class will inherit this configuration
* At instance level: applies only for the current instance

### (*) Namespacing 

Each created library could be signed with a 'namespace'. The direct effect of this on your library is that your base proxy class name will include your namespace as prefix, for instance, if your namespace is "MyApi", your base proxy class will be named "MyApiBaseProxy" .

We will see this in deep into the *Configuration on the client side* section.

Client side configuration
------------------------

Firts of all, let's take a look at the available configuration settings:

* url: to set the web api base url. Default: ''
* contentType: to set the data type you will be sending to the server. Default: 'application/x-www-form-urlencoded; charset=UTF-8'
* dataType: to set the data type you will be expecting back from the server. Default: 'json'
* includeActionName: to specify if controller action should be added to the url (some people prefer to configure their controllers as the very basic REST form). Default: true
* timeout: 0
* crossDomain: Default: false

This configuration can be done at prototype definition level or at proxy instance level. 

The AutoProxy libraries architecture offers a very flexible configuration methodology.
As we already mentioned, we have three levels of configuration, let's see and example of each one:

### Library level

Each library contains a local base proxy class. This configuration is made at prototype definition level over this base class.

Configuration made at this level will be inherited by all proxies contained into this library.
Example:

    $(document).ready(function () {
        //Library included on this example has no namespace configured
        //That's why base proxy class is called just "BaseProxy"
        BaseProxy.prototype.dataType = 'xml';
    });
    
This means ALL proxies contained into the imported proxy library will be expecting xml from the server.

Lets keep in mind that the base proxy class name could be affected by the libary *namespace* setting. For instance, if you create a libary with "MyApi" as namespace, the base proxy class for this library will be called "MyApiBaseProxy", and the the previous configuration should me made like this:

    $(document).ready(function () {
        //This library does have a namespace
        MyApiBaseProxy.prototype.dataType = 'xml';
    });
    
This is useful for applications consuming several api's. Using this approach, you will be able to set a properly configuration for each different api you are consuming without affecting the other ones.

For instance, your application consume 2 api's:

* HumanResources api. Namespace = 'HumanResources'
* Orders api. Namespace = 'Orders'

Let's say you will be expecting XML from the HumanResources api and JSON from the Orders api. This configuration can be achieved like this:

    HumanResourcesBaseProxy.prototype.url = 'http://server1/hr/api';
    HumanResourcesBaseProxy.prototype.dataTtpe = 'xml';
    
    OrdersBaseProxy.prototype.url = 'http://server2/orders/api';
    OrdersBaseProxy.prototype.dataTtpe = 'json';
    
### Proxy level

This configuration is made at the given proxy prototype definition level and applies only for instances of this proxy class.

Example:

    PersonsProxy.prototype.dataType = 'json';
    PersonsProxy.prototype.includeActionName = false;
    
At this point we have covered the three levels of configuration that are available at prototype definition level.

The idea is make this configuration "global" and then we just need to instanciate the proxies to communicate with the api's. Something like:

    <script type="text/javascript" src="../../Scripts/proxy/ProxyGlobalConfiguration.js" ></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //Create the proxy
            var personsProxy = new PersonsProxy();

            //Pull all
            personsProxy.GetAll().done(callback);

            function callback(response) {
                //Your logic...
            }
        });
    </script>

### Instance level

There is one last level of configuration: instance level. Due configuration settings are defined at the prototype definition of the base proxy class, each proxy instance inherits this properties and you can set them at instance level:

    <script type="text/javascript">
        $(document).ready(function () {
            //Create the proxy
            var personsProxy = new PersonsProxy();
            personsProxy.url = '/api/';
            personsProxy.dataType = 'json';

            //Pull all
            personsProxy.GetAll().done(callback);

            function callback(response) {
            }
        });
    </script>

The strategy you choose to configure your proxies is up to you.

Client side use convention
------------------------------

### Parameters

All the api call methods generated into the libraries follow this parameters convention:

* Methods without parameters: ([optional]context)
* Methods with parameters: (request, [optional]context)

Examples: 

    $(document).ready(function() { 
        //Create the proxy
        var proxy = new SomeProxy();
        
        //Sending parameters, default context for callbacks
        var proxy.FunctionOne({ param1: 10, param2: 20 });
        
        //Sending parameters, some context for callbacks
        var context = this;
        var proxy.FunctionOne({ param1: 10, param2: 20 }, context);
        
        //No parameters, default context for callbacks
        var proxy.FunctionTwo();
        
        //No parameters, some context for callbacks
        var proxy.FunctionTwo(context);
        
        //NOTE: context is optional always!
    });

### Callbacks

For callbacks, AutoProxy simply returns the jqXHR (XMLHttpRequest) object returned by the ajax call, which implements the Promise interface (see [Deferred Object](http://api.jquery.com/category/deferred-object/) for more information).
This way you can configure one or more callbacks for different situations, like this:

    $(document).ready(function () {
        //Create the proxy
        var personsProxy = new PersonsProxy();

        //Pull all
        var r = personsProxy.GetAll().done(function () { })
                                    .fail(function () { })
                                    .always(function () { });
                            
        
        //Second callback
        r.done(secondDoneCallback);
        
        function secondDoneCallback() {
        }
    });
    
The execution context within all callbacks is the context passed to the proxy function.

[jQueryAjax](http://api.jquery.com/jQuery.ajax/) is your friend. Go there if you need go deeper.

Dependencies
------------

AutoProxy depends on this packages:  

*  [jQuery](https://nuget.org/packages/jQuery/) on the client side
*  [AjaxMin](https://nuget.org/packages/AjaxMin/) on the server side

Demo
----

You can find a demo of AutoProxy [here](https://github.com/ElNinjaGaiden/AutoProxyDemo)

Contact
-------

You can contact me at diegogarcia@yoinbol.com