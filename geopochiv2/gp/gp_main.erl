-module(gp_main).
-compile(export_all).
-include("gp_struct.hrl").

out(Args) ->
    process(yaws_api:parse_query(Args)).  

process(ArgL) ->
    {value,{_,KTerr}} = lists:keysearch("t",1,ArgL),
    {value,{_,KLay}} = lists:keysearch("l",1,ArgL),
    {value,{_,S}} = lists:keysearch("s",1,ArgL),
    AKTerr = list_to_atom(KTerr),
    AKLay = list_to_atom(KLay),
    IS = list_to_integer(S),
    create_document(AKTerr,AKLay,IS,lists:keysearch("x",1,ArgL),lists:keysearch("y",1,ArgL)).

create_document(KTerr,KLay,S,{value,{_,X}},{value,{_,Y}}) ->
    Terr = gp_data:terr_data(KTerr),
    Lay = gp_data:lay_data(KTerr,KLay),
    gp_util:format_kml([{'Document',create_tile_description(S,list_to_integer(X),list_to_integer(Y),
							    Terr,Lay,KTerr,KLay)}]).

create_tile_description(S,I,J,Terr,Lay,KTerr,KLay) ->
    Size = gp_geo:tile_size(Terr#terr.ratios,S),
    XY1 = {I * element(1,Size) , J * element(2,Size)},
    XY2 = {(I+1) * element(1,Size) ,(J+1) * element(2,Size)},
    {West,South} = gp_geo:unproject(XY1,Terr#terr.proj),
    {East,North} = gp_geo:unproject(XY2,Terr#terr.proj),
    Image = Lay#lay.id ++ 
	gp_util:scale_encode(S,gp_data:crypt_data(nbr)) ++
	gp_util:coords_encode({I,J},gp_data:crypt_data(sign),gp_data:crypt_data(nbr),gp_data:crypt_data(xy))  ++
	"." ++ Lay#lay.format,
    MinLodPixels= if S == Lay#lay.smax ->
			  0;
		     true -> 360
		  end,
    [gp_util:create_region(West,East,North,South,MinLodPixels,-1) |
     create_network_links(S-1,I,J,XY1,XY2,Terr,Lay,KTerr,KLay) ++
     [gp_util:create_ground_overlay(West,East,North,South,
				   integer_to_list(I)++ integer_to_list(J),Image,Lay#lay.format)]
     
    ].

create_network_links(S,_,_,_,_,_,Lay,_,_) when S < Lay#lay.smin ->
    [];
create_network_links(S,I,J,XY1,XY2,Terr,_,KTerr,KLay) ->
    case element(1,lists:nth(S,Terr#terr.ratios)) / element(1,lists:nth(S+1,Terr#terr.ratios)) of
	Z when Z >1.999, Z<2.001 -> %% around 2
	    [gp_util:create_network_link(S,I2,J2,Terr,KTerr,KLay) || I2 <- [I*2,I*2 + 1], J2<-[J*2,J*2+1]];
	_ -> 
	    NM1 = gp_geo:tile_num(XY1,Terr#terr.ratios,S),
	    {N2,M2} = gp_geo:tile_num(XY2,Terr#terr.ratios,S),
	    NM2 = {N2+1,M2+1},
	    [gp_util:create_network_link(S,I2,J2,Terr,KTerr,KLay) || 
	      I2 <- lists:seq(element(1,NM1),element(1,NM2)), 
	      J2 <- lists:seq(element(2,NM1),element(2,NM2))]
	    
    end.
