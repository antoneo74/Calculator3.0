CMAKE_MINIMUM_REQUIRED(VERSION 3.5)

PROJECT(CalculatorTests)

set(USED_CMAKE_GENERATOR "${CMAKE_GENERATOR}" CACHE STRING "Expose CMAKE_GENERATOR" FORCE)

add_subdirectory(../Calculator/Core/CalculatorDll bin)
