				$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');$('<option />').attr('value', '17').text('Dondomofon').appendTo('#projects');

				var chart;
				chart = new Highcharts.Chart({
				chart: {
					renderTo: 'container',
					type: 'column'
				},
				title: {
					text: 'График за период времени'
				},
				subtitle: {
					text: '01.03.2012-06.05.2012'
				},
				xAxis: {
					categories: [
					'01.03.2012-04.03.2012','05.03.2012-11.03.2012','12.03.2012-18.03.2012','19.03.2012-25.03.2012','26.03.2012-01.04.2012','02.04.2012-08.04.2012','09.04.2012-15.04.2012','16.04.2012-22.04.2012','23.04.2012-29.04.2012','30.04.2012-06.05.2012'
					],
					labels: {
						rotation: -45,
						align: 'right',
						style: {
							font: 'normal 8px Verdana, sans-serif'
						}
					}
				},
				yAxis: {
					min: 0,
					title: {
						text: 'Сумма (руб.)'
					}
				},
				legend: {
					layout: 'vertical',
					backgroundColor: '#FFFFFF',
					align: 'left',
					verticalAlign: 'top',
					x: 800,
					y: 10,
					floating: true,
					shadow: true
				},
				tooltip: {
					formatter: function() {
						return ''+
							this.x +': '+ this.y +' руб.';
					}
				},
				plotOptions: {
					column: {
						pointPadding: 0.2,
						borderWidth: 0
					}
				},
					series: [{
name: 'Общ. сумма заказов',
data: [0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000]
},
{
name: 'Сумма под. заказов',
data: [0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000]
},
{
name: 'Сумма отказов',
data: [0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000]
},
{
name: 'Реал. сумма заказов',
data: [0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000]
},
{
name: 'Сумма скидок',
data: [0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000]
},{
name: 'Расходы',
data: [0.0000,94.4200,224.9800,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000,0.0000]
}]
			});