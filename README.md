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

AutoProxy is going to create this prototype:  

  function PersonProxy(apiAddress) { 
     BaseProxy.call(this, apiAddress, 'Person'); 
  } 
  
  inheritPrototype(PersonProxy, BaseProxy);
  
  PersonProxy.prototype.GetAll = function (callback, context, carryover) { 
     this.ExecuteRequest('GET', 'GetAll', null, callback, context, carryover); 
  }; 
  
  PersonProxy.prototype.Get = function (request, callback, context, carryover) { 
     this.ExecuteRequest('GET', 'Get', request, callback, context, carryover); 
  }; 
  
  PersonProxy.prototype.Save = function (request, callback, context, carryover) { 
     this.ExecuteRequest('POST', 'Save', request, callback, context, carryover); 
  }; 
  
  PersonProxy.prototype.OtherThing = function (request, callback, context, carryover) { 
     this.ExecuteRequest('POST', 'OtherThing', request, callback, context, carryover); 
  }; 
  
And then your calls can be implemented like this:  

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

Dependencies
------------

AutoProxy depends on this packages:  

*  [jQuery](https://nuget.org/packages/jQuery/) on the client side
*  [AjaxMin](https://nuget.org/packages/AjaxMin/) on the server side
