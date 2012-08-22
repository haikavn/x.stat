function GenerateChart(id, data)
{
	var width = $("#" + id).width();
	var height = $("#" + id).height();
		
	var w = width / data.length;
	
	var i;
	var l = data.length;
	var x = 0;
	var y = 0;
	for (i = 0; i < l; i++)
	{
		jQuery('<div/>', { 
			id: 'col' + i, 
			style: 'position: absolute; width: ' + w + 'px; height: 25px; left: ' + x + 'px; top: ' + y + 'px; border:1px solid #BDB76B;'
		}).appendTo("#" + id); 
		x += w;
	}
}