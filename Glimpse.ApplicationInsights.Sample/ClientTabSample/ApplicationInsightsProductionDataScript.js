(function ($, pubsub, util, data, renderEngine)
{
    var isSelected = false,
        generateAddress = function (paramIdKey)
        {
            var currentMetadata = data.currentMetadata();
            return util.uriTemplate(
                currentMetadata.resources.glimpse_applicationinsights_sample_applicationinsightsproductiondataresource, { 'paramIdKey': paramIdKey });
        },
        setup = function (args)
        {
            args.newData.data.inventory = { name: 'MyCustomTab', data: '', isPermanent: true };
            args.newData.metadata.plugins.inventory = {};
        },
        build = function (args)
        {
            request(args.panel, 1);
            request(args.panel, 2);
            request(args.panel, 3);

            return;
        },
        request = function (panel, paramIdKey)
        {
            $.ajax({
                url: generateAddress(paramIdKey),
                type: 'GET',
                contentType: 'application/json',
                success: function (result)
                {
                    layout(panel, result);
                }
            });
        },
        layout = function (panel, result) {
            var divElement = document.getElementById('aiDiv');
            if (divElement == undefined)
                divElement = "<div id='aiDiv' />";

            divElement.innerHTML = divElement.innerHTML + 'test' + result.a;

            panel.html(divElement);
            // panel.html("<div id=""aiDiv"">My server data</b>:" + result.a);

            
        },
        activate = function ()
        {
            isSelected = true;
        },
        deactivate = function ()
        {
            isSelected = false;
        };


    pubsub.subscribe('action.panel.hiding.inventory', deactivate);
    pubsub.subscribe('action.panel.showing.inventory', activate);
    pubsub.subscribe('action.data.initial.changed', setup);
    pubsub.subscribe('action.panel.rendered.inventory', build);
})(jQueryGlimpse, glimpse.pubsub, glimpse.util, glimpse.data, glimpse.render.engine);