function AutoProxy() {
    //A collection of apis
    this.Apis = [];
}

AutoProxy.prototype = {
    constructor: AutoProxy,

    GetDefault: function () {
        if (this.Apis.length > 0) {
            return this.Apis[0].config;
        }
        return null;
    },

    GetConfig: function (namespace) {
        var config = $.grep(this.Apis, function (item) { return item.namespace === namespace })[0];
        if (config) {
            return config.config;
        }
        return null;
    },

    SaveConfig: function (namespace, config) {
        var actualConfig = this.GetConfig(namespace);
        if (actualConfig) {
            actualConfig.config = config;
        }
        else {
            this.Apis[this.Apis.length] = { namespace: namespace, config: config };
        }
    },

    SaveDefault: function (config) {
        if (this.Apis.length > 0) {
            this.Apis[0].config = config;
        }
    }
};