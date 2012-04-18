<script type="text/javascript">
        $(document).ready(function () {
            var theme = '';

            var url = "/data/{[(FILE)]}";
            }

            // prepare the data
            var source =
            {
                datatype: "json",
                datafields: [
                    { name: 'total', type: 'int'},
                    { name: 'totala', type: 'int'},
                    { name: 'totala', type: 'int'},					
                    { name: 'totalsum'},
                    { name: 'totalsuma'},
                    { name: 'totalsumr'},										
                    { name: 'totalsumd'},
                    { name: 'totalsumdd'},
                    { name: 'clicks', type: 'int'},
                    { name: 'shows', type: 'int'},
                    { name: 'expenses'}															
                ],
                id: 'id',
                url: url
            };

            $("#jqxgrid").jqxGrid(
            {
                width: 670,
                source: source,
                theme: theme,
                columnsresize: true,
                columns: [
                  { text: 'Заказы', datafield: 'total', width: 250},
                  { text: 'Под. заказы', datafield: 'totala', width: 250 },
                  { text: 'Отказы', datafield: 'totalr', width: 180 },
                  { text: 'Общ. сумма заказов', datafield: 'totalsum', width: 120 },
                  { text: 'Сумма под. заказов', datafield: 'totalsuma', minwidth: 120 },
                  { text: 'Сумма отказов', datafield: 'totalsumr', minwidth: 120 },
                  { text: 'Реал. сумма заказов', datafield: 'totalsumd', minwidth: 120 },
                  { text: 'Сумма скидок', datafield: 'totalsumdd', minwidth: 120 },
                  { text: 'Клики', datafield: 'ckicks', minwidth: 120 },	
                  { text: 'Показы', datafield: 'shows', minwidth: 120 },	
                  { text: 'Расходы', datafield: 'expenses', minwidth: 120 },				  				  			  				  				               ]
            });        
        });
    </script>