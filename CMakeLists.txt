# CMakeList.txt : Top-level CMake project file, do global configuration
# and include sub-projects here.
#
cmake_minimum_required(VERSION 3.8)

project("SMEIoT")

find_library(MOSQUITTO_LIBRARY mosquitto)

add_subdirectory("src/Mosquitto")
add_subdirectory("src/MosquittoPlugin")

