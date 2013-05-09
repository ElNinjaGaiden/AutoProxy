//reference http://javascriptissexy.com/oop-in-javascript-what-you-need-to-know/
function inheritPrototype(childObject, parentObject) {
    // Crockford’s method to copy the properties and methods from the parentObject onto the childObject
    // So the copyOfParent object now has everything the parentObject has 
    var copyOfParent = Object.create(parentObject.prototype);

    //Then we set the constructor of this new object to point to the childObject.
    //This step is necessary because the preceding step overwrote the childObject constructor when it overwrote the childObject prototype (during the Object.create() process)
    copyOfParent.constructor = childObject;

    // Then we set the childObject prototype to copyOfParent, so that the childObject can in turn inherit everything from copyOfParent (from parentObject)
    childObject.prototype = copyOfParent;
}

function __namespace__BaseProxy(controllerName) {
    this.controllerName = controllerName;
}

__namespace__BaseProxy.prototype = {
    constructor: __namespace__BaseProxy,
    url: '',
    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
    dataType: 'json',
    crossDomain: false,
    includeActionName: true,
    timeout: 0,

    GetUrl: function (actionName) {
        //Creates the url based on the api location, the controller and the action
        if (this.includeActionName) {
            return [this.url, this.controllerName, actionName].join("/");
        }
        else {
            return [this.url, this.controllerName].join("/");
        }
    },

    ExecReq: function (webActionType, actionName, request, context) {
        //Prepare the data to send
        var data = request;
        if (request != null && this.contentType.indexOf('json') !== -1 && (webActionType === 'POST' || webActionType === 'PUT')) {
            data = JSON.stringify(request);
        }
        //Executes a server action request
        return $.ajax(
        {
            url: this.GetUrl(actionName), //url
            type: webActionType, //GET/POST/PUT/DELETE/OPTIONS
            dataType: this.dataType, //data type returned by the server
            contentType: this.contentType, //data type to send to the server
            data: data, //data
            context: context, //callbacks execution context
            crossDomain: this.crossDomain, // cross domain
            timeout: this.timeout //timeout
        });
    }
};