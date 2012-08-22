				{[(LINKS)]}
				
				$('#statsChart').html('{[(SCH)]}');
				
				$("#statsTable td").hover(function () {
					$(".sum").hide();
					//$("#statsTable td").find("div").css("background-color", "");
					$("#statsTable td[title='" + $(this).attr("title") + "']").find(".sum").show();
					//$("#statsTable td[title='" + $(this).attr("title") + "']").find("div").css("background-color", "yellow");
					//$("#statsTable td[title='" + $(this).attr("title") + "']").find("div").css("color", "black");
		
				}, function () {
					$(".sum").fadeOut("fast");
					//$("#statsTable td[title='" + $(this).attr("title") + "']").find("div").css("background-color", "");
					//$("#statsTable td[title='" + $(this).attr("title") + "']").find("div").css("color", "");
		
				});					

				var chart;
				chart = new Highcharts.Chart({
				chart: {
					renderTo: '{[(CONT1)]}',
					type: 'column'
				},
				title: {
					text: '{[(TITLE1)]}'
				},
				subtitle: {
					text: '{[(SUBTITLE1)]}'
				},
				xAxis: {
					categories: [
					{[(CAT1)]}
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
						text: 'Процент %'
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
							this.x +': '+ this.y +'%';
					}
				},
				plotOptions: {
					column: {
						pointPadding: 0.2,
						borderWidth: 0
					}
				},
					series: [{[(DATA1)]}]
			});