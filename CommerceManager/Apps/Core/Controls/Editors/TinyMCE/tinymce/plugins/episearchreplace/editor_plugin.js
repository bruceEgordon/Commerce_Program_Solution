(function(b,a){b.create("tinymce.plugins.episearchreplace",{init:function(e,c){function d(f){e.windowManager.open({file:c+"/searchreplace.htm",width:420+parseInt(e.getLang("searchreplace.delta_width",0)),height:183+parseInt(e.getLang("searchreplace.delta_height",0)),inline:1,auto_focus:0},{mode:f,search_string:e.selection.getContent({format:"text"}),plugin_url:c});}e.addCommand("mceSearch",function(){d("search");});e.addCommand("mceReplace",function(){d("replace");});e.addButton("search",{title:"searchreplace.search_desc",cmd:"mceSearch"});e.addButton("replace",{title:"searchreplace.replace_desc",cmd:"mceReplace"});e.addShortcut("ctrl+f","searchreplace.search_desc","mceSearch");},getInfo:function(){return{longname:"Search/Replace modified by EPiServer",author:"Moxiecode Systems AB/EPiServer AB",authorurl:"http://www.episerver.com",infourl:"http://www.episerver.com",version:b.majorVersion+"."+b.minorVersion};}});b.PluginManager.add("episearchreplace",b.plugins.episearchreplace);}(tinymce,epiJQuery));