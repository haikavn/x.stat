<script type="text/javascript">       
				var data = new Array(); 
$(document).ready(function () {
	            var theme = getTheme();              
				
				// prepare the data            
				var i = 0;
				var row = 0;   
				        
				{[(DATA)]}        
				
				var source = {localdata: data, datatype: "array"};            
				$("#jqxgrid").jqxGrid({width: 670, source: source, theme: theme,                
				columns: [                  
				{ text: 'Период', datafield: 'period', width: 50 },                  
				{ text: 'Заказы', datafield: 'total', width: 100 },                  
				{ text: 'Под. заказы', datafield: 'totala', width: 100 },                  
				{ text: 'Отказы', datafield: 'totalr', width: 180 },                  
				{ text: 'Сумма заказов', datafield: 'totalsum', width: 80, cellsalign: 'right' },                  
	            { text: 'Сумма под. заказов', datafield: 'totalsuma', width: 80, cellsalign: 'right', cellsformat: 'c2' }, 
				{ text: 'Сумма отказов', datafield: 'totalsumr', width: 80, cellsalign: 'right', cellsformat: 'c2' },  
 				{ text: 'Реал. сумма заказов', datafield: 'totalsumd', width: 80, cellsalign: 'right', cellsformat: 'c2' },            
 				{ text: 'Сумма скидок', datafield: 'totalsumdd', width: 80, cellsalign: 'right', cellsformat: 'c2' },
				{ text: 'Клики', datafield: 'clicks', width: 50 },  
				{ text: 'Показы', datafield: 'shows', width: 50 },  
    			{ text: 'Расходы', datafield: 'expenses', width: 50, cellsalign: 'right', cellsformat: 'c2' }                
				]            
				});        
				
});    
				</script>