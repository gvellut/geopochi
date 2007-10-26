-module(gp_index).
-compile(export_all).
-include("gp_struct.hrl").

%% Sends back a KML file pointing at the root data from all territories
out(_A) ->
    gp_util:format_kml(
      [{'Document',
	 [{name,["GEopochi v2"]} |  format_terr(dict:to_list(gp_data:lay_data()))
	 ]}]
     ).

format_terr([])	->
    [];
format_terr([{KTerr,Terr}|T]) ->
    FormatTerr = {'Folder',
 		  [{name,[Terr#terr.name]},
 		   {'Style',[{'ListStyle',[{listItemType,["radioFolder"]}]}]} |
		   format_lay(Terr,KTerr,dict:to_list(Terr#terr.layers))
 		  ]},
    [FormatTerr | format_terr(T)].

format_lay(_,_,[]) ->	
    [];
format_lay(Terr,KTerr,[{KLay,Lay}|T]) ->
    {West,South} = gp_geo:unproject({Lay#lay.x1,Lay#lay.y1},Terr#terr.proj),
    {East,North} = gp_geo:unproject({Lay#lay.x2,Lay#lay.y2},Terr#terr.proj),
    FormatLay = {'NetworkLink',
		 [{name,[Lay#lay.name]},
		  gp_util:create_region(West,East,North,South,0,-1),
		  {'Link',
		   [{href,[gp_data:localhost_data() ++ "/gp_root?t=" ++ atom_to_list(KTerr) ++ "&l=" ++ atom_to_list(KLay)]},
		    {viewRefreshMode,["onRequest"]}
		   ]}
		 ]},
    [FormatLay | format_lay(Terr,KTerr,T)].
    
		   
		   
    
