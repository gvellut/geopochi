-module(gp_root).
-compile(export_all).
-include("gp_struct.hrl").

out(Args) ->
    process(yaws_api:parse_query(Args)).    

process(ArgL) ->
    {value,{_,KTerr}} = lists:keysearch("t",1,ArgL),
    {value,{_,KLay}} = lists:keysearch("l",1,ArgL),
    create_root_document(list_to_atom(KTerr),list_to_atom(KLay)).

create_root_document(KTerr,KLay) ->
    Data = gp_data:lay_data(),
    Terr = dict:fetch(KTerr,Data),
    Lay = dict:fetch(KLay,Terr#terr.layers),
    Smax = Lay#lay.smax,
    NM1 = gp_geo:tile_num({Lay#lay.x1,Lay#lay.y1},Terr#terr.ratios,Smax),
    {N2,M2} = gp_geo:tile_num({Lay#lay.x2,Lay#lay.y2},Terr#terr.ratios,Smax),
    NM2 = {N2+1,M2+1},
    {West,South} = gp_geo:unproject({Lay#lay.x1,Lay#lay.y1},Terr#terr.proj),
    {East,North} = gp_geo:unproject({Lay#lay.x2,Lay#lay.y2},Terr#terr.proj),
    gp_util:format_kml(
      [{'Document',
	[gp_util:create_region(West,East,North,South,0,-1) |
	 [gp_util:create_network_link(Smax,I,J,Terr,KTerr,KLay) || 
	     I <- lists:seq(element(1,NM1),element(1,NM2)), 
	     J <- lists:seq(element(2,NM1),element(2,NM2))]
	]}
      ]).
