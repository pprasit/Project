using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataKeeper.Engine.Command
{
    public enum ASTROSERVERSET
    {
        NULL,
        ASTROSERVER_DATABASE_SYNC
    }

    public enum TS700MMSET
    {
        NULL,

        //------------------------------------Both Focuser---------------------------------------------

        TS700MM_FOCUSER_SETCONNECT,
        TS700MM_FOCUSER_SETDISCONNECT,
        TS700MM_FOCUSER_SETPOSITION,        //int Position (Micron)
        TS700MM_FOCUSER_SETINCREMENT,      //int Position (Micron)
        TS700MM_FOCUSER_SETSTOP,
        TS700MM_FOCUSER_STARTAUTOFOCUS,

        //------------------------------------Focuser 1---------------------------------------------
        TS700MM_FOCUSER1_SETCONNECT,
        TS700MM_FOCUSER1_SETDISCONNECT,
        TS700MM_FOCUSER1_SETPOSITION,        //int Position (Micron)
        TS700MM_FOCUSER1_SETINCREMENT,      //int Position (Micron)
        TS700MM_FOCUSER1_SETSTOP,
        TS700MM_FOCUSER1_STARTAUTOFOCUS,

        //------------------------------------Focuser 2---------------------------------------------
        TS700MM_FOCUSER2_SETCONNECT,
        TS700MM_FOCUSER2_SETDISCONNECT,
        TS700MM_FOCUSER2_SETPOSITION,        //int Position (Micron)
        TS700MM_FOCUSER2_SETINCREMENT,      //int Position (Micron)
        TS700MM_FOCUSER2_SETSTOP,
        TS700MM_FOCUSER2_STARTAUTOFOCUS,

        //------------------------------------Both Rotator---------------------------------------------
        TS700MM_ROTATOR_SETPOSITION,        //Decimal Degree
        TS700MM_ROTATOR_SETINCREMENT,        //Decimal Degree
        TS700MM_ROTATOR_SETSTOP,
        TS700MM_ROTATOR_SETDEROTATORSTART,
        TS700MM_ROTATOR_SETDEROTATORSTOP,

        //------------------------------------Rotator 1---------------------------------------------
        TS700MM_ROTATOR1_SETPOSITION,        //Decimal Degree
        TS700MM_ROTATOR1_SETINCREMENT,        //Decimal Degree
        TS700MM_ROTATOR1_SETSTOP,
        TS700MM_ROTATOR1_SETDEROTATORSTART,
        TS700MM_ROTATOR1_SETDEROTATORSTOP,

        //------------------------------------Rotator 2---------------------------------------------
        TS700MM_ROTATOR2_SETPOSITION,        //Decimal Degree
        TS700MM_ROTATOR2_SETINCREMENT,        //Decimal Degree
        TS700MM_ROTATOR2_SETSTOP,
        TS700MM_ROTATOR2_SETDEROTATORSTART,
        TS700MM_ROTATOR2_SETDEROTATORSTOP,

        //------------------------------------Mount---------------------------------------------
        TS700MM_MOUNT_SETCONNECT,
        TS700MM_MOUNT_SETDISCONNECT,
        TS700MM_MOUNT_SETENABLE,
        TS700MM_MOUNT_SETDISABLE,
        TS700MM_MOUNT_SLEWINCREMENTRADEC,  //Decimal Ra (Arcsecs), Decimal Dec (Arcsecs)
        TS700MM_MOUNT_SLEWINCREMENTAZMALT, //Decimal Azm (Arcsecs), Decimal Alt (Arcsecs)
        TS700MM_MOUNT_SLEWRADEC,           //Decimal Ra (Ra Hours), Decimal Dec (Dec Degree)
        TS700MM_MOUNT_SLEWRADEC2000,       //Decimal Ra (Ra Hours), Decimal Dec (Dec Degree)
        TS700MM_MOUNT_SLEWAZMALT,          //Decimal Alt (Degree), Decimal Azm (Degree)
        TS700MM_MOUNT_SETSTOP,
        TS700MM_MOUNT_SETTRACKINGON,
        TS700MM_MOUNT_SETTRACKINGOFF,
        TS700MM_MOUNT_SETTRACKINGRATES,    //Decimal RaRate (Arcsecs/Sec), Decimal DecRate (Arcsecs/Sec)
        TS700MM_MOUNT_SETMOUNTMODEL,       //String FileName
        TS700MM_MOUNT_SETJOG,              //Decimal Azm/RA (Degree/Sec), Decimal Alt/Dec (Degree/Sec)
        TS700MM_MOUNT_SETHOME,

        //------------------------------------M3---------------------------------------------
        TS700MM_M3_SETCONNECT,
        TS700MM_M3_SETDISCONNECT,
        TS700MM_M3_SETSELECTPORT,          //int PortNum (1 or 2)
        TS700MM_M3_SETSTOP
    }

    public enum IMAGINGSET
    {
        NULL,

        IMAGING_CCD_CONNECT,
        IMAGING_CCD_DISCONNECT,
        IMAGING_CCD_CAMERA,
        IMAGING_CCD_EXPOSE,
        IMAGING_CCD_ABORTEXPOSE,
        IMAGING_CCD_QUIT,
        IMAGING_CCD_FITSKEY,
        IMAGING_CCD_FULLFRAME,
        IMAGING_CCD_SHOWWINDOW,
        IMAGING_CCD_STARTDOWNLOAD,
        IMAGING_CCD_AUTODOWNLOAD,
        IMAGING_CCD_BINXY,
        IMAGING_CCD_COOLERON,
        IMAGING_CCD_FANENABLE,
        IMAGING_CCD_FASTREADOUT,
        IMAGING_CCD_FILTER,
        IMAGING_CCD_SUBFRAMESIZE,
        IMAGING_CCD_READOUTMODE,
        IMAGING_CCD_SETSPEED,
        IMAGING_CCD_SUBFRAMESTARTXY,
        IMAGING_CCD_TEMPERATURESETPOINT,
        IMAGING_IMAGE_DISPLAY_SIZE,
        IMAGING_IMAGE_STRETCH_PROFILE,
        IMAGING_IMAGE_FILENAME,

        //------------------------------------Imaging Application---------------------------------------------
        IMAGING_Imaging_CREATECONNECTION,
        IMAGING_Imaging_DISCONNECT,
    }

    public enum LANOUTLETSET
    {
        NULL,

        LANOUTLET_SWITCHONOFF,
        LANOUTLET_SWITCHSWITCHING,
    }

    public enum DOMESET
    {
        NULL,

        Dome_FULLYOPENSHUTTERA,
        Dome_FULLYOPENSHUTTERB,
        Dome_FULLYCLOSESHUTTERA,
        Dome_FULLYCLOSESHUTTERB,
        Dome_OPENSHUTTERA,
        Dome_OPENSHUTTERB,
        Dome_CLOSESHUTTERA,
        Dome_CLOSESHUTTERB
    }
}
