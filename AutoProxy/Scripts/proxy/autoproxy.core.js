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
    contentType: 'application/json',
    dataType: 'json',
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

    ExecReq: function (webActionType, actionName, request, callback, context, error) {
        //Executes a server action request
        $.ajax(
        {
            url: this.GetUrl(actionName),
            type: webActionType,
            dataType: this.dataType,
            contentType: this.contentType,
            data: this.contentType.indexOf('json') !== -1 && request != null ? JSON.stringify(request) : request,
            context: context,
            timeout: this.timeout,
            success: callback,
            error: error
        });
    }
};