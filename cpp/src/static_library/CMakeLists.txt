set(STATIC_LIBRARY_NAME ${CMAKE_PROJECT_NAME}_static)
set (CMAKE_CXX_STANDARD 17)

include_directories(${CMAKE_SOURCE_DIR}/include)

set (SOURCE "../credential_manager.cpp")

add_library(${STATIC_LIBRARY_NAME} STATIC ${SOURCE})

set_target_properties(${CMAKE_PROJECT_NAME}_static PROPERTIES VS_GLOBAL_KEYWORD "Win32Proj")

# Output directory
set_target_properties(${STATIC_LIBRARY_NAME} PROPERTIES
	OUTPUT_DIRECTORY_DEBUG   "${CMAKE_SOURCE_DIR}/target/${CMAKE_VS_PLATFORM_NAME}/$<CONFIG>/"
	OUTPUT_DIRECTORY_RELEASE "${CMAKE_SOURCE_DIR}/target/${CMAKE_VS_PLATFORM_NAME}/$<CONFIG>/"
)

target_compile_definitions(${STATIC_LIBRARY_NAME} PRIVATE
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

	target_compile_options(${STATIC_LIBRARY_NAME} PRIVATE
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

	target_link_options(${STATIC_LIBRARY_NAME} PRIVATE
		$<$<CONFIG:Debug>:
			/DEBUG;
			/INCREMENTAL 
		> 
		$<$<CONFIG:Release>:
			/OPT:REF;
			/OPT:ICF;
			/INCREMENTAL:NO
		> 
	)
endif()

set_target_properties(${STATIC_LIBRARY_NAME} PROPERTIES VS_GLOBAL_KEYWORD "Win32Proj")
