require 'geoportail_struct'
include GeoportailStruct

require 'geoportail_data'
include GeoportailData

require 'wms_data'
include WmsData

require 'proj4'
include Proj4

require 'create_image'

class GpController < ApplicationController
  session :disabled => true
  caches_page :index, :kml_root, :kml

  #generates the kml Folder hierarchy, linking to all the territories
  def index
    @territories = DATA
    @url_root = host
    
    render :content_type => "application/vnd.google-earth.kml+xml", :layout => false
  end
  
  def kml_root
    territory = params[:territory].intern
    layer = params[:layer].intern
    
    dt_terr = DATA[territory]
    dt_layer = dt_terr[layer]
        
    @name = "#{territory} #{layer}"
    @extent = dt_layer.wgs84_extent
    @url_root = host + "/#{territory}/#{layer}/#{dt_layer.top_level}/0/0"
    
    render :content_type => "application/vnd.google-earth.kml+xml", :layout => false
  end

  def kml
    territory = params[:territory].intern
    layer = params[:layer].intern
    level = params[:level].to_i
    x = params[:x].to_i
    y = params[:y].to_i
        
    dt_terr = DATA[territory]
    dt_layer = dt_terr[layer]

    @name = "#{territory} #{layer} #{level}_#{x}_#{y}"

    #Get the extent and the position of the corners of the tile
    gp_wgs84_level = dt_layer.wgs84_levels[level]
    
    uv_top_left = UV.new(dt_layer.wgs84_extent.top_left.u + x * gp_wgs84_level.world_per_tile.u,
                         dt_layer.wgs84_extent.top_left.v - y * gp_wgs84_level.world_per_tile.v)
    uv_bottom_right = UV.new(uv_top_left.u + gp_wgs84_level.world_per_tile.u,
                             uv_top_left.v - gp_wgs84_level.world_per_tile.v)
    @extent = Extent.new(uv_top_left,uv_bottom_right)
    
    #arbitrarily switch at 1.5
    lod_tile_min = (gp_wgs84_level.tile_size / 1.5).to_i
    
    if level == dt_layer.top_level
      @lod = [0,-1]
    else
      @lod = [lod_tile_min, -1]
    end

    if level == 0
      @sub_tiles = []
    else
      new_level = level - 1
      uv_center = UV.new((uv_top_left.u + uv_bottom_right.u)/2,
                         (uv_top_left.v + uv_bottom_right.v)/2)

      url_root = host + "/#{territory}/#{layer}/#{new_level}"
      
      @sub_tiles = [
        GeoportailTileLink.new(2 * x,2 * y,new_level,Extent.new(uv_top_left,uv_center),lod_tile_min,-1,url_root),
        GeoportailTileLink.new(2 * x + 1,2 * y,new_level,Extent.new(uv_center.u,uv_top_left.v,uv_bottom_right.u,uv_center.v),lod_tile_min,-1,url_root),
        GeoportailTileLink.new(2 * x,2 * y + 1,new_level,Extent.new(uv_top_left.u,uv_center.v,uv_center.u,uv_bottom_right.v),lod_tile_min,-1,url_root),
        GeoportailTileLink.new(2 * x + 1,2 * y + 1,new_level,Extent.new(uv_center,uv_bottom_right),lod_tile_min,-1,url_root)
      ]
    end
    
    @overlay_url = host + "/image/#{territory}/#{layer}/#{level}/#{x}/#{y}"
    render :content_type => "application/vnd.google-earth.kml+xml", :layout => false
  end

  #Fetch the image data and format it: will be the meat of the application
  def image
    territory = params[:territory].intern
    layer = params[:layer].intern
    level = params[:level].to_i
    x = params[:x].to_i
    y = params[:y].to_i
    
    dt_terr = DATA[territory]
    dt_layer = dt_terr[layer]
    
    #creates a cache directory
    cache_dir = cache_directory(territory,layer,level)

    #determinate a name for the file to send back
    tile_name = "GEt_#{x}_#{y}.jpg"
    result_tile_path = "#{cache_dir}#{tile_name}"
    
    if File.exists?(result_tile_path)
      #the file already exists: send it back directly
      open(result_tile_path,"rb") do |f|
        send_data f.read, :type => "image/jpeg", :filename => tile_name
      end
      return
    end
    
    dt_wgs84_level = dt_layer.wgs84_levels[level]
    
    #get the coordinates of the tile
    wgs84_extent_wn = dt_layer.wgs84_extent.top_left
    
    uv_wn = UV.new(wgs84_extent_wn.u + x * dt_wgs84_level.world_per_tile.u,
                   wgs84_extent_wn.v - y * dt_wgs84_level.world_per_tile.v)
    uv_es = UV.new(wgs84_extent_wn.u + (x + 1) * dt_wgs84_level.world_per_tile.u,
                   wgs84_extent_wn.v - (y + 1) * dt_wgs84_level.world_per_tile.v)
    
    result = get_image(territory,dt_terr,layer,dt_layer,:wgs84_latlon,
                       Extent.new(uv_wn,uv_es),"image/jpeg",WGS84_TILE_SIZE,WGS84_TILE_SIZE,
                       tile_name,:hint_level => level)
    
    if !result
      #error: send back a black pixel; else an image has been sent back
      open(RAILS_ROOT + "/public/images/na.jpg","rb") do |f|
        send_data f.read
      end
    end

  end

  #barebone WMS interface to the Geoportail data
  def wms
    #put all the parameters into smallcase; not really sure if can be done by rails directly (probably though...)
    self.params = downcase(params)
        
    request = params[:request]

    #don't verifiy the service : assumes it is WMS
    #service = params[:service]
    #return wms_exception("ServiceNotSupported") if service != "WMS"
    
    if request == "GetCapabilities"
      wms_capabilities
    elsif request == "GetMap"
      wms_map
    else
      return wms_exception("OperationNotSupported")
    end
  end

  private

  def wms_capabilities
    @territories = DATA
    @wms_data = WMS_DATA
    @online_resource = host + '/wms'
    render :action => :wms_capabilities, :layout => false
  end

  def wms_map
    #don't verify the version : assumes it is 1.3.0
    #version = params[:version]
    #return wms_exception("VersionNotSupported") if version != "1.3.0"
    
    #layer: must be defined
    layers = params[:layers]
    return wms_exception("LayerNotDefined") if layers.nil?
    #only one layer processed (layerlimit == 1 in my capabilities document anyway): the first one
    territory,layer = layers.split(",")[0].split("_")
    return wms_exception("LayerNotDefined") if territory.nil? or layer.nil?
    territory = territory.intern
    layer = layer.intern
    
    dt_terr = DATA[territory]
    return wms_exception("LayerNotDefined") if(dt_terr.nil?)
    dt_layer = dt_terr[layer]
    return wms_exception("LayerNotDefined") if(dt_layer.nil?)
    
    bbox = params[:bbox]
    return wms_exception("InvalidBoundingBox") if bbox.nil?
    bbox = bbox.split(",")
    return wms_exception("BoundingBoxFormatNotSupported") if bbox.length != 4
    #build the extent
    bbox.map! { |corner| corner.to_f }
    uv_wn = UV.new(bbox[0],bbox[3]) #minx / maxy
    uv_es = UV.new(bbox[2],bbox[1]) #maxx / miny
    extent = Extent.new(uv_wn,uv_es)
    
    crs = params[:crs]
    return wms_exception("InvalidCRS") if WMS_DATA[:crs][crs].nil?
    
    width = params[:width].to_i
    return wms_exception("InvalidWidth") if width <= 0
    height = params[:height].to_i
    return wms_exception("InvalidHeight") if height <= 0
    
    format = params[:format]
    return wms_exception("InvalidFormat") if !WMS_DATA[:format].include?(format)
    
    tile_name = "Wt_#{bbox.join("_")}.#{WMS_DATA[:format][format]}"
        
    #we ignore the style (it is unique anyway) and other optional parameters
    result = get_image(territory,dt_terr,layer,dt_layer,WMS_DATA[:crs][crs],
                       extent,format,width,height,
                       tile_name, :no_cache => true)

    
    wms_exception("InternalError") if !result
    
  end
  
  #extent is in the crs defined by the proj_string
  #options include:  :hint_level (the level to use ; will be computed if not given), :no_cache
  def get_image(territory,dt_terr,layer,dt_layer,proj_data,extent,format,width,height,result_tile_name,options = {})   
    uv_wn = extent.top_left
    uv_es = extent.bottom_right

    proj_correspondance = [uv_wn.u,(uv_es.u-uv_wn.u)/width,0,uv_wn.v,0,(uv_es.v-uv_wn.v)/height]
        
    corners = [uv_wn,uv_es,UV.new(uv_wn.u,uv_es.v),UV.new(uv_es.u,uv_wn.v)]
    unless proj_data == :wgs84_latlon
      #The projection in which we want the result image
      result_projection = Projection.new(proj_data);
      #convert the corners in wgs84 ; if already in wgs84 don't do anything
      corners.map! do |corner| 
        result_projection.inverse(corner)
      end
    else
      #in WGS84 ; GDAL will assume the unit is dec deg
      proj_data = ["proj=longlat"]
    end

    level = options[:hint_level]
    if level.nil?
      #compute a level by finding a view arc and using some formula
      min_lat=max_lat=min_lon=max_lon=nil
      corners.each do |corner|
        lon = corner.u
        lat = corner.v
        max_lon = lon if max_lon == nil or lon > max_lon 
        max_lat = lat if max_lat == nil or lat > max_lat
        min_lon = lon if min_lon == nil or lon < min_lon
        min_lat = lat if min_lat == nil or lat < min_lat
      end
      lon_extent = max_lon - min_lon
      lat_extent = max_lat - min_lat
      #arc_extent in degrees
      arc_extent = lon_extent > lat_extent ? lat_extent : lon_extent
      level = dt_layer.level_by_arc_extent(arc_extent/3) #/3 is totally arbitrary: seems to look ok
    end
    
    #project these corners in the crs of the territory
    pj_corners = []
    corners.each do |corner|
      #first inverse the extent to wgs84 then forward to the territory projection
      pj_corners << dt_terr.projection.forward(corner)
    end
    
    #Find the smallest block of gp tiles which contains this surface :
    #Find the max,min indices in x and y for this.
    dt_level = dt_layer.levels[level]
    max_i = max_j = min_i = min_j = nil
    unclamped_top_left = UV.new(Float::MAX, Float::MAX)
    unclamped_bottom_right = UV.new(-Float::MAX,-Float::MAX)

    pj_corners.each do |pj_corner|
      unclamped_top_left.u = pj_corner.u if unclamped_top_left.u > pj_corner.u
      unclamped_top_left.v = pj_corner.v if unclamped_top_left.v < pj_corner.v
      unclamped_bottom_right.u = pj_corner.u if unclamped_bottom_right.u < pj_corner.u
      unclamped_bottom_right.v = pj_corner.v if unclamped_bottom_right.v > pj_corner.v
            
      #For the indices : Relative to the north west corner of the extent
      pj_corner.u = pj_corner.u - dt_layer.extent.top_left.u
      pj_corner.v = dt_layer.extent.top_left.v - pj_corner.v
            
      i = (pj_corner.u / dt_level.world_per_tile.u).floor
      j = (pj_corner.v / dt_level.world_per_tile.v).floor
      
      max_i = i if max_i == nil or i > max_i 
      max_j = j if max_j == nil or j > max_j
      min_i = i if min_i == nil or i < min_i
      min_j = j if min_j == nil or j < min_j
      
    end
     
    max_i += 1 #should not have to do that but helps in solving the GDAL/Proj4 projection discrepancies for Lamb2e
        
    dx = unclamped_bottom_right.u - unclamped_top_left.u
    dy = unclamped_top_left.v - unclamped_bottom_right.v
    if dx > dy
      scale = compute_scale(dy,(max_j-min_j+1) * TILE_SIZE)
    else
      scale = compute_scale(dx,(max_i-min_i+1) * TILE_SIZE)
    end
        
    #get the components
    components = dt_layer.find_components_for_scale_extent(scale,Extent.new(unclamped_top_left,unclamped_bottom_right))
    #make the string to request, including the copyright url's
    layers = components.join(",")

    unless dt_layer.copyright.empty?
      layers += "," + dt_layer.copyright
    end
    
    #Test if outside the limit and clamp if it is
    if max_i < 0 or max_j < 0 or min_i > dt_level.max_tile_x or min_j > dt_level.max_tile_y
      #send back a black pixel
      open(RAILS_ROOT + "/public/images/na.jpg","rb") do |f|
        send_data f.read
      end
      return true
    else
      #clamp at 0
      min_i = 0 if min_i < 0
      min_j = 0 if min_j < 0
      #clamp at max_tile_x and max_tile_y
      max_i = dt_level.max_tile_x if max_i > dt_level.max_tile_x
      max_j = dt_level.max_tile_y if max_j > dt_level.max_tile_y
    end

    #Compute the WGS84 coordinates of all the corners of the tiles to be used
    num_i = max_i - min_i + 1
    num_j = max_j - min_j + 1
    
    #Coordinate of top_left coordinate for tile num_i, num_j
    top_left = UV.new(dt_layer.extent.top_left.u + dt_level.world_per_tile.u * min_i,
                      dt_layer.extent.top_left.v - dt_level.world_per_tile.v * min_j )
     
    correspondance = [top_left.u,dt_level.world_per_tile.u/TILE_SIZE,0,top_left.v,0,-dt_level.world_per_tile.v/TILE_SIZE]
    
    #get the cache dir of the gp tiles
    cache_dir = cache_directory(territory,layer,level)
    
    tile_paths = []
    threads = []
    
    #gets all the tiles
    min_i.upto(max_i) do |i|
      ind_i = i - min_i
      min_j.upto(max_j) do |j|
        ind_j = j - min_j
        
        tile_path = "#{cache_dir}t_#{i}_#{j}.jpg"
        url = URI.parse("http://cimg.geoportail.fr/ImageX/ImageX.dll?image?cache=true&transparent=true&type=jpg&l=#{level}&tx=#{i}&ty=#{j}&ts=#{TILE_SIZE}&fill=ffffff&quality=60&layers=#{layers}")
               
        tile_paths << tile_path
        #we perform the downloads in parrallel
        threads << Thread.new(tile_path,url) do |tile_path,url|
          if ! File.exists?(tile_path)
            begin
              res = Net::HTTP.start(url.host, url.port) do |http|
                http.request_get(url.request_uri, {'Referer' => "http://visu.geoportail.fr/",'User-Agent' => 'GEopochi'})
              end
              unless res.code == 200
                if res.code.to_i == 302 || res.code.to_i == 301
                  #redirection: get the new location
                  url = URI.parse(res['location'])
                  res = Net::HTTP.start(url.host, url.port) do |http|
                    http.request_get(url.request_uri, {'Referer' => "http://visu.geoportail.fr/",'User-Agent' => 'GEopochi'})
                  end
                end
              end
            rescue
              retry #try to redownload until no error: can be a infinie loop if no network...
            end
            #cache the downloaded tile locally
            open(tile_path,"wb") do |f|
              f.write res.body
            end
          end
                    
        end
      end
    end

    #create the image
    threads.each {|thread| thread.join}
    
    result_tile_path = "#{cache_dir}#{result_tile_name}";

    result = CreateImage.create(result_tile_path,format,num_i,num_j,
                                tile_paths,correspondance,dt_terr.projection_string,
                                proj_correspondance, "+" + proj_data.join(" +"),
                                TILE_SIZE,width,height)
    
    if result
      open(result_tile_path,"rb") do |f|
        send_data f.read, :type => format, :filename => result_tile_name
      end
      File.delete(result_tile_path) if options[:no_cache]
      return true
    else
      #let the caller decide what to do in case of a problem
      return false
    end
  end

  def wms_exception(error)
    @error = error
    render :action => :wms_exception,:layout => false
  end

  def host
    "http://" + @request.host_with_port
  end
  
  def cache_directory(territory,layer,level)
    dir = RAILS_ROOT + '/tmp/cache/'
    if ! File.exists?(dir)
      Dir.mkdir(dir)
    end
    dir += "#{territory}/"
    if ! File.exists?(dir)
      Dir.mkdir(dir)
    end
    dir += "#{layer}/"
    if ! File.exists?(dir)
      Dir.mkdir(dir)
    end
    dir += "#{level}/"
    if ! File.exists?(dir)
      Dir.mkdir(dir)
    end
    dir
  end

  def compute_scale(dx,tile_size)
    9600 *  dx / (tile_size * 2)
  end

  def downcase(hash)
    result = {}
    hash.each do |key,value|
      result[key.to_s.downcase.intern] = value
    end
    result
  end

end
