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