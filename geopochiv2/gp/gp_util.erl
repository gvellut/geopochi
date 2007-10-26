-module(gp_util).
-compile(export_all).
-include("gp_struct.hrl").


%% KML generation helpers
format_kml(Data) ->
    {content,"application/vnd.google-earth.kml+xml",
     xmerl:export_simple([{kml,[{xmlns,"http://earth.google.com/kml/2.1"}],
			   Data}],
			 xmerl_xml)}.
	

create_region(West,East,North,South,MinLodPixels,MaxLodPixels) ->
    {'Region',[{'LatLonAltBox',[{west,[float_to_list(West)]},
				 {east,[float_to_list(East)]},
				 {north,[float_to_list(North)]},
				 {south,[float_to_list(South)]}
				]},
		{'Lod',[{minLodPixels,[integer_to_list(MinLodPixels)]},
			{maxLodPixels,[integer_to_list(MaxLodPixels)]}
		       ]}
	       ]
     }.

create_ground_overlay(West,East,North,South,Pr,Image,Format) ->
    {'GroundOverlay',[{drawOrder,["21"]},
		      {'Icon',[{href,[gp_data:localhost_data() ++ "/gp_image?pr=" ++ Pr ++ "&i=" ++ Image ++ "&t=" ++ Format]}]},
		      {'LatLonBox',[{west,[float_to_list(West)]},
				    {east,[float_to_list(East)]},
				    {north,[float_to_list(North)]},
				    {south,[float_to_list(South)]}
				   ]}
		     ]}.


create_network_link(S,I,J,Terr,KTerr,KLay) ->
    Size = gp_geo:tile_size(Terr#terr.ratios,S),
    XY1 = {I * element(1,Size) , J * element(2,Size)},
    XY2 = {(I+1) * element(1,Size) ,(J+1) * element(2,Size)},
    {West,South} = gp_geo:unproject(XY1,Terr#terr.proj),
    {East,North} = gp_geo:unproject(XY2,Terr#terr.proj),
    {'NetworkLink',[create_region(West,East,North,South,360,-1),
		    {'Link',[{href,[gp_data:localhost_data() ++ "/gp_main?t=" ++ atom_to_list(KTerr) ++
				    "&l=" ++ atom_to_list(KLay) ++ "&s=" ++ integer_to_list(S) ++
				    "&x=" ++ integer_to_list(I) ++ "&y=" ++ integer_to_list(J)]},
			     {viewRefreshMode,["onRegion"]}]}
		   ]}.
	
%% Returns the integer most immediately smaller	than the argument 
floor(X) ->
    T = erlang:trunc(X),
    case (X - T) of
        Neg when Neg < 0 -> T - 1;
        _ -> T
    end.


%% Functions to encode URL of image tiles
scale_encode(S,NbrCrypt) ->
    lists:nth(S+1,NbrCrypt).

coords_encode(NM={N,M},SignCrypt,NbrCrypt,XYCrypt) ->
    LN = xy_encode(abs(N),XYCrypt),
    LM = xy_encode(abs(M),XYCrypt),
    sign_encode(NM,SignCrypt) ++ lists:nth(length(LN)+1,NbrCrypt) ++ LN ++ LM.

xy_encode(N,XYCrypt) ->
    R = N rem 62,
    if 
	N - R == 0 ->
	    lists:nth(R + 1,XYCrypt);
	true ->
	    xy_encode((N-R) div 62,XYCrypt) ++ lists:nth(R + 1,XYCrypt)
    end.

sign_encode({NM1,NM2},SignCrypt) ->
    if 
	NM1 * NM2 >= 0 ->
	    if 
		NM1 < 0 ->
		    lists:nth(4,SignCrypt);
		true ->
		    lists:nth(1,SignCrypt)
	    end;
	true ->
	    if 
		NM1 < 0 ->
		    lists:nth(3,SignCrypt);
		true ->
		    lists:nth(2,SignCrypt)
	    end
    end.

    
