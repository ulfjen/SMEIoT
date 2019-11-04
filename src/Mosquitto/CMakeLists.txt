# CMakeList.txt : CMake project for Mosquitto, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

add_library(mosq SHARED "client.c" "client.h")
target_include_directories(mosq PUBLIC "C:\\Dev\\vcpkg\\installed\\x64-windows\\include")
target_link_libraries(mosq PRIVATE ${MOSQUITTO_LIBRARY})
target_compile_definitions(mosq PRIVATE LIBRARY_EXPORTS=1)

