; (function($, window, document, undefined) {
	var pluginName = "manhole",
			defaults = {
				endpointUrl: "",
				prompt: '> ',
				name: 'manhole',
				greetings: null
			};
	function Plugin(element, options) {
		this.element = element;
		this.options = $.extend({}, defaults, options);
		this._defaults = defaults;
		this._name = pluginName;
		this.init();
	}
	Plugin.prototype = {
		init: function() {
			var endpointUrl = this.options.endpointUrl;
			$(this.element).terminal(function(command, term) {
				term.pause();
				$.ajax(endpointUrl, { type: "POST", data: command, contentType: "text/plain", dataType: "json" })
					.success(function (data) {
						if (data)
							term.echo(JSON.stringify(data, null, "  "));
						term.resume();
					})
					.error(function(xhr, status, type) {
						term.error($.terminal.encode(xhr.responseText).replace(/\<br\/\>/g, "\n"));
						term.resume();
					})
			}, this.options)
		}
	};
	$.fn[pluginName] = function(options) {
		return this.each(function() {
			if(!$.data(this, "plugin_" + pluginName)) {
				$.data(this, "plugin_" + pluginName, new Plugin(this, options));
			}
		});
	};
})(jQuery, window, document);