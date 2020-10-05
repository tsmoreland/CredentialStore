set(SHARED_LIBRARY_NAME ${CMAKE_PROJECT_NAME})
set (CMAKE_CXX_STANDARD 17)

include_directories(${CMAKE_SOURCE_DIR}/include)

set (SOURCE 
	"${CMAKE_SOURCE_DIR}/include/credential.h"
	"${CMAKE_SOURCE_DIR}/include/credential_manager.h"
	"${CMAKE_SOURCE_DIR}/include/credential_manager_interface.h"
	"${CMAKE_SOURCE_DIR}/include/credential_type.h"
	"${CMAKE_SOURCE_DIR}/include/either.h"
	"${CMAKE_SOURCE_DIR}/include/persistence_type.h"
	"${CMAKE_SOURCE_DIR}/include/result_t.h"
	"${CMAKE_SOURCE_DIR}/include/win32_exception.h"

	
	"../credential_factory_traits.h"
	"../credential_factory_traits.cpp"
	"../credential_manager.cpp"
	"../credential_manager_impl.h"
	"../credential_manager_impl_using_traits.h"
	"../credential_traits.h"
	"../credential_traits.cpp"
	"../credential_w_facade.h"
)

add_library(${SHARED_LIBRARY_NAME} SHARED ${SOURCE})
set_target_properties(${CMAKE_PROJECT_NAME} PROPERTIES VS_GLOBAL_KEYWORD "Win32Proj")
set_target_properties(${SHARED_LIBRARY_NAME} PROPERTIES
	OUTPUT_DIRECTORY_DEBUG   "${CMAKE_SOURCE_DIR}/target/${CMAKE_VS_PLATFORM_NAME}/$<CONFIG>/"
	OUTPUT_DIRECTORY_RELEASE "${CMAKE_SOURCE_DIR}/target/${CMAKE_VS_PLATFORM_NAME}/$<CONFIG>/"
)

target_compile_definitions(${SHARED_LIBRARY_NAME} PRIVATE
	"$<$<CONFIG:DEBUG>:" 
		"_DEBUG" 
	">"
	"$<$<CONFIG:RELEASE>:" 
		"_NDEBUG" 
	">"
	"WIN32;"
	"_CONSOLE;"
	"UNICODE;"
	"_UNICODE;"
	"WIN32_CREDENTIAL_STORE_SHARED_LIBRARY"
	"WIN32_CREDENTIAL_STORE_EXPORTS"
)

if (MSVC)

	target_compile_options(${SHARED_LIBRARY_NAME} PRIVATE
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

	target_link_options(${SHARED_LIBRARY_NAME} PRIVATE
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

set_target_properties(${SHARED_LIBRARY_NAME} PROPERTIES VS_GLOBAL_KEYWORD "Win32Proj")
