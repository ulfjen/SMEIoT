# CMakeList.txt : CMake project for Mosquitto, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

set(CMAKE_C_FLAGS_DEBUG "${CMAKE_C_FLAGS_DEBUG} -DENABLE_DEBUG_MACRO")
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} -DENABLE_DEBUG_MACRO")

add_library(mosqauth SHARED "auth.c" "auth.h")
if (${CMAKE_SYSTEM_NAME} EQUAL "Windows")
  target_include_directories(mosqauth PUBLIC "C:\\Dev\\vcpkg\\installed\\x64-windows\\include")
elseif (${CMAKE_SYSTEM_NAME} EQUAL "Darwin")
  target_include_directories(mosqauth PUBLIC "~/Dev/vcpkg/packages/mosquitto_x64-osx/include")
else ()
  target_include_directories(mosqauth PUBLIC "~/Dev/vcpkg/packages/mosquitto_x64-linux/include")
endif()
target_link_libraries(mosqauth PRIVATE ${MOSQUITTO_LIBRARY})
target_compile_definitions(mosqauth PRIVATE LIBRARY_EXPORTS=1)

if (CMAKE_BUILD_TYPE MATCHES Debug)
  install(TARGETS mosqauth
          LIBRARY DESTINATION ${CMAKE_BINARY_DIR}
          )
elseif(CMAKE_BUILD_TYPE MATCHES Release)
  install(TARGETS mosqauth
          LIBRARY DESTINATION /var/SMEIoT
          )
endif ()
