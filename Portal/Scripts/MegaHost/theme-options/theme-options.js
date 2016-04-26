// THEME OPTIONS.JS
//--------------------------------------------------------------------------------------------------------------------------------
//This is JS file that contains skin, layout Style and bg used in this template*/
// -------------------------------------------------------------------------------------------------------------------------------
// Template Name: Megahost Template.
// Author: Iwthemes.
// Name File: theme-options.js
// Version 1.3 - Update on 29 May 2014
// Website: http://www.iwthemes.com 
// Email: support@iwthemes.com
// Copyright: (C) 2014
// -------------------------------------------------------------------------------------------------------------------------------

  $(document).ready(function($) {

	/* Selec your skin_version, skin_color, layout Style And bg pattern */
	function interface(){

		// Skin Version value
	    var skin_version = "style"; // style (default), style-dark 

	    // Skin Color value
	    var skin_color = "red"; 	// red (default), green ,yellow,purple,blue, orange, purple, pink, cocoa, custom 

	    // Boxed value
	    var layout = "layout-wide"; // layout-wide ( default) ,layout-boxed, layout-boxed-margin 

	    //Only in boxed version 
	    var bg = "none";  // none (default), bg1, bg2, bg3, bg4, bg5, bg6, bg7, bg8, bg9, bg10, bg11, bg12, bg13, 
	    				  // bg14, bg15, bg16, bg17, bg18, bg19,bg20, bg21, bg22, bg23, bg24, bg12, bg25, bg26

	    // Theme Panel - Visible - no visible panel options
	    var themepanel = "1"; // 1 (default - visible), 0 ( No visible)

	    //$(".skin_version").attr("href", "/css/" + skin_version + ".css");
	    //$(".skin_color").attr("href", "/css/skins/"+ skin_color + "/" + skin_color + ".css");
	    //$("#layout").addClass(layout);	
	    //$("body").addClass(bg);
	    //$("#theme-options").css('opacity' , themepanel);
	    return false;
	 }
	 interface();

	//=================================== Theme Options ====================================//

	$('.wide').click(function() {
		$('.boxed').removeClass('active');
		$('.boxed-margin').removeClass('active');
		$(this).addClass('active');
		$('.patterns').css('display' , 'none');
		$('#layout').removeClass('layout-boxed').removeClass('layout-boxed-margin').addClass('layout-wide');
	});
	$('.boxed').click(function() {
		$('.wide').removeClass('active');
		$('.boxed-margin').removeClass('active');
		$(this).addClass('active');
		$('.patterns').css('display' , 'block');
		$('#layout').removeClass('layout-boxed-margin').removeClass('layout-wide').addClass('layout-boxed');
	});
	$('.boxed-margin').click(function() {
		$('.boxed').removeClass('active');
		$('.wide').removeClass('active');
		$(this).addClass('active');
		$('.patterns').css('display' , 'block');
		$('#layout').removeClass('layout-wide').removeClass('layout-boxed').addClass('layout-boxed-margin');
	});
	$('.light').click(function() {
		$('.dark').removeClass('active');
		$(this).addClass('active');
	});
	$('.dark').click(function() {
		$('.light').removeClass('active');
		$(this).addClass('active');
	});

	//=================================== Skins Changer ====================================//

	google.setOnLoadCallback(function(){

	'use strict';

	$(".light").click(function(){
	   	$(".skin_version").attr("href", "Content/MegaHost/style.css");
	    return false;
	});
	$(".dark").click(function(){
	    $(".skin_version").attr("href", "Content/MegaHost/style-dark.css");
	    return false;
	});

    // Color changer
    $(".red").click(function(){
        $(".skin_color").attr("href", "Content/MegaHost/skins/red/red.css");
	    return false;
	});
	$(".blue").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/blue/blue.css");
	    return false;
	});
	$(".yellow").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/yellow/yellow.css");
	    return false;
	});
	$(".green").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/green/green.css");
	    return false;
	});
	$(".orange").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/orange/orange.css");
    	return false;
	});
	$(".purple").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/purple/purple.css");
	    return false;
	});
	$(".pink").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/pink/pink.css");
	    return false;
	});
	$(".cocoa").click(function(){
	    $(".skin_color").attr("href", "Content/MegaHost/skins/cocoa/cocoa.css");
        return false;
   	});
 });

	//=================================== Background Options ====================================//
	
	$('#theme-options ul.backgrounds li').click(function(){
	var 	$bgSrc = $(this).css('background-image');
		if ($(this).attr('class') == 'bgnone')
			$bgSrc = "none";

		$('body').css('background-image',$bgSrc);
		$.cookie('background', $bgSrc);
		$.cookie('backgroundclass', $(this).attr('class').replace(' active',''));
		$(this).addClass('active').siblings().removeClass('active');
	});

	//=================================== Panel Options ====================================//

	$('.openclose').click(function(){
		if ($('#theme-options').css('left') == "-220px")
		{
			$left = "0px";
			$.cookie('displayoptions', "0");
		} else {
			$left = "-220px";
			$.cookie('displayoptions', "1");
		}
		$('#theme-options').animate({
			left: $left
		},{
			duration: 500			
		});

	});

	$(function(){
		$('#theme-options').fadeIn();
		$bgSrc = $.cookie('background');
		$('body').css('background-image',$bgSrc);

		if ($.cookie('displayoptions') == "1")
		{
			$('#theme-options').css('left','-220px');
		} else if ($.cookie('displayoptions') == "0") {
			$('#theme-options').css('left','0');
		} else {
			$('#theme-options').delay(800).animate({
				left: "-220px"
			},{
				duration: 500				
			});
			$.cookie('displayoptions', "1");
		}
		$('#theme-options ul.backgrounds').find('li.' + $.cookie('backgroundclass')).addClass('active');

	});

});