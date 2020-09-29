set(TEST_PROJECT_NAME ${CMAKE_PROJECT_NAME}_test)
set (CMAKE_CXX_STANDARD 17)

find_package(GTest REQUIRED)
include_directories(${CMAKE_SOURCE_DIR}/include)

set (SOURCE 
 "credential_tests.cpp"
 "credential_manager_tests.cpp")

add_executable(${TEST_PROJECT_NAME} WIN32 ${SOURCE})
add_test(NAME ${TEST_PROJECT_NAME} COMMAND ${TEST_PROJECT_NAME})

set_target_properties(${TEST_PROJECT_NAME} PROPERTIES VS_GLOBAL_KEYWORD "Win32Proj")

# Output directory
set_target_properties(${TEST_PROJECT_NAME} PROPERTIES
	INTERPROCEDURAL_OPTIMIZATION_RELEASE "TRUE"
	OUTPUT_DIRECTORY_DEBUG   "${CMAKE_SOURCE_DIR}/target/${CMAKE_VS_PLATFORM_NAME}/$<CONFIG>/"
	OUTPUT_DIRECTORY_RELEASE "${CMAKE_SOURCE_DIR}/target/${CMAKE_VS_PLATFORM_NAME}/$<CONFIG>/"
)

target_link_libraries(${TEST_PROJECT_NAME} PUBLIC ${CMAKE_PROJECT_NAME}_static GTest::GTest GTest::Main)

target_compile_definitions(${TEST_PROJECT_NAME} PRIVATE
	"$<$<CONFIG:DEBUG>:" 
		"_DEBUG" 
	">"
	"$<$<CONFIG:RELEASE>:" 
		"_NDEBUG" 
	">"
	"WIN32;"
	"_CONSOLE;"
	"UNICODE;"
	"_UNICODE;")

if (MSVC)

	target_compile_options(${TEST_PROJECT_NAME} PRIVATE
		$<$<CONFIG:Debug>:
			${DEFAULT_CXX_DEBUG_RUNTIME_LIBRARY}
		>
		$<$<CONFIG:Release>:
			/Oi;
			${DEFAULT_CXX_RUNTIME_LIBRARY};
			/Gy
		>
		/permissive-;
		/sdl;
		/W4;
		/EHsc;
		${DEFAULT_CXX_DEBUG_INFORMATION_FORMAT};
		${DEFAULT_CXX_EXCEPTION_HANDLING}
	)

	target_link_options(${TEST_PROJECT_NAME} PRIVATE
		$<$<CONFIG:Debug>:
			/DEBUG;
			/INCREMENTAL 
		> 
		$<$<CONFIG:Release>:
			/OPT:REF;
			/OPT:ICF;
			/INCREMENTAL:NO
		> 
		/SUBSYSTEM:CONSOLE
	)
endif()

set_target_properties(${TEST_PROJECT_NAME} PROPERTIES VS_GLOBAL_KEYWORD "Win32Proj")
