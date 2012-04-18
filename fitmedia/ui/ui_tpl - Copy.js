	    function loadData()
		{
			var theme = getTheme();  
						
			//$('#jqxTree').load('ui/tree.html');
			if (isfirstload)
			{
				$("#jqxButton").jqxButton({ width: '150', height: '25', theme: theme });
                // Subscribe to Click events.               
				$("#jqxButton").bind('click', function () {                    
               		refreshData();
				});
				
				$('#jqxTree').jqxTree({ hasThreeStates: true, checkboxes: true, width: '330px', theme: theme });
				
				$('#jqxTree').bind('checkchange', function (event) {
					return;
					var args = event.args;               
					var item = $('#jqxTree').jqxTree('getItem', args.element);
					
					//if (cItem != null)
					//{
						//cItem = null;
						//return;
					//}
					
					/*if (item.checked == null)
					{
						pItem = null;
						return;
					}
					
					if (pItem != null) 
					{
						if (item.parentElement != null)
						if (item.parentElement.name == pItem.element.name)
						{
							cCurIndex++;
							alert("t=" + cItemCount + " i=" + cCurIndex);
							
							if (cCurIndex == cItemCount)
							{
								cCurIndex = 0;

								rCount++;
								if (rCount == 2)
								{
									pItem = null;
									cItemCount = 0;
									rCount = 0;
								}
							}
						}
						
						//alert(item.element.name);
						return;
					}*/
					
						
							
					if (!isfirstload)
					{
						if (item.parentElement != null)
							return;
							
						alert(item.element.name);	
							
						/*if (item.parentElement == null && item.checked != null)
						{
							pItem = item;
						}
						else
							if (item.parentElement != null && item.checked != null)
							{
								//alert('kuku');
								//cItem = null;
							}*/
												
						var items = $('#jqxTree').jqxTree('getItems');
						var i;


						//if (event.isImmediatePropagationStopped()) return;
						//event.stopImmediatePropagation();
						var pids = '';
						var cids = '';	
	
						for (i = 0; i < items.length; i++)
						{
							/*if (pItem != null)
							{
								if (items[i].parentElement != null)
								{
									if (items[i].parentElement.name != pItem.element.name && !pItem.checked)
									{
										continue;
									}
									else
										if (items[i].parentElement.name == pItem.element.name)
										{
											if (pItem.checked)
												cids += items[i].element.name + ',';
											cItemCount++;
										}
								}
								else
								if (items[i].checked || items[i].checked == null)
									pids += items[i].element.name + ',';
//								alert("a="+item.element.name);
//								alert("b="+items[i].parentElement.name);
							}
							else*/
							if (items[i].checked || items[i].checked == null)
							{
								if (items[i].parentElement == null)
									pids += items[i].element.name + ',';
								else
									cids += items[i].element.name + ',';
							}
						}
							var sd = $('#startdate').find('#inputElement').val();
							var ed = $('#enddate').find('#inputElement').val();
						//if (item.parentElement == null)
						//	locked = true;
						var res = $.get("datagen.aspx?sd="+sd+"&ed=" + ed + "&pids=" + pids + "&cids=" + cids, function () {
							//alert('kuku');
							loadData();
						}); 	
					}
				//alert(item.checked);
				//alert(item.parentElement);
				//loadData();
					locked = false;
        		}); 
							
				$.getScript("ui/tree.js");
			}
			
		
				// prepare the data            
				//var i = 0;
				//var row = 0;   
				//var data = new Array();
				        
				var url = "ui/griddata.txt";            
          
				// prepare the data           
				 var source =            
				 {                
				 	datatype: "json",
					datafields: 
					[                    
						{ name: 'period' },                    
   						{ name: 'total' }, 
						{ name: 'totala' }, 
						{ name: 'totalr' }, 
						{ name: 'totalsum' }, 
						{ name: 'totalsuma' }, 
						{ name: 'totalsumr' }, 
						{ name: 'totalsumd' }, 
						{ name: 'totalsumdd' }, 
						{ name: 'clicks' }, 
						{ name: 'shows' }, 
						{ name: 'expenses' } 																																																												            		],               
					id: 'id',                
					url: url            
				};            
				
								//{[(GDATA)]}        
				
				//var source = {localdata: data, datatype: "array"}; 
				
			     var dataAdapter = 0;      
					   
				if (isfirstload)
				{				
					dataAdapter = new $.jqx.dataAdapter(source);  
					$("#jqxgrid").jqxGrid({width: 670, source: dataAdapter, theme: theme,                
					columns: [                  
					{ text: '
					', datafield: 'period', width: 50 },                  
					{ text: 'Заказы', datafield: 'total', width: 100 },                  
					{ text: 'Под. заказы', datafield: 'totala', width: 100 },                  
					{ text: 'Отказы', datafield: 'totalr', width: 180 },                  
					{ text: 'Сумма заказов', datafield: 'totalsum', width: 80, cellsalign: 'right' },                  
					{ text: 'Сумма под. заказов', datafield: 'totalsuma', width: 80, cellsalign: 'right' }, 
					{ text: 'Сумма отказов', datafield: 'totalsumr', width: 80, cellsalign: 'right' },  
					{ text: 'Реал. сумма заказов', datafield: 'totalsumd', width: 80, cellsalign: 'right' },            
					{ text: 'Сумма скидок', datafield: 'totalsumdd', width: 80, cellsalign: 'right' },
					{ text: 'Клики', datafield: 'clicks', width: 50 },  
					{ text: 'Показы', datafield: 'shows', width: 50 },  
					{ text: 'Расходы', datafield: 'expenses', width: 50, cellsalign: 'right' }                
					]            
					});
				}
				else
				{
					dataAdapter = new $.jqx.dataAdapter(source);  					
					$('#jqxgrid').jqxGrid('clear');
					$("#jqxgrid").jqxGrid({ source: dataAdapter });
					$('#jqxgrid').jqxGrid('updatebounddata');
            		$('#jqxgrid').jqxGrid('refreshdata');
				}
				
				$("#startdate").jqxDateTimeInput({ width: '250px', height: '25px', theme: theme, formatString: 'dd.MM.yyyy'  });
				$("#enddate").jqxDateTimeInput({ width: '250px', height: '25px', theme: theme, formatString: 'dd.MM.yyyy' });	
				
				$.getScript("ui/chart.js");

				
				if (isfirstload)
				{
					 $('#startdate').bind('valuechanged', function (event) {      
					 	return;          
         				var items = $('#jqxTree').jqxTree('getItems');
						var i;


						//if (event.isImmediatePropagationStopped()) return;
						//event.stopImmediatePropagation();
						var pids = '';
						var cids = '';	
	
						for (i = 0; i < items.length; i++)
						{
							if (items[i].checked || items[i].checked == null)
							{
								if (items[i].parentElement == null)
									pids += items[i].element.name + ',';
								else
									cids += items[i].element.name + ',';
							}
						}
						
						var startdate = $('#startdate').find('#inputElement').val();
						var enddate = $('#enddate').find('#inputElement').val();
						//if (item.parentElement == null)
						//	locked = true;
						var url = "datagen.aspx?sd="+startdate+"&ed=" + enddate + "&pids=" + pids + "&cids=" + cids;
												
						//alert(url);
						//ddd.dd= 0;
						var res = $.get(url, function () {
							//alert(startdate + ' ' + enddate);
							loadData();
						}); 
				  	});
					
					$('#enddate').bind('valuechanged', function (event) {   
						return;             
         				var items = $('#jqxTree').jqxTree('getItems');
						var i;


						//if (event.isImmediatePropagationStopped()) return;
						//event.stopImmediatePropagation();
						var pids = '';
						var cids = '';	
	
						for (i = 0; i < items.length; i++)
						{
							if (items[i].checked || items[i].checked == null)
							{
								if (items[i].parentElement == null)
									pids += items[i].element.name + ',';
								else
									cids += items[i].element.name + ',';
							}
						}
						
						var startdate = $('#startdate').find('#inputElement').val();
						var enddate = $('#enddate').find('#inputElement').val();
						//if (item.parentElement == null)
						//	locked = true;
						alert(pids + "____" + cids);
						var res = $.get("datagen.aspx?sd="+startdate+"&ed="+enddate+"&pids=" + pids + "&cids=" + cids, function () {
							loadData();
						}); 
				  	});
				}
 		}
		
		function refreshData()
		{
			var items = $('#jqxTree').jqxTree('getItems');
			var i;
	
	
			//if (event.isImmediatePropagationStopped()) return;
			//event.stopImmediatePropagation();
			var pids = '';
			var cids = '';	
	
			for (i = 0; i < items.length; i++)
			{
				if (items[i].checked || items[i].checked == null)
				{
					if (items[i].parentElement == null)
						pids += items[i].element.name + ',';
					else
						cids += items[i].element.name + ',';
				}
			}
			
			var startdate = $('#startdate').find('#inputElement').val();
			var enddate = $('#enddate').find('#inputElement').val();
			//if (item.parentElement == null)
			//	locked = true;
			var url = "datagen.aspx?sd="+startdate+"&ed=" + enddate + "&pids=" + pids + "&cids=" + cids;
									
			//alert(url);
			//ddd.dd= 0;
			var res = $.get(url, function () {
				//alert(startdate + ' ' + enddate);
				loadData();
			});
		}
		
		
		$(document).ready(loadData);