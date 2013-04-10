function BaseProxy(namespace, controllerName, config) {
    //this.apiAddress = apiAddress != null && apiAddress != '' ? apiAddress : autoproxy.baseUrl;
    this.namespace = namespace;
    this.controllerName = controllerName;

    this._config = config;
}

BaseProxy.prototype = {
    constructor: BaseProxy,

    GetUrl: function (actionName, config) {
        //Creates the url based on the api location, the controller and the action
        if (config.includeActionName) {
            return [config.url, this.controllerName, actionName].join("/");
        }
        else {
            return [config.url, this.controllerName].join("/");
        }
    },

    ExecReq: function (webActionType, actionName, request, callback, context, carryover) {
        //Grab the config for the corresponding api
        var config = this._config;
        if (!config) {
            config = autoproxy.GetConfig(this.namespace);
        }
        //Executes a server action request
        $.ajax(
        {
            url: this.GetUrl(actionName, config),
            type: webActionType,
            dataType: config.dataType,
            contentType: config.contentType,
            data: config.contentType.indexOf('json') !== -1 && request != null ? JSON.stringify(request) : request,
            context: context,
            success: function (response) {
                //Execute the callback function
                if (typeof callback === 'function') {
                    callback(response, carryover);
                }
            },
            failure: function () {
                //Execute the callback function
                if (typeof callback === 'function') {
                    //Create an error response
                    var response = {
                        Result: -1,
                        ResultMessage: 'Communication error'
                    };
                    callback(response, carryover);
                }
            }
        });
    }
};