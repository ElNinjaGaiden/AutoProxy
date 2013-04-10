function ApiConfig(url, contentType, dataType, includeActionName) {
    this.url = url;
    this.contentType = contentType; //the type of data to send
    this.dataType = dataType; //the type of data we are waiting back

    //A possible routing configuration could make this a valid 'GET'
    //http://api.mydomain.com/user/userId=123
    //for this cases you need to omit the action name from the url by turning off this flag
    this.includeActionName = includeActionName;
}

ApiConfig.prototype = {
    constructor: ApiConfig
};