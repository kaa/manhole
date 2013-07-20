; (function () {
	var id = Math.floor(Math.random() * 1000000);
	document.open();
	document.write('<div id="manhole-' + id + '"></div>');
	document.close();
	jQuery(document.head).append('<link rel="stylesheet" href="$CSS_PATH$"/>');
	jQuery("#manhole-" + id).manhole({
		endpointUrl: '$ENDPOINT_PATH$'
	});
})();
