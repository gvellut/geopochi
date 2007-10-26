-module(gp_image).
-compile(export_all).

out(Args) ->
    process(yaws_api:parse_query(Args),4). %%try 3 times to download the file

process(_,0) ->
    get_image_from_file("cache/empty.png","png");
process(ArgL,N) ->
    {value,{_,Image}} = lists:keysearch("i",1,ArgL),
    case lists:keysearch("pr",1,ArgL) of %%to cache the image : in case the fs is case insensitive
	{value,{_,undefined}} ->
	    Prefix = "";
	{value,{_,Prefix}} ->
	    ok
    end,
    CachedImage = "cache/" ++ Prefix ++ Image,
    {value,{_,Type}} = lists:keysearch("t",1,ArgL),
    case filelib:is_file(CachedImage) of
	true ->
	    get_image_from_file(CachedImage,Type);
	false ->
	    case catch(http:request(get,
			      {gp_data:image_url_data() ++ Image,[{"Referer","http://visubeta.geoportail.fr"}]},
			      [{autoredirect, false}],
			      [{stream,CachedImage}])) of
		{ok, saved_to_file} ->
		    get_image_from_file(CachedImage,Type);
		{ok,_} ->
		    get_image_from_file("cache/empty.png","png");
		_ -> process(ArgL,N-1) %% try again
	    end
    end.
    
get_image_from_file(CachedImage,Type) ->
    {ok, Content} = file:read_file(CachedImage),
    {content,"image/" ++ Type,Content}.
    
