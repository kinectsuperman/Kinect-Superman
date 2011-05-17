/*-----------------------------------------------------------------------------
 * iweardrv.h
 *
 * Copyright (c) 2007 Icuiti Corporation.  All rights reserved.
 *
 * Header file for the Icuiti iWear SDK 
 *
 * Revision History:
 * $Log: /227-VR920/IWEARSDK/inc/iweardrv.h $
 * 
 * 9     6/18/10 9:58a Steves
 * 
 * 8     3/22/10 9:38a Steves
 * 
 * 7     3/01/10 8:36a Steves
 * 
 * 6     1/13/10 2:59p Steves
 * 
 * 5     12/16/09 11:11a Steves
 * 
 * 4     9/25/08 10:44a Steves
 * 
 * 3     6/19/07 4:14p Steves
 * 
 * 2     5/24/07 2:31p Steves
 * 
 * 1     4/10/07 9:52a Steves
 * 
 * 1     4/10/07 9:33a Steves
 * 
 */
#ifndef _IWEARDRV_H
#define _IWEARDRV_H

#ifdef __cplusplus
extern "C" {
#endif

// iWear Tracker Product IDs
#define IWR_PROD_NONE    0    // indicated an unsupported product
#define IWR_PROD_VR920   227
#define IWR_PROD_WRAP920 329

typedef struct tag_IWRVERSION {
	unsigned short DLLMajor;
	unsigned short DLLMinor;
	unsigned short DLLSubMinor;
	unsigned short DLLBuildNumber;
	char USBFirmwareMajor;
	char USBFirmwareMinor;
	char TrackerFirmwareMajor;
	char TrackerFirmwareMinor;
	char VideoFirmware;
} IWRVERSION, *PIWRVERSION;

typedef struct tag_IWRMAGSENSOR {
    unsigned char magx_lsb;
    unsigned char magx_msb;
    unsigned char magy_lsb;
    unsigned char magy_msb;
    unsigned char magz_lsb;
    unsigned char magz_msb;
} IWRMAGSENSOR, *IWRPMAGSENSOR;

typedef struct tag_IWRACCELSENSOR {
    unsigned char accx_lsb;
    unsigned char accx_msb;
    unsigned char accy_lsb;
    unsigned char accy_msb;
    unsigned char accz_lsb;
    unsigned char accz_msb;
} IWRACCELSENSOR, *PIWRACCELSENSOR;

// High Bandwidth Gyros 2000 degrees per second
// = 1.465 dps/cnt
typedef struct tag_IWRGYROSENSOR {
    unsigned char gyx_lsb;
    unsigned char gyx_msb;
    unsigned char gyy_lsb;
    unsigned char gyy_msb;
    unsigned char gyz_lsb;
    unsigned char gyz_msb;
} IWRGYROSENSOR, *PIWRGYROSENSOR;

// High Bandwidth Gyros 500 degrees per second
// = 0,366 dps/cnt
typedef struct tag_IWRLBGYROSENSOR {
    unsigned char gyx_lsb;
    unsigned char gyx_msb;
    unsigned char gyy_lsb;
    unsigned char gyy_msb;
    unsigned char gyz_lsb;
    unsigned char gyz_msb;
} IWRLBGYROSENSOR, *PIWRLBGYROSENSOR;

typedef struct tag_IWRSENSDATA {
    IWRMAGSENSOR mag_sensor;
    IWRACCELSENSOR acc_sensor;
    IWRGYROSENSOR gyro_sensor;
    IWRLBGYROSENSOR lbgyro_sensor;
} IWRSENSDATA, *PIWRSENSDATA;

#ifndef IWEARDRV_EXPLICIT
#ifdef IWEARDRV_EXPORTS
__declspec( dllexport ) DWORD __cdecl IWROpenTracker( void );
__declspec( dllexport ) void __cdecl IWRCloseTracker( void );
__declspec( dllexport ) void __cdecl IWRZeroSet( void );
//__declspec( dllexport ) void __cdecl IWRZeroGyros( void );
//__declspec( dllexport ) void __cdecl IWRZeroClear( void );
__declspec( dllexport ) DWORD __cdecl IWRGetTracking(LONG *yaw, LONG *pitch, LONG *roll);
__declspec( dllexport ) DWORD __cdecl IWRGet6DTracking(LONG *yaw, LONG *pitch, LONG *roll, LONG *xtrn, LONG *ytrn, LONG *ztrn);
__declspec( dllexport ) DWORD __cdecl IWRGetFilteredSensorData(LONG *ax, LONG *ay, LONG *az, LONG *lgx, LONG *lgy, LONG *lgz, LONG *gx, LONG *gy, LONG *gz, LONG *mx, LONG *my, LONG *mz);
__declspec( dllexport ) DWORD __cdecl IWRGetSensorData(PIWRSENSDATA sensdataptr);
__declspec( dllexport ) DWORD __cdecl IWRBeginCalibrate(void);
__declspec( dllexport ) void __cdecl IWREndCalibrate(BOOL save);
__declspec( dllexport ) DWORD __cdecl IWRGetVersion(PIWRVERSION ver);
__declspec( dllexport ) void __cdecl IWRSetFilterState(BOOL on);
__declspec( dllexport ) BOOL __cdecl IWRGetFilterState(void);
__declspec( dllexport ) WORD __cdecl IWRGetProductID(void);
#else
__declspec( dllimport ) extern DWORD IWROpenTracker( void );
__declspec( dllimport ) extern void IWRCloseTracker( void );
__declspec( dllimport ) extern void IWRZeroSet( void );
//__declspec( dllimport ) extern void IWRZeroGyros( void );
//__declspec( dllimport ) extern void IWRZeroClear( void );
__declspec( dllimport ) extern DWORD IWRGetTracking(LONG *yaw, LONG *pitch, LONG *roll);
__declspec( dllimport ) extern DWORD IWRGet6DTracking(LONG *yaw, LONG *pitch, LONG *roll, LONG *xtrn, LONG *ytrn, LONG *ztrn);
__declspec( dllimport ) extern DWORD IWRGetFilteredSensorData(LONG *ax, LONG *ay, LONG *az, LONG *lgx, LONG *lgy, LONG *lgz, LONG *gx, LONG *gy, LONG *gz, LONG *mx, LONG *my, LONG *mz);
__declspec( dllimport ) extern DWORD IWRGetSensorData(PIWRSENSDATA sensdataptr);
__declspec( dllexport ) extern DWORD IWRBeginCalibrate(void);
__declspec( dllexport ) extern void IWREndCalibrate(BOOL save);
__declspec( dllimport ) extern DWORD IWRGetVersion(PIWRVERSION ver);
__declspec( dllimport ) extern void IWRSetFilterState(BOOL on);
__declspec( dllimport ) extern BOOL IWRGetFilterState(void);
__declspec( dllimport ) extern WORD IWRGetProductID(void);
#endif
#else
typedef DWORD	(__cdecl *PIWROPENTRACKER)( void );
typedef void	(__cdecl *PIWRCLOSETRACKER)( void );
typedef void	(__cdecl *PIWRZEROSET)( void );
//typedef void	(__cdecl *PIWRZEROGYROS)( void );
//typedef void	(__cdecl *PIWRZEROCLEAR)( void );
typedef DWORD	(__cdecl *PIWRGETTRACKING)(LONG *, LONG *, LONG *);
typedef DWORD	(__cdecl *PIWRGET6DTRACKING)(LONG *, LONG *, LONG *, LONG *, LONG *, LONG *);
typedef DWORD	(__cdecl *PIWRGETFILTEREDSENSORDATA)(LONG *, LONG *, LONG *, LONG *, LONG *, LONG *, LONG *, LONG *, LONG *, LONG *, LONG *, LONG *);
typedef DWORD	(__cdecl *PIWRGETSENSORDATA)(PIWRSENSDATA);
typedef DWORD	(__cdecl *PIWRGETVERSION)(PIWRVERSION);
typedef void	(__cdecl *PIWRSETFILTERSTATE)( BOOL );
typedef BOOL	(__cdecl *PIWRGETFILTERSTATE)(void);
typedef WORD	(__cdecl *PIWRGETPRODUCTID)(void);

PIWROPENTRACKER IWROpenTracker;
PIWRCLOSETRACKER IWRCloseTracker;
PIWRZEROSET IWRZeroSet;
//PIWRZEROGYROS IWRZeroGyros;
PIWRGETTRACKING IWRGetTracking;
PIWRGET6DTRACKING IWRGet6DTracking;
PIWRGETFILTEREDSENSORDATA IWRGetFilteredSensorData;
PIWRGETSENSORDATA IWRGetSensorData;
PIWRGETVERSION IWRGetVersion;
PIWRSETFILTERSTATE IWRSetFilterState;
PIWRGETFILTERSTATE IWRGetFilterState;
PIWRGETPRODUCTID IWRGetProductID;
#endif

#ifdef __cplusplus
}
#endif

#endif