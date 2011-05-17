// VUZIX DRIVER WRAPPER
// Wrapper functions to map the sdk library of the Vuzix iWear tracker to a
// dynamic library that can be called from c# scripts inside Unity3D
// Uses the iweardrv.h file in the inc folder and the iweardrv.lib library in the
// lib folder.
// To use this, compile to a dll. (currently found in the Release folder)
//
// Authors: Maarten van der Velden (maarten DOT vandervelden AT student DOT uva DOT nl),
//          Sicco van Sas
//          Daniël Karavolos
//
// Kinect & iWear Superman project
// 
// University of Amsterdam - Intelligent Systems Lab (ISLA)
// last update: 02/15/2011
// 

#define IWEARDRV_EXPLICIT
#include "afxwin.h"
#include <iweardrv.h>

extern "C" __declspec(dllexport) DWORD WrapIWROpenTracker(){
	HINSTANCE m_hIwear = LoadLibrary(TEXT("IWEARDRV.DLL"));
	IWROpenTracker = (PIWROPENTRACKER)GetProcAddress(m_hIwear, "IWROpenTracker");
	return IWROpenTracker();
}

extern "C" __declspec(dllexport) DWORD WrapIWRGetTracking(long *yaw, long *pitch, long *roll){
	HINSTANCE m_hIwear = LoadLibrary(TEXT("IWEARDRV.DLL"));
	IWRGetTracking = (PIWRGETTRACKING)GetProcAddress(m_hIwear, "IWRGetTracking");
	return IWRGetTracking(yaw, pitch, roll);
}

extern "C" __declspec(dllexport) void WrapIWRCloseTracker(){
	HINSTANCE m_hIwear = LoadLibrary(TEXT("IWEARDRV.DLL"));
	IWRCloseTracker = (PIWRCLOSETRACKER)GetProcAddress(m_hIwear, "IWRCloseTracker");
	IWRCloseTracker();
}