# CMakeList.txt : CMake project for Mosquitto, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

add_library(mosq SHARED "client.c" "client.h")
if (${CMAKE_SYSTEM_NAME} EQUAL "Windows")
  target_include_directories(mosq PUBLIC "C:\\Dev\\vcpkg\\installed\\x64-windows\\include")
elseif (${CMAKE_SYSTEM_NAME} EQUAL "Darwin")
  target_include_directories(mosq PUBLIC "~/Dev/vcpkg/packages/mosquitto_x64-osx/include")
else ()
  target_include_directories(mosq PUBLIC "~/Dev/vcpkg/packages/mosquitto_x64-linux/include")
endif()
target_link_libraries(mosq PRIVATE ${MOSQUITTO_LIBRARY})
target_compile_definitions(mosq PRIVATE LIBRARY_EXPORTS=1)

if (CMAKE_BUILD_TYPE MATCHES Debug)
  install(TARGETS mosq
          LIBRARY DESTINATION ${CMAKE_SOURCE_DIR}/src/SMEIoT.Infrastructure/bin/Debug/netcoreapp3.1
          )
  install(TARGETS mosq
          LIBRARY DESTINATION ${CMAKE_SOURCE_DIR}/src/SMEIoT.Web/bin/Debug/netcoreapp3.1
          )
elseif(CMAKE_BUILD_TYPE MATCHES Release)
  install(TARGETS mosq
          LIBRARY DESTINATION /var/SMEIoT
          )
endif ()
