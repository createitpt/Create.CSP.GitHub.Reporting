// THEME OPTIONS.JS
//--------------------------------------------------------------------------------------------------------------------------------
//This is JS file that contains principal fuctions of theme */
// -------------------------------------------------------------------------------------------------------------------------------
// Template Name: Megahost Template.
// Author: Iwthemes.
// Name File: main.js
// Version 1.3 - Update on 29 May 2014
// Website: http://www.iwthemes.com 
// Email: support@iwthemes.com
// Copyright: (C) 2014

$(document).ready(function($) {

	'use strict';

	//=================================== Nav Responsive =============================//

    $('#menu').tinyNav({
      active: 'selected'
  	});

  	//=================================== Sticky nav ===================================//

  	$(".search_domain").sticky({topSpacing:0});

  	//=================================== Nav Superfish ===============================//

	$('ul.sf-menu').superfish();

    //=================================== Totop  ===================================//

	$().UItoTop({ 		
		scrollSpeed:500,
		easingType:'linear'
	});

	//=================================== jBar  ===================================//
	
	$('.jBar').hide();
	$('.jRibbon').show().removeClass('up', 500);
	$('.jTrigger').click(function(){
		$('.jRibbon').toggleClass('up', 500);
		$('.jBar').slideToggle();
	});


	//=================================== Tabs Varius  ===================================//

	$(".tab_content").hide(); //Hide all content
	$("ul.tabs_varius li:first").addClass("active").show(); //Activate first tab
	$(".tab_content:first").show(); //Show first tab content
	

	//=================================== Tabs On Click Event  ===================================//

	$("ul.tabs_varius li").click(function() {
		$("ul.tabs_varius li").removeClass("active"); //Remove any "active" class
		$(this).addClass("active"); //Add "active" class to selected tab
		$(".tab_content").hide(); //Hide all tab content
		var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
		$(activeTab).fadeIn(); //Fade in the active content
		return false;
	});


	//=================================== Accordion  =================================//
	
	//$('.accordion-container').hide(); 
	//$('.accordion-trigger:first').addClass('active').next().show();
	//$('.accordion-trigger').click(function(){
	//	if( $(this).next().is(':hidden') ) { 
	//		$('.accordion-trigger').removeClass('active').next().slideUp();
	//		$(this).toggleClass('active').next().slideDown();
	//	}
	//	return false;
	//});

	//=================================== Subtmit Form  =================================//

	$('#contact-form').submit(function(event) {  
	  event.preventDefault();  
	  var url = $(this).attr('action');  
	  var datos = $(this).serialize();  
	  $.get(url, datos, function(resultado) {  
	    $('#result').html(resultado);  
	  });  
	});  

	//=================================== Form Newslleter  =================================//

    $('#newsletterForm').submit(function(event) {  
       event.preventDefault();  
       var url = $(this).attr('action');  
       var datos = $(this).serialize();  
        $.get(url, datos, function(resultado) {  
        $('#result-newsletter').html(resultado);  
      });  
    });  

	//=================================== Carousel facilities  ==================================//

    $("#carousel-facilities").owlCarousel({
       autoPlay: 2800,      
       items : 4,
       navigation: false,
       itemsDesktop : [1199,4],
       itemsDesktopSmall : [1024,3],
       itemsTablet : [990,2],
       itemsMobile : [500,1],
       pagination: true
    });

    //=================================== Carousel stories  ==================================//

    $("#carousel-stories").owlCarousel({
       autoPlay: 2800,      
       items : 3,
       navigation: false,
       itemsDesktop : [1199,3],
       itemsDesktopSmall : [1024,2],
       itemsTablet : [768,2],
       itemsMobile : [500,1],
       pagination: true
    });

    //=================================== Carousel Sponsor  ==================================//

    $("#carousel-sponsors").owlCarousel({
       autoPlay: false,      
       items : 6,
       navigation: true,
       itemsDesktop : [1199,5],
       itemsDesktopSmall : [1024,4],
       itemsTablet : [768,3],
       itemsMobile : [500,2],
       pagination: false
    });

	//=================================== Ligbox  ===========================================//	

	$(".fancybox").fancybox({
	    openEffect  : 'elastic',
	    closeEffect : 'elastic',

	    helpers : {
	      title : {
	        type : 'inside'
	      }
	    }
	});
	
	//=============================  tooltip demo ===========================================//

    $('.tooltip-hover').tooltip({
        selector: "[data-toggle=tooltip]",
        container: "body"
     });

     //================================== Scroll Efects =====================================//

  	$(window).scroll(function() {

	    $('.animation-elements').each(function(){
			var imagePos = $(this).offset().top;       
			var topOfWindow = $(window).scrollTop();
			if (imagePos < topOfWindow+800) {
       			$(this).addClass("animated fadeInDown").css('opacity' , '1');
			}
		});  
	});

	//=================================== Hover Efects =====================================//

	$('.sponsors li').hover(function() {
		$(this).toggleClass('animated pulse');
	});

	$('.item_table').hover(function() {
		$(this).toggleClass('animated bounce');
	});

	//=================================== Portfolio Filters  ==============================//

	$(window).load(function(){
     var $container = $('.portfolioContainer');
     $container.isotope({
      filter: '*',
              animationOptions: {
              duration: 750,
              easing: 'linear',
              queue: false
            }
     });
	 
    $('.portfolioFilter a').click(function(){
      $('.portfolioFilter .current').removeClass('current');
      $(this).addClass('current');
       var selector = $(this).attr('data-filter');
       $container.isotope({
        filter: selector,
               animationOptions: {
               duration: 750,
               easing: 'linear',
               queue: false
             }
        });
       return false;
      }); 
   });

	
});
	//=================================== Slide =====================================//
		
	$('#slide').camera({
		height: 'auto'	       
	});

