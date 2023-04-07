

namespace Common
{

    public enum ServiceType
    {
        Package,
        Cold,
        Hot
    }

    public enum HotServiceType
    {
        Ilik,
        Orta,
        Sicak
    }

    public enum TestType
    {
        SingleWrite,
        SingleRead,
        MultipleWrite,
        MultipleRead,
    }



    public enum DeviceType
    {
        CafeUnite,
        CafeKapUnite,
        CafeRobotTutucuKiskacUnite,
        CafeOtomatUnite,
        XArmRobotArm
    }

    public enum KapakType
    {
        KaseKapak,
        BardakKapak
    }

    public enum KapType
    {
        Kase,
        Bardak
    }

    public enum AmbalajType
    {
        Size_11,
        Size_12,
        Size_13
    }

    public enum Mode
    {
        Idle,
        SaleService,
        Yukleme,
        Bakim
    }

    public enum OtomatState
    {
        Normal,
        OutService
    }

    
}
