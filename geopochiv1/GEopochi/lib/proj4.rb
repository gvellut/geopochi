#set the path to the initialization file
ENV['PROJ_LIB'] = File.dirname(__FILE__) + '/NAD'
#load the library per se
require 'projrb'
