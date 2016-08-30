#include <windows.h>

#include "Gmapi.h"

#define DLL_EXPORT __declspec(dllexport)

using namespace gm;

CGMAPI* g_pGmapi;
CRITICAL_SECTION critSec;
//FILE* fp;

#ifdef __cplusplus
extern "C"
{
#endif
	struct Guard
	{
		CRITICAL_SECTION& critSec;
		Guard(CRITICAL_SECTION& critSec)
			: critSec(critSec)
		{
			EnterCriticalSection(&critSec);
		}
		~Guard() {
			LeaveCriticalSection(&critSec);
		}
		Guard(const Guard&) = delete;
		Guard& operator = (const Guard&) = delete;
	};

	DLL_EXPORT double gm_gmlproxy(double ind, char* msg)
	{
		int ret = 0;

		if (script_exists((int)ind))
		{
			if(!instance_exists(g_pGmapi->GetCurrentInstancePtr()->object_index)) return 1;
			Guard guard(critSec);
			CGMVariable args[1];
			args[0].Set(msg);
			script_execute((int)ind, args, 1);
		}

		return ret;
	}

	DLL_EXPORT double gm_init_api()
	{
		return 0;
	}

#ifdef __cplusplus
}
#endif

BOOL WINAPI
DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	static PFUNCTIONDATA functionData = NULL;
	DWORD result = 0;

	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		//fopen_s(&fp, "out.txt", "w");
		InitializeCriticalSection(&critSec);
		g_pGmapi = CGMAPI::Create(&result);
		functionData = g_pGmapi->PreserveFunctionData();
		g_pGmapi->PatchIdentifierTypeCheckOrder();
		break;
	case DLL_PROCESS_DETACH:
		g_pGmapi->RestoreFunctionData(functionData);
		g_pGmapi->Destroy();
		DeleteCriticalSection(&critSec);
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	default:
		break;
	}

	return TRUE;
}