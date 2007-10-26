-module(gp_geo).
-compile(export_all).
-include("gp_struct.hrl").

-define(Rapp,35).

project({LL1,LL2},#proj{type=equirectangular,params={R,Fi1,_,_,_}}) ->
    {LL1 *  math:cos(Fi1 * math:pi() / 180)* R * math:pi() / 180, 
     LL2 * R * math:pi() / 180}.

unproject({XY1,XY2},#proj{type=equirectangular,params={R,Fi1,_,_,_}}) ->
    {XY1/(math:cos(Fi1 * math:pi() / 180) * R) * 180/math:pi(),
     XY2/R * 180 /math:pi()}.

tile_num(XY,Ratio,S) ->
    {TSX,TSY} = tile_size(Ratio,S),
    {gp_util:floor(element(1,XY) / TSX),
     gp_util:floor(element(2,XY) / TSY)}.

tile_size(Ratio,S) ->
    {RX,RY} = lists:nth(S,Ratio),
    {256/(100 * RX),256/(100 * RY)}.

