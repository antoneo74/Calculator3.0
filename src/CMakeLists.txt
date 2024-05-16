cmake_minimum_required(VERSION 3.5)

project(CalculatorDll)

include(GNUInstallDirs)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

if(CMAKE_COMPILER_IS_GNUCXX)
    message(STATUS "GCC detected, adding compile flags")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pedantic -Wall -Wextra -Werror")
endif(CMAKE_COMPILER_IS_GNUCXX)

# Directories
set(BASE_DIR ${CMAKE_CURRENT_SOURCE_DIR})
set(SOURCE_DIR ${CMAKE_CURRENT_SOURCE_DIR}/Core/CalculatorDll)
set ( CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON )

# Sources
file(GLOB_RECURSE ALL_SOURCES 
${SOURCE_DIR}/*.cc
${SOURCE_DIR}/*.cpp 
${SOURCE_DIR}/*.h
${SOURCE_DIR}/*.h)

add_library(${PROJECT_NAME} SHARED ${ALL_SOURCES})
set_target_properties(${PROJECT_NAME} PROPERTIES PREFIX "")
