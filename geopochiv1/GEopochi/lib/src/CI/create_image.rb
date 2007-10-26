ENV['GDAL_DATA'] = File.dirname(__FILE__) + '/data'
ENV['PATH'] = File.dirname(__FILE__) + ";" + ENV['PATH']
require 'CI'
