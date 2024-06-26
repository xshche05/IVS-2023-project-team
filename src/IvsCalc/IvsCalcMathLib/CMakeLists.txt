﻿cmake_minimum_required(VERSION 3.24)
project(IvsCalcMathLib)

set(CMAKE_CXX_STANDARD 20)

add_executable(MathLib main.cpp IvsCalcMathLib.h IvsCalcMathLib.cpp)

include(FetchContent)
FetchContent_Declare(
    googletest
    URL https://github.com/google/googletest/archive/03597a01ee50ed33e9dfd640b249b4be3799d395.zip
)
# For Windows: Prevent overriding the parent project's compiler/linker settings
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

add_executable(IvsCalcMathLibTest IvsCalcMathLibTest.cpp IvsCalcMathLib.cpp IvsCalcMathLib.h)
target_link_libraries(IvsCalcMathLibTest GTest::gtest_main)

include(GoogleTest)
gtest_discover_tests(IvsCalcMathLibTest)