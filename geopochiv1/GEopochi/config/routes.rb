ActionController::Routing::Routes.draw do |map|
  map.wms 'wms', :controller => 'gp', :action => 'wms'
  map.kml ':territory/:layer/:level/:x/:y',:controller => 'gp',:action => 'kml'
  map.image 'image/:territory/:layer/:level/:x/:y',:controller => 'gp',:action => 'image'
  map.root_kml ':territory/:layer', :controller => 'gp', :action => 'kml_root'
  map.connect '', :controller => 'gp'
  map.connect ':controller/:action/:id'
end
                                 
